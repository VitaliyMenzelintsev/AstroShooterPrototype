using UnityEngine.AI;
using UnityEngine;

public class EnemyDroneBehavior : EnemyBaseBehavior
{
    private NavMeshAgent _navMeshAgent;


    public override void Start()
    {
        base.Start();

        _navMeshAgent = GetComponent<NavMeshAgent>();
    }


    private void FixedUpdate()
    {
        if (MyVitals.IsAlive())
        {
            if (IsTargetAlive() 
                && IsTargetNeedHeal())
            {
                if (IsDistanceCorrect())
                {
                    StateCombat();
                }
                else
                {
                    StateInvestigate();
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
        if (_navMeshAgent != null
        && _navMeshAgent.speed != 0)
            _navMeshAgent.speed = 0;

        Destroy(GetComponent<CapsuleCollider>());

        Destroy(gameObject, 10f);
    }


    private void StateIdle()
    {

    }


    private bool IsTargetNeedHeal()
    {
        if (CurrentTarget.GetComponent<Vitals>().IsNeedHealing())
        {
            return true;
        }

        return false;
    }

    private void StateInvestigate()
    {
        _navMeshAgent.SetDestination(CurrentTarget.transform.position);
    }


    private void StateCombat()
    {
        transform.LookAt(CurrentTarget.transform); // смотрим на цель

        //_currentGun.Aim(_currentTarget.Eyes.position);

        _currentGun.Shoot(CurrentTarget.GetComponent<BaseCharacter>().GetEyesPosition().position);
    }

    public override void StateSkill(bool _isESkill, GameObject _target)
    {
        throw new System.NotImplementedException();
    }
}
