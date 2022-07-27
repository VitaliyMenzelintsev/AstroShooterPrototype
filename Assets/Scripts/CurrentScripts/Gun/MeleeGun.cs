using UnityEngine;

public class MeleeGun : BaseGun
{
    //private Vector3 _target;
    private float _maxAttackDistance;

    public override void Start()
    {
        base.Start();
        _maxAttackDistance = GetComponentInParent<BaseCharacter>().GetMaxAttackDistance();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    public override void Shoot(Vector3 _aimPoint)
    {
        //_target = _aimPoint;

        if (IsGunReady())
        {
            _nextShotTime = Time.time + _msBetweenShots / 1000;

            Invoke("DamageDeal", _shootDelay); 
        }
    }


    private void DamageDeal()
    {
        CurrentTarget = GetComponentInParent<BaseCharacter>().GetMyTarget();

        if(Vector3.Distance(transform.position, CurrentTarget.transform.position) <= _maxAttackDistance)
            CurrentTarget.GetComponent<Vitals>().GetHit(_damage);
    }

    public override Vector3 GetDirection()
    {
        Vector3 _direction = transform.forward;

        return _direction;
    }

    public override void ShootRender(Vector3 _aimPoint) { }

    public override void Punch() { }
}
