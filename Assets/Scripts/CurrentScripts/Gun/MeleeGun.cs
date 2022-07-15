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

            Vector3 _direction = GetDirection(); // определяем направление стрельбы

            Ray _ray = new Ray(_barrelPoint.position, _direction);

            RaycastHit _hit;

            if (Physics.Raycast(_ray, out _hit, float.MaxValue))   // если попали во что-то
            {
                _lastShootTime = Time.time;

                if (_hit.collider.gameObject.GetComponent<Vitals>())
                    _hit.collider.gameObject.GetComponent<Vitals>().GetHit(_damage);
            }
            else
            {
                _lastShootTime = Time.time;
            }
        }
    }

    public override Vector3 GetDirection()
    {
        Vector3 _direction = _barrelPoint.forward;

        return _direction;
    }

    public override void ShootRender(Vector3 _aimPoint) { }

    public override void Punch() { }
}
