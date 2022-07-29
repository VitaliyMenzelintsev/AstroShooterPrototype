using UnityEngine;

public class LaserGun : BaseGun
{
    [Header("Gun Settings")]
    private float _punchDamage;
    [SerializeField]
    private LineRenderer _lineRenderer;
    [SerializeField]
    private ParticleSystem _laserEnding;
    private Vitals _myOwnerVitals;
    private float _maxDistance;
    private bool _isMyOwnerAlive;
    private Vector3 _target;


    public override void Start()
    {
        base.Start();

        _lineRenderer.enabled = false;

        _punchDamage = _damage / 2;

        _maxDistance = gameObject.GetComponentInParent<BaseCharacter>().GetMaxAttackDistance() - 0.5f;

        _myOwnerVitals = GetComponentInParent<Vitals>();
    }



    public override void LateUpdate()
    {
        base.LateUpdate();

        _isMyOwnerAlive = _myOwnerVitals.IsAlive();

        if (!_isMyOwnerAlive)
        {
            _lineRenderer.enabled = false;
        }

        if(_target != null
           && Vector3.Distance(transform.position, _target) > _maxDistance)
        {
            _lineRenderer.enabled = false;
        }
    }

    public override void Shoot(Vector3 _aimPoint)
    {
        _target = _aimPoint;

        if (!IsGunReady())
        {
            _lineRenderer.SetPosition(1, _barrelOrigin.position);
            return;
        }

        if (!IsLaserPossible(_aimPoint))
        {
            _lineRenderer.SetPosition(1, _barrelOrigin.position);
            return;
        }

        _bulletsInMagazine--;

        _shootingParticle.Play();

        Vector3 _direction = GetDirection();

        Ray _ray = new Ray(_barrelOrigin.position, _direction);

        RaycastHit _hit;

        _lineRenderer.SetPosition(0, _barrelOrigin.position);

        if (Physics.Raycast(_ray, out _hit, _maxDistance))
        {
            _lineRenderer.SetPosition(1, _hit.point);

            Instantiate(_laserEnding, _hit.point, Quaternion.identity);

            if (_hit.collider.GetComponentInParent<Vitals>().IsAlive()
                && _hit.collider.GetComponentInParent<Team>().GetTeamNumber() != _myOwnerTeamNumber)
            {
                _lineRenderer.enabled = true;

                _hit.collider.GetComponentInParent<Vitals>().GetHit(_damage);
            }
        }
    }


    public override Vector3 GetDirection()
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



    public override void Punch()
    {
        if (Time.time > _nextShotTime)
        {
            _nextShotTime = Time.time + _msBetweenShots / 1000;

            if (_lastShootTime + _shootDelay < Time.time)
            {
                Vector3 _direction = GetDirection();

                if (Physics.Raycast(_barrelOrigin.position, _direction, out RaycastHit _hit, float.MaxValue))
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


    public override void ShootRender(Vector3 _aimPoint) { }

    private bool IsLaserPossible(Vector3 _aimPoint)
    {
        if (_aimPoint != null
            && (Vector3.Distance(gameObject.transform.position, _aimPoint) <= _maxDistance)
            && _isMyOwnerAlive)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
