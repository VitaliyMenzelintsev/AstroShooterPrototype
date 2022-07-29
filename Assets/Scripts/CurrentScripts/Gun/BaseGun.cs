using System.Collections;
using UnityEngine;

public abstract class BaseGun : MonoBehaviour
{
    [SerializeField]
    protected Transform _barrelOrigin;
    [SerializeField]
    protected int _magazineCapacity;
    [SerializeField]
    protected float _reloadTime = 1.2f;
    [SerializeField]
    protected float _msBetweenShots = 100;
    [SerializeField]
    protected float _damage = 15;
    [SerializeField]
    protected float _shootDelay = 0.05f;
    [SerializeField]
    protected ParticleSystem _shootingParticle;
    [SerializeField]
    protected bool _isRangeGun = false;
    [SerializeField]
    protected bool _addBulletSpread = false;
    [SerializeField]
    protected Vector3 _bulletSpreadVariance = new(0.05f, 0.05f, 0.05f);
    [SerializeField]
    protected float _distance = 55f;
    protected int _myOwnerTeamNumber;

    protected float _lastShootTime = 0;
    protected bool _isReloading;
    protected float _nextShotTime;
    protected int _bulletsInMagazine;

    [HideInInspector]
    public GameObject CurrentTarget = null;

    public virtual void Start()
    {
        _bulletsInMagazine = _magazineCapacity;
        _myOwnerTeamNumber = GetComponentInParent<Team>().GetTeamNumber();
    }


    public virtual void LateUpdate()
    {
        if (!_isReloading && _bulletsInMagazine == 0)
            Reload();
    }


    public int BulletsInMagazine()
    {
        return _bulletsInMagazine;
    }


    public void Reload()
    {
        if (!_isReloading && _bulletsInMagazine != _magazineCapacity)
            StartCoroutine(AnimateReload());
    }


    public void Aim(Vector3 _aimPoint)
    {
        Mathf.Clamp(_aimPoint.y, -45, 45);
        transform.LookAt(_aimPoint);
    }


    public virtual Vector3 GetDirection()
    {
        Vector3 _direction = transform.forward;

        if (_addBulletSpread) 
        {
            _direction += new Vector3(
                Random.Range(-_bulletSpreadVariance.x, _bulletSpreadVariance.x),
                Random.Range(-_bulletSpreadVariance.y, _bulletSpreadVariance.y),
                Random.Range(-_bulletSpreadVariance.z, _bulletSpreadVariance.z));

            _direction.Normalize();
        }
        return _direction;
    }


    public abstract void ShootRender(Vector3 _aimPoint);


    public abstract void Punch();


    public virtual void Shoot(Vector3 _aimPoint)
    {
        if (IsGunReady())
        {
            _bulletsInMagazine--;

            _nextShotTime = Time.time + _msBetweenShots / 1000;

            _shootingParticle.Play();

            Vector3 _direction = GetDirection(); // определяем направление стрельбы

            Ray _ray = new(_barrelOrigin.position, _direction);

            RaycastHit _hit;

            if (Physics.Raycast(_ray, out _hit, _distance))   // если попали во что-то
            {
                ShootRender(_hit.point);

                _lastShootTime = Time.time;

                if (_hit.collider != null
                   && _hit.collider.GetComponentInParent<Vitals>()
                   && _hit.collider.GetComponent<Team>().GetTeamNumber() != _myOwnerTeamNumber)
                    _hit.collider.GetComponentInParent<Vitals>().GetHit(_damage);
            }
            else
            {
                ShootRender(_aimPoint);

                _lastShootTime = Time.time;
            }

        }
    }


    public bool IsGunReady()
    {
        if (!_isReloading
            && Time.time > _nextShotTime
            && _bulletsInMagazine > 0
            && _lastShootTime + _shootDelay < Time.time)
        {
            return true;
        }
        else return false;
    }


    private IEnumerator AnimateReload()
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
            float _reloadAngle = Mathf.Lerp(0, _maxReloadAngle, interpolation);
            transform.localEulerAngles = _initialRot + Vector3.down * _reloadAngle;

            yield return null;
        }

        _isReloading = false;
        _bulletsInMagazine = _magazineCapacity;
    }
}
