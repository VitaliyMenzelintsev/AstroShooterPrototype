using System.Collections;
using UnityEngine;

public class BulletGun : BaseGun
{
    [Header("Gun Settings")]
    
    public float _punchDamage = 30;
    [SerializeField]
    protected float _bulletSpeed = 200;
    [SerializeField]
    protected TrailRenderer _bulletTrail;

    [Header("Recoil")]
    [SerializeField]
    protected Vector2 _kickMinMax = new(0.05f, 0.2f);
    [SerializeField]
    protected Vector2 _recoilAngleMinMax = new(5, 8);
    protected Vector3 _gunOriginPosition;
    protected Vector3 _recoilSmoothDampVelocity;
    protected float _recoilBackTime = 0.1f;
    protected float _recoilRotSmoothDampVelocity;
    protected float _recoilAngle;
    

    public override void Start()
    {
        base.Start();

        _gunOriginPosition = transform.localPosition;
    }


    public override void LateUpdate()
    {
        base.LateUpdate();

        // Возвращение оружия в начальное положение после отдачи
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

            Vector3 _direction = GetDirection();

            if (Physics.SphereCast(_barrelOrigin.position, 0.15f, _direction, out RaycastHit _hit, _distance))
            {
                ShootRender(_hit.point);

                _lastShootTime = Time.time;

                if (_hit.transform.gameObject.GetComponentInParent<Vitals>()
                    && _hit.transform.gameObject.GetComponentInParent<Team>().GetTeamNumber() != _myOwnerTeamNumber)
                    _hit.transform.gameObject.GetComponentInParent<Vitals>().GetHit(_damage);
            }
            else
            {
                ShootRender(_aimPoint);

                _lastShootTime = Time.time;
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
                Vector3 _direction = transform.forward;

                if (Physics.Raycast(_barrelOrigin.position, _direction, out RaycastHit _hit, float.MaxValue))  
                {
                    _lastShootTime = Time.time;

                    _hit.collider.GetComponent<Vitals>().GetHit(_punchDamage);
                }
                else
                {
                    _lastShootTime = Time.time;
                }
            }
        }
    }


    public override void ShootRender(Vector3 _aimPoint)
    {
        TrailRenderer _trail = Instantiate(_bulletTrail, _barrelOrigin.transform.position, Quaternion.identity);

        StartCoroutine(SpawnTrail(_trail, _aimPoint));
    }


    public void Recoil()
    {
        transform.localPosition -= -Vector3.back * Random.Range(_kickMinMax.x, _kickMinMax.y);
        _recoilAngle += Random.Range(_recoilAngleMinMax.x, _recoilAngleMinMax.y);
        _recoilAngle = Mathf.Clamp(_recoilAngle, 0, 25);
    }


    public IEnumerator SpawnTrail(TrailRenderer _trail, Vector3 _hitPoint)
    {
        Vector3 _startPosition = _barrelOrigin.transform.position;
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