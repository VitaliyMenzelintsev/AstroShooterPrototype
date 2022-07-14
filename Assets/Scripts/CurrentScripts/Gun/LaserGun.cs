using UnityEngine;

public class LaserGun : BaseGun
{
    [Header("Gun Settings")]
    private float _punchDamage;
    [SerializeField]
    private bool _addBulletSpread = false;
    [SerializeField]
    private Vector3 _bulletSpreadVariance = new Vector3(0.05f, 0.05f, 0.05f);
    [SerializeField]
    private LineRenderer _lineRenderer;

     
    public override void Start()
    {
        base.Start();

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

            Vector3 _direction = GetDirection(); // определ€ем направление стрельбы

            Ray _ray = new Ray(_barrelPoint.position, _direction);

            RaycastHit _hit;

            if (Physics.Raycast(_ray, out _hit, float.MaxValue))   // если попали во что-то
            {
                ShootRender(_hit.point);

                Debug.Log("Ћечу");

                _lastShootTime = Time.time;

                if (_hit.collider.gameObject.GetComponent<Vitals>())
                    _hit.collider.gameObject.GetComponent<Vitals>().GetHit(_damage);
            }
            else
            {
                ShootRender(_aimPoint);

                Debug.Log("не лечу");

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
        _lineRenderer.SetPosition(0, _barrelPoint.position);
        _lineRenderer.SetPosition(1, _aimPoint);
    }
}
