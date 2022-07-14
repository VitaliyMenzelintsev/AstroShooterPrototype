using System.Collections;
using UnityEngine;

public abstract class BaseGun : MonoBehaviour
{ 
    [SerializeField]
    protected Transform _barrelPoint;
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


    protected float _lastShootTime = 0;
    protected bool _isReloading;
    protected float _nextShotTime;
    protected int _bulletsInMagazine;

    public virtual void Start()
    {
        _bulletsInMagazine = _magazineCapacity;
    }

    public virtual void LateUpdate()
    {
        if (!_isReloading && _bulletsInMagazine == 0)
            Reload();
    }

    public void Reload()
    {
        if (!_isReloading && _bulletsInMagazine != _magazineCapacity)
            StartCoroutine(AnimateReload());
    }


    public void Aim(Vector3 _aimPoint)
    {
        transform.LookAt(_aimPoint);
    }

    public abstract Vector3 GetDirection();

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

            Ray _ray = new Ray(_barrelPoint.position, _direction);

            RaycastHit _hit;

            if (Physics.Raycast(_ray, out _hit, float.MaxValue))   // если попали во что-то
            {
                _lastShootTime = Time.time;

                ShootRender(_hit.point);

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

    public bool IsGunReady()
    {
        if (!_isReloading && Time.time > _nextShotTime
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
