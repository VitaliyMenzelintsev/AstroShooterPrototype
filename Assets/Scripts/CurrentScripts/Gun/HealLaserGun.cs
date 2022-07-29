using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealLaserGun : LaserGun
{
    [SerializeField]
    private LineRenderer _healLineRenderer;
    [SerializeField]
    private ParticleSystem _laserEnding;
    private float _maxDistance;
    private Vitals _myOwnerVitals;
    private bool _isMyOwnerAlive;


    public override void Start()
    {
        base.Start();

        _healLineRenderer.enabled = false;

        _maxDistance = gameObject.GetComponentInParent<BaseCharacter>().GetMaxAttackDistance() - 0.5f;

        _myOwnerVitals = GetComponentInParent<Vitals>();
    }


    public override void LateUpdate()
    {
        base.LateUpdate();

        _isMyOwnerAlive = _myOwnerVitals.IsAlive();
    }

    public override void Shoot(Vector3 _aimPoint)
    {
        _bulletsInMagazine--;

        _shootingParticle.Play();

        Vector3 _direction = _aimPoint - _barrelOrigin.position;

        Ray _ray = new Ray(_barrelOrigin.position, _direction);

        RaycastHit _hit;

        _healLineRenderer.SetPosition(0, _barrelOrigin.position);

        if (Physics.Raycast(_ray, out _hit, _maxDistance))
        {
            _healLineRenderer.SetPosition(1, _hit.point);

            Instantiate(_laserEnding, _hit.point, Quaternion.identity);

            if (_hit.collider.GetComponentInParent<Vitals>().IsAlive()
                && _hit.collider.GetComponentInParent<Team>().GetTeamNumber() != _myOwnerTeamNumber)
            {
                _healLineRenderer.enabled = true;

                _hit.collider.GetComponentInParent<Vitals>().GetHit(_damage);
            }
        }
    }
}
