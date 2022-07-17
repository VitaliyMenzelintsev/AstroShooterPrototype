using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]

public class EnemyDroneBehavior : EnemyBehavior
{
    private NavMeshAgent _navMeshAgent;


    private void Start()
    {
        MyTeam = GetComponent<Team>();

        MyVitals = GetComponent<Vitals>();

        _allCharacters = GameObject.FindObjectsOfType<Team>();

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
                _currentTarget = GetNewTarget();

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
        _navMeshAgent.SetDestination(_currentTarget.transform.position);
    }


    private void StateCombat()
    {
        transform.LookAt(_currentTarget.transform); // смотрим на цель

        //_currentGun.Aim(_currentTarget.Eyes.position);

        _currentGun.Shoot(_currentTarget.Eyes.position);
    }
}
