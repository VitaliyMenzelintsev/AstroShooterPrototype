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
    private float _reloadTime = 1.2f;
    [SerializeField]
    private float _msBetweenShots = 100;
    [SerializeField]
    private float _damage = 15;
    private float _punchDamage;
    [SerializeField]
    private float _shootDelay = 0.05f;
    [SerializeField]
    private float _bulletSpeed = 200;
    [SerializeField]
    private bool _addBulletSpread = true;
    [SerializeField]
    private Vector3 _bulletSpreadVariance = new Vector3(0.05f, 0.05f, 0.05f);

    [SerializeField]
    private LayerMask _mask;
    [SerializeField]
    private ParticleSystem _shootingParticle;
    [SerializeField]
    private TrailRenderer _bulletTrail;

    private float _lastShootTime = 0;
    private bool _isReloading;
    private float _nextShotTime;
    private bool _triggerReleasedSinceLastShot;
    private int _bulletsInMagazine;

    [Header("Recoil")]
    private Vector2 kickMinMax = new Vector2(0.05f, 2f);
    private Vector2 recoilAngleMinMax = new Vector2(3, 5);
    [SerializeField]
    private float timeOfReturnToPosition = 0.1f;
    private Vector3 recoilSmoothDampVelocity;    
    private float recoilRotSmoothDampVelocity;  
    private float recoilAngle;


    private void Start()
    {
        _bulletsInMagazine = _magazineCapacity;
        _punchDamage = _damage / 2;
    }

    private void LateUpdate()
    {
        // Возвращение оружия в нормальное положение после отдачи
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, timeOfReturnToPosition); 
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, timeOfReturnToPosition);          
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

        if (!_isReloading && _bulletsInMagazine == 0)
            Reload();
    }

    public void Shoot(Vector3 _aimPoint)
    {
        if (!_isReloading && Time.time > _nextShotTime && _bulletsInMagazine > 0)
        {
            _bulletsInMagazine--;
            _nextShotTime = Time.time + _msBetweenShots / 1000;

            if (_lastShootTime + _shootDelay < Time.time)
            {
                _shootingParticle.Play();

                Vector3 _direction = GetDirection(); // определяем направление стрельбы

                if (Physics.Raycast(_bulletSpawnPoint.position, _direction, out RaycastHit _hit, float.MaxValue, _mask))   // если попали во что-то
                {
                    TrailRenderer _trail = Instantiate(_bulletTrail, _bulletSpawnPoint.position, Quaternion.identity);  // делаем след

                    StartCoroutine(SpawnTrail(_trail, _hit.point));

                    _lastShootTime = Time.time;

                    _hit.collider.gameObject.GetComponent<Vitals>().GetHit(_damage);
                }
                else
                {
                    TrailRenderer _trail = Instantiate(_bulletTrail, _bulletSpawnPoint.position, Quaternion.identity);

                    StartCoroutine(SpawnTrail(_trail, _aimPoint));

                    _lastShootTime = Time.time;
                }
            }
        }
        _shootingParticle.Stop();
    }


    public void Punch()
    {
        if (Time.time > _nextShotTime)
        {
            _nextShotTime = Time.time + _msBetweenShots / 1000;

            if (_lastShootTime + _shootDelay < Time.time)
            {
                Vector3 _direction = GetDirection(); // определяем направление удара

                if (Physics.Raycast(_bulletSpawnPoint.position, _direction, out RaycastHit _hit, float.MaxValue, _mask))   // если попали 
                {
                    _lastShootTime = Time.time;

                    _hit.collider.gameObject.GetComponent<Vitals>().GetHit(_punchDamage);
                }
                else
                {
                    _lastShootTime = Time.time;
                }
            }
        }
    }


    private void Recoil()
    {
        transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
        recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
        recoilAngle = Mathf.Clamp(recoilAngle, 0, 25);
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


    private IEnumerator SpawnTrail(TrailRenderer _trail, Vector3 _hitPoint) // почкму не воид
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
        float _maxReloadAngle = 35;

        while (_percent < 1)
        {
            _percent += Time.deltaTime * _reloadSpeed;
            float interpolation = (-Mathf.Pow(_percent, 2) + _percent) * 4;
            float reloadAngle = Mathf.Lerp(0, _maxReloadAngle, interpolation);
            transform.localEulerAngles = _initialRot + Vector3.left * reloadAngle;

            yield return null;
        }

        _isReloading = false;
        _bulletsInMagazine = _magazineCapacity;
    }

    public void Aim(Vector3 _aimPoint)
    {
        transform.LookAt(_aimPoint);
    }

    public void OnTriggerHold(Vector3 _aimPoint)
    {
        Shoot(_aimPoint);

        _triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        _triggerReleasedSinceLastShot = true;
    }
}