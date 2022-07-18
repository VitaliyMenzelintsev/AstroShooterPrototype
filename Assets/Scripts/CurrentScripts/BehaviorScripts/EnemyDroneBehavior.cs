using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]

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
            if (IsTargetAlive())
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
        Destroy(GetComponent<CapsuleCollider>());

        Destroy(GetComponent<NavMeshAgent>());

        Destroy(gameObject, 10f);
    }


    private void StateIdle()
    {

    }


    private void StateInvestigate()
    {
        _navMeshAgent.SetDestination(CurrentTarget.transform.position);
    }


    private void StateCombat()
    {
        transform.LookAt(CurrentTarget.transform); // смотрим на цель

        //_currentGun.Aim(_currentTarget.Eyes.position);

        _currentGun.Shoot(CurrentTarget.GetComponent<AIBaseBehavior>().Eyes.position);
    }

    public override void StateSkill(bool _isESkill, GameObject _target)
    {
        throw new System.NotImplementedException();
    }
}
