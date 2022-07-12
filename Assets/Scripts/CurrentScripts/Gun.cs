using System.Collections;
using UnityEngine;

// Висит на каждом оружии. Характеристики настраиваются индивидуально
public class Gun : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField]
    private bool _isItLaserGun = false;
    [SerializeField]
    private Transform _barrelPoint;
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

    //[SerializeField]
    //private LayerMask _mask;
    [SerializeField]
    private ParticleSystem _shootingParticle;
    [SerializeField]
    private TrailRenderer _bulletTrail;
    [SerializeField]
    private LineRenderer _lineRenderer;

    private float _lastShootTime = 0;
    private bool _isReloading;
    private float _nextShotTime;
    private bool _triggerReleasedSinceLastShot;
    private int _bulletsInMagazine;

    [Header("Recoil")]
    private Vector2 _kickMinMax = new Vector2(0.05f, 0.2f);
    private Vector2 _recoilAngleMinMax = new Vector2(5, 8);
    [SerializeField]
    private float _recoilBackTime = 0.1f;
    private Vector3 _recoilSmoothDampVelocity;    
    private float _recoilRotSmoothDampVelocity;  
    private float _recoilAngle;
    private Vector3 _gunOriginPosition;


    private void Start()
    {
        _bulletsInMagazine = _magazineCapacity;
        _punchDamage = _damage / 2;
        _gunOriginPosition = transform.localPosition;
    }

    private void LateUpdate()
    {
        // Возвращение оружия в нормальное положение после отдачи
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, _gunOriginPosition, ref _recoilSmoothDampVelocity, _recoilBackTime);
        _recoilAngle = Mathf.SmoothDamp(_recoilAngle, 0, ref _recoilRotSmoothDampVelocity, _recoilBackTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * _recoilAngle;

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

                Ray _ray = new Ray(_barrelPoint.position, _direction);

                RaycastHit _hit;

                if (Physics.Raycast(_ray, out _hit, float.MaxValue))   // если попали во что-то
                {
                    if (_isItLaserGun)
                    {
                        LaserRender(_hit.point);
                    }
                    else
                    {
                        Recoil(); 

                        BulletRender(_hit.point);
                    }

                    Debug.Log("hit");

                    _lastShootTime = Time.time;
                    if(_hit.collider.gameObject.GetComponent<Vitals>())
                    _hit.collider.gameObject.GetComponent<Vitals>().GetHit(_damage);
                }
                else
                {
                    if (_isItLaserGun)
                    {
                        LaserRender(_aimPoint);
                    }
                    else
                    {
                        Recoil();

                        BulletRender(_aimPoint);
                    }

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

                if (Physics.Raycast(_barrelPoint.position, _direction, out RaycastHit _hit, float.MaxValue))   // если попали 
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
        transform.localPosition -= Vector3.forward * Random.Range(_kickMinMax.x, _kickMinMax.y);
        _recoilAngle += Random.Range(_recoilAngleMinMax.x, _recoilAngleMinMax.y);
        _recoilAngle = Mathf.Clamp(_recoilAngle, 0, 25);
    }


    private Vector3 GetDirection()
    {
        Vector3 _direction = _barrelPoint.forward;

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


    private void BulletRender(Vector3 _aimPoint)
    {
        TrailRenderer _trail = Instantiate(_bulletTrail, _barrelPoint.position, Quaternion.identity);

        StartCoroutine(SpawnTrail(_trail, _aimPoint));
    }

    private void LaserRender(Vector3 _aimPoint)
    {
        //_lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, _barrelPoint.position);
        _lineRenderer.SetPosition(1, _aimPoint);
    }

    private IEnumerator SpawnTrail(TrailRenderer _trail, Vector3 _hitPoint) // почему не воид
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