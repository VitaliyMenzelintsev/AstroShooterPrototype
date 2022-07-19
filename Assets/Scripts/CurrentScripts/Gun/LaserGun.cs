using UnityEngine;

public class LaserGun : BaseGun
{
    [Header("Gun Settings")]
    private float _punchDamage;
    [SerializeField]
    private LineRenderer _lineRenderer;


    public override void Start()
    {
        base.Start();

        _lineRenderer = GetComponent<LineRenderer>();

        _lineRenderer.enabled = false;

        _punchDamage = _damage / 2;
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    public override void Shoot(Vector3 _aimPoint)
    {
        if (IsGunReady())
        {
            _bulletsInMagazine--;

            _nextShotTime = Time.time + _msBetweenShots / 1000;

            _shootingParticle.Play();

            Vector3 _direction = GetDirection(); // определяем направление стрельбы

            Ray _ray = new Ray(_barrelOrigin.position, _direction);

            RaycastHit _hit;


            if (Physics.Raycast(_ray, out _hit, _distance))   // если попали во что-то
            {
                _lastShootTime = Time.time;

                if (_hit.collider != null
                    && _hit.collider.TryGetComponent(out IDamageable _damageableObject))
                {
                    _damageableObject.GetHit(_damage);

                    _lineRenderer.enabled = true;

                    ShootRender(_hit.point);
                }
            }
            else
            {
                //_lineRenderer.enabled = false;

                ShootRender(_aimPoint);

                _lastShootTime = Time.time;
            }

        }
        else
        {
            _lineRenderer.enabled = false;
        }
    }


    public override void Punch()
    {
        if (Time.time > _nextShotTime)
        {
            _nextShotTime = Time.time + _msBetweenShots / 1000;

            if (_lastShootTime + _shootDelay < Time.time)
            {
                Vector3 _direction = GetDirection(); // определяем направление удара

                if (Physics.Raycast(_barrelOrigin.position, _direction, out RaycastHit _hit, float.MaxValue))   // если попали 
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

    public override void ShootRender(Vector3 _aimPoint)
    {
        _lineRenderer.SetPosition(0, _barrelOrigin.position);
        _lineRenderer.SetPosition(1, _aimPoint);
    }
}
