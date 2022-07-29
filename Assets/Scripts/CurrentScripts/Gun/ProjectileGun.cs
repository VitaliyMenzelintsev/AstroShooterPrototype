using UnityEngine;

public class ProjectileGun : BaseGun // 
{
    [SerializeField]
    private GameObject _projectilePrefab;

    protected float _punchDamage;
    [Header("Recoil")]
    [SerializeField]
    protected Vector2 _kickMinMax = new Vector2(0.05f, 0.2f);
    [SerializeField]
    protected Vector2 _recoilAngleMinMax = new Vector2(5, 8);
    protected float _recoilBackTime = 0.1f;
    protected Vector3 _recoilSmoothDampVelocity;
    protected float _recoilRotSmoothDampVelocity;
    protected float _recoilAngle;
    protected Vector3 _gunOriginPosition;

    public override void Start()
    {
        base.Start();

        _punchDamage = _damage / 2;
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

            Ray _ray = new Ray(_barrelOrigin.position, _direction);

            RaycastHit _hit;

            if (Physics.Raycast(_ray, out _hit, _distance))   
            {
                Quaternion fireRotation = Quaternion.LookRotation(transform.forward); // возможно лучше Quaternion identity в instantiate

                GameObject _bullet = Instantiate(_projectilePrefab, _barrelOrigin.position, fireRotation);

                _bullet.GetComponent<Projectile>()._aimPoint = _aimPoint;
            }
            else
            {
              _lastShootTime = Time.time;
            }
        }
    }



    public void Recoil()
    {
        transform.localPosition -= Vector3.forward * Random.Range(_kickMinMax.x, _kickMinMax.y);
        _recoilAngle += Random.Range(_recoilAngleMinMax.x, _recoilAngleMinMax.y);
        _recoilAngle = Mathf.Clamp(_recoilAngle, 0, 25);
    }



    public override void Punch()
    { }



    public override void ShootRender(Vector3 _aimPoint)
    { }
}
