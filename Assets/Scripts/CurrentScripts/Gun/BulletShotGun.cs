using System.Collections;
using UnityEngine;

public class BulletShotGun : BulletGun
{
    [SerializeField]
    private Transform[] _barrelPoints;
    private Vector3 _shootPoint;



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

            _shootPoint = _aimPoint;

            Invoke("DamageDeal", _shootDelay);
        }
    }

    private void DamageDeal()
    {
        for (int i = 0; i < _barrelPoints.Length; i++)
        {
            Vector3 _direction;
            _direction = _barrelPoints[i].forward += new Vector3(
            Random.Range(-_bulletSpreadVariance.x, _bulletSpreadVariance.x),
            Random.Range(-_bulletSpreadVariance.y, _bulletSpreadVariance.y),
            Random.Range(-_bulletSpreadVariance.z, _bulletSpreadVariance.z));

            _direction.Normalize();

            Ray _ray = new Ray(_barrelPoints[i].position, _direction);

            RaycastHit _hit;

            if (Physics.Raycast(_ray, out _hit, float.MaxValue))   // если попали во что-то
            {
                _shootingParticle.Play();

                ShootRender(_hit.point);

                _lastShootTime = Time.time;

                if (_hit.collider != null
                    && _hit.collider.GetComponentInParent<Vitals>()
                    && _hit.collider.GetComponent<Team>().GetTeamNumber() != _myOwnerTeamNumber)
                    _hit.collider.GetComponentInParent<Vitals>().GetHit(_damage);
            }
            else
            {
                _shootingParticle.Play();

                ShootRender(_shootPoint);

                _lastShootTime = Time.time;
            }
        }

        Recoil();
    }
}
