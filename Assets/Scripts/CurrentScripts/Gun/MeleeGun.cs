using UnityEngine;

public class MeleeGun : BaseGun
{
    public override void Start()
    {
        base.Start();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    public override void Shoot(Vector3 _aimPoint)
    {
        if (IsGunReady())
        {
            _nextShotTime = Time.time + _msBetweenShots / 1000;

            Invoke("DamageDeal", _shootDelay); 
        }
    }


    private void DamageDeal()
    {
        Vector3 _direction = GetDirection(); // определяем направление стрельбы

        Ray _ray = new Ray(_barrelOrigin.position, _direction);

        RaycastHit _hit;

        if (Physics.Raycast(_ray, out _hit, _distance))   // если попали во что-то
        {
            _lastShootTime = Time.time;

            if (_hit.collider != null
                && _hit.collider.TryGetComponent(out IDamageable _damageableObject))
                _damageableObject.GetHit(_damage);
        }
        else
        {
            _lastShootTime = Time.time;
        }
    }

    public override Vector3 GetDirection()
    {
        Vector3 _direction = transform.forward;

        return _direction;
    }

    public override void ShootRender(Vector3 _aimPoint) { }

    public override void Punch() { }
}
