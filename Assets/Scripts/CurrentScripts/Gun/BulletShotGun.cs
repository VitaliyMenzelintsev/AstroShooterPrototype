using System.Collections;
using UnityEngine;

public class BulletShotGun : BaseGun
{
    [Header("Gun Settings")]
    private float _punchDamage;
    [SerializeField]
    private Transform[] _barrelPoints;
    [SerializeField]
    private float _bulletSpeed = 200;
    [SerializeField]
    private bool _addBulletSpread = false;
    [SerializeField]
    private Vector3 _bulletSpreadVariance = new Vector3(0.05f, 0.05f, 0.05f);
    [SerializeField]
    private TrailRenderer _bulletTrail;

    [Header("Recoil")]
    [SerializeField]
    private Vector2 _kickMinMax = new Vector2(0.05f, 0.2f);
    [SerializeField]
    private Vector2 _recoilAngleMinMax = new Vector2(5, 8);
    private float _recoilBackTime = 0.1f;
    private Vector3 _recoilSmoothDampVelocity;
    private float _recoilRotSmoothDampVelocity;
    private float _recoilAngle;
    private Vector3 _gunOriginPosition;


    public override void Start()
    {
        base.Start();

        _punchDamage = _damage / 2;
        _gunOriginPosition = transform.localPosition;
    }


    public override void LateUpdate()
    {
        base.LateUpdate();

        // ¬озвращение оружи€ в нормальное положение после отдачи
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, _gunOriginPosition, ref _recoilSmoothDampVelocity, _recoilBackTime);
        _recoilAngle = Mathf.SmoothDamp(_recoilAngle, 0, ref _recoilRotSmoothDampVelocity, _recoilBackTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * _recoilAngle;
    }


    public override void Shoot(Vector3 _aimPoint)
    {
        if (IsGunReady())
        {
            _bulletsInMagazine--;

            _nextShotTime = Time.time + _msBetweenShots / 1000;

            _shootingParticle.Play();

            Recoil();

            for (int i = 0; i < _barrelPoints.Length; i++)
            {
                Vector3 _direction = GetDirection(); // определ€ем направление стрельбы

                Ray _ray = new Ray(_barrelPoints[i].position, _direction);

                RaycastHit _hit;

                if (Physics.Raycast(_ray, out _hit, float.MaxValue))   // если попали во что-то
                {
                    ShootRender(_hit.point);

                    _lastShootTime = Time.time;

                    if (_hit.collider.gameObject.GetComponent<Vitals>())
                        _hit.collider.gameObject.GetComponent<Vitals>().GetHit(_damage);
                }
                else
                {
                    ShootRender(_aimPoint);

                    _lastShootTime = Time.time;
                }
            }
        }
    }


    public override void Punch()
    {
        if (Time.time > _nextShotTime)
        {
            _nextShotTime = Time.time + _msBetweenShots / 1000;

            if (_lastShootTime + _shootDelay < Time.time)
            {
                Vector3 _direction = GetDirection(); // определ€ем направление удара

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


    public override Vector3 GetDirection()
    {
        Vector3 _direction = _barrelPoint.forward;

        if (_addBulletSpread) // если делаем разброс, то он задаЄтс€ путЄм рандомизации координат вектора направлени€
        {
            _direction += new Vector3(
                Random.Range(-_bulletSpreadVariance.x, _bulletSpreadVariance.x),
                Random.Range(-_bulletSpreadVariance.y, _bulletSpreadVariance.y),
                Random.Range(-_bulletSpreadVariance.z, _bulletSpreadVariance.z));

            _direction.Normalize();
        }
        return _direction;
    }


    public override void ShootRender(Vector3 _aimPoint)
    {
        TrailRenderer _trail = Instantiate(_bulletTrail, _barrelPoint.position, Quaternion.identity);

        StartCoroutine(SpawnTrail(_trail, _aimPoint));
    }


    private void Recoil()
    {
        transform.localPosition -= Vector3.forward * Random.Range(_kickMinMax.x, _kickMinMax.y);
        _recoilAngle += Random.Range(_recoilAngleMinMax.x, _recoilAngleMinMax.y);
        _recoilAngle = Mathf.Clamp(_recoilAngle, 0, 25);
    }


    private IEnumerator SpawnTrail(TrailRenderer _trail, Vector3 _hitPoint)
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
}
