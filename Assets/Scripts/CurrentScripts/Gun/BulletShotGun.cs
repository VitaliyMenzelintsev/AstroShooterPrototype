using System.Collections;
using UnityEngine;

public class BulletShotGun : BulletGun
{
    [SerializeField]
    private Transform[] _barrelPoints;


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

                    if (_hit.collider != null
                        && _hit.collider.TryGetComponent(out IDamageable _damageableObject))
                        _damageableObject.GetHit(_damage);

                    if (_hit.collider.TryGetComponent(out ITeamable _targetableObject)
                        && _targetableObject != null)
                        CurrentTarget = _hit.collider.gameObject;
                }
                else
                {
                    ShootRender(_aimPoint);

                    _lastShootTime = Time.time;
                }
            }
        }
    }
}
