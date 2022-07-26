using UnityEngine;

[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]

public class EnemyTurretBehavior : EnemyBaseBehavior
{
    [SerializeField]
    private Transform _partToRotate;                                             // определили поворачивающуюся деталь
    private float _turnSpeed = 5f;                                               // скорость поворота башни

    public override void Start()
    {
        base.Start();
    }


    private void FixedUpdate()
    {
        if (MyVitals.IsAlive())
        {
            if (IsTargetAlive())
                {
                if (IsDistanceCorrect())
                {
                    StateRangeCombat();
                }
                else
                {
                    StateIdle();
                }
            }
            else
            {
                GetNewTarget();

                StateIdle();
            }
        }
        else
        {
            StateDeath();
        }
    }

    private void StateDeath()
    {
        Destroy(this, 3f);
    }

    private void StateIdle()
    {
       
    }

    private void StateRangeCombat()
    {

        LockOnTarget();

        _currentGun.Shoot(CurrentTarget.GetComponent<BaseCharacter>().GetEyesPosition().position);

    }



    private void LockOnTarget()
    {
        Vector3 _direction = CurrentTarget.transform.position - transform.position;

        Quaternion _lookRotation = Quaternion.LookRotation(_direction);

        Vector3 _rotation = Quaternion.Lerp(_partToRotate.rotation, _lookRotation, Time.deltaTime * _turnSpeed).eulerAngles;

        _partToRotate.rotation = Quaternion.Euler(0f, _rotation.y, 0f);             // задаём вращение верхушке башни (X и Z freeze)
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _maxAttackDistance);
    }

    public override void StateSkill(bool _isESkill, GameObject _target)
    {
        throw new System.NotImplementedException();
    }
}
