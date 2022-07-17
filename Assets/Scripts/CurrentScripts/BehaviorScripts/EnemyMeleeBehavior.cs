using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]
[RequireComponent(typeof(Animator))]

public class EnemyMeleeBehavior : EnemyBaseBehavior
{
    private NavMeshAgent _navMeshAgent;
    private Animator _characterAnimator;


    private void Start()
    {
        MyTeam = GetComponent<Team>();

        MyVitals = GetComponent<Vitals>();

        _allCharacters = GameObject.FindObjectsOfType<Team>();

        _navMeshAgent = GetComponent<NavMeshAgent>();

        _characterAnimator = GetComponent<Animator>();
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
        _characterAnimator.SetBool("Move", false);

        _characterAnimator.SetBool("Dead", true);

        Destroy(GetComponent<CapsuleCollider>());

        Destroy(GetComponent<NavMeshAgent>());

        Destroy(gameObject, 10f);
    }


    private void StateIdle()
    {
        _characterAnimator.SetBool("Move", false);

        _characterAnimator.SetBool("HasEnemy", false);
    }


    private void StateCombat()
    {
        _characterAnimator.SetBool("Move", false);

        _characterAnimator.SetTrigger("Fire");

        transform.LookAt(_currentTarget.transform);

        _currentGun.Shoot(_currentTarget.Eyes.position);
    }


    private void StateInvestigate()
    {
        _characterAnimator.SetBool("HasEnemy", true);

        _characterAnimator.SetBool("Move", true);

        _navMeshAgent.SetDestination(_currentTarget.transform.position); // действие Investigate
    }
}
