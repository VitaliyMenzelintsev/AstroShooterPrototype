using UnityEngine;

public class EnemyTurretBehavior : EnemyBaseBehavior
{
    [SerializeField]
    private Transform _partToRotate;                                          
    private readonly float _turnSpeed = 5f;                                        


    public override void Start()  { base.Start(); }


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


    public override void StateSkill(bool _isESkill, GameObject _target) { }


    private void StateDeath()
    {
        Destroy(this, 3f);
    }


    private void StateIdle() { }



    private void StateRangeCombat()
    {
        LockOnTarget();

        _currentGun.Shoot(CurrentTarget.GetComponent<BaseCharacter>().GetHeadTransform().position);
    }



    private void LockOnTarget()
    {
        Vector3 _direction = CurrentTarget.transform.position - transform.position;

        Quaternion _lookRotation = Quaternion.LookRotation(_direction);

        Vector3 _rotation = Quaternion.Lerp(_partToRotate.rotation, _lookRotation, Time.deltaTime * _turnSpeed).eulerAngles;

        _partToRotate.rotation = Quaternion.Euler(0f, _rotation.y, 0f);    
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _maxAttackDistance);
    }
}
