using System.Collections;
using UnityEngine;


// Висит на каждом оружии. Характеристики настраиваются индивидуально
public class Gun : MonoBehaviour
{
    [Header("Gun Settings")]

    [SerializeField]
    private Transform _bulletSpawnPoint;
    [SerializeField]
    private int _magazineCapacity;
    [SerializeField]
    private float _reloadTime = 0.3f;
    [SerializeField]
    private float _msBetweenShots = 100;
    [SerializeField]
    private float _muzzleVelocity = 35;

    [SerializeField]
    private float _shootDelay = 0.05f;
    [SerializeField]
    private LayerMask _mask;
    [SerializeField]
    private float _bulletSpeed = 100;
    [SerializeField]
    private float _range = 30f;
    [SerializeField]
    private bool _addBulletSpread = true;
    [SerializeField]
    private Vector3 _bulletSpreadVariance = new Vector3(0.05f, 0.05f, 0.05f);

    [SerializeField]
    private ParticleSystem _shootingParticle;
    [SerializeField]
    private TrailRenderer _bulletTrail;

    private Animator _animator;
    private float _lastShootTime; // ?

    private bool _isReloading;
    private float _nextShotTime;
    private bool _triggerReleasedSinceLastShot;
    private int _bulletsInMagazine;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _bulletsInMagazine = _magazineCapacity;
    }

    private void LateUpdate()
    {
        if (!_isReloading && _bulletsInMagazine == 0)
            Reload();
    }

    public void Shoot()
    {
        _bulletsInMagazine--;
        _nextShotTime = Time.time + _msBetweenShots / 1000;

        if (_lastShootTime + _shootDelay < Time.deltaTime)  // если по КД можно стрелять
        {
            // Make shot
            _animator.SetBool("Fire", true);     // trigger? переключаем аниматор в режим стрельбы

            _shootingParticle.Play();            // включаем партикл систем

            Vector3 _direction = GetDirection(); // определяем направление стрельбы

            if (Physics.Raycast(_bulletSpawnPoint.position, _direction, out RaycastHit _hit, float.MaxValue, _mask))   // если попали во что-то
            {
                TrailRenderer _trail = Instantiate(_bulletTrail, _bulletSpawnPoint.position, Quaternion.identity);  // делаем след

                StartCoroutine(SpawnTrail(_trail, _hit.point, _hit.normal));

                _lastShootTime = Time.time;
            }
            else
            {
                TrailRenderer _trail = Instantiate(_bulletTrail, _bulletSpawnPoint.position, Quaternion.identity);

                StartCoroutine(SpawnTrail(_trail, transform.forward * 100, Vector3.zero));

                _lastShootTime = Time.time;
            }
        }
    }


    private Vector3 GetDirection()
    {
        Vector3 _direction = transform.forward;

        if (_addBulletSpread) // если делаем разброс, то он задаётся путём рандомизации координат вектора направления
        {
            _direction += new Vector3(
                Random.Range(-_bulletSpreadVariance.x, _bulletSpreadVariance.x),
                Random.Range(-_bulletSpreadVariance.y, _bulletSpreadVariance.y),
                Random.Range(-_bulletSpreadVariance.z, _bulletSpreadVariance.z));

            _direction.Normalize();
        }
        return _direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer _trail, Vector3 _hitPoint, Vector3 _hitNormal)
    {
        Vector3 _startPosition = _trail.transform.position;
        float _distance = Vector3.Distance(_trail.transform.position, _hitPoint);
        float _remainingDistance = _distance;

        while (_remainingDistance > 0)
        {
            _trail.transform.position = Vector3.Lerp(_startPosition, _hitPoint, 1 - (_remainingDistance / _distance));

            _remainingDistance -= _bulletSpeed * Time.deltaTime;

            yield return null;
        }

        _animator.SetBool("Fire", false);

        _trail.transform.position = _hitPoint;

        Destroy(_trail.gameObject, _trail.time);
    }

    public void Reload()
    {
        if (!_isReloading && _bulletsInMagazine != _magazineCapacity)
            StartCoroutine(AnimateReload());
    }

    IEnumerator AnimateReload()
    {
        _isReloading = true;
        yield return new WaitForSeconds(0.2f);

        float _reloadSpeed = 1f / _reloadTime;
        float _percent = 0;
        Vector3 _initialRot = transform.localEulerAngles;
        float _maxReloadAngle = 20;

        while (_percent < 1)
        {
            _percent += Time.deltaTime * _reloadSpeed;
            float interpolation = (-Mathf.Pow(_percent, 2) + _percent) * 4;
            float reloadAngle = Mathf.Lerp(0, _maxReloadAngle, interpolation);
            transform.localEulerAngles = _initialRot + Vector3.up * reloadAngle;

            yield return null;
        }

        _isReloading = false;
        _bulletsInMagazine = _magazineCapacity;
    }

    public void Aim(Vector3 _aimPoint)
    {
        if (!_isReloading)
            transform.LookAt(_aimPoint);
    }

    public void OnTriggerHold()
    {
        Shoot();
        _triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        _triggerReleasedSinceLastShot = true;
    }
}