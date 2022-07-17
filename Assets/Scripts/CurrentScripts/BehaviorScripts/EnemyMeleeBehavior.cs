using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]
[RequireComponent(typeof(Animator))]

public class EnemyMeleeBehavior : EnemyBehavior
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


    private Team GetNewTarget()
    {
        Team _bestTarget = null;

        for (int i = 0; i < _allCharacters.Length; i++)
        {
            Team _currentCharacter = _allCharacters[i];

            //выбирать текущего противника в качестве цели, только если мы не в одной команде и если у него осталось здоровье
            if (_currentCharacter.GetComponent<Team>().GetTeamNumber() != MyTeam.GetTeamNumber()
                && _currentCharacter.GetComponent<Vitals>().IsAlive())
            {
                //если рейкаст попал в цель, мы знаем, что видим её
                if (CanSeeTarget(_currentCharacter))
                {
                    if (_bestTarget == null)
                    {
                        _bestTarget = _currentCharacter;
                    }
                    else
                    {
                        //если текущий враг ближе, чем лучшая цель, то выбрать текущего врага в качестве лучшей цели
                        if (Vector3.Distance(_currentCharacter.transform.position, transform.position) < Vector3.Distance(_bestTarget.transform.position, transform.position))
                        {
                            _bestTarget = _currentCharacter;
                        }
                    }
                }
            }
        }

        return _bestTarget;
    }


    private bool CanSeeTarget(Team _target)
    {
        bool _canSeeIt = false;

        Vector3 _enemyPosition = _target.Eyes.position; ;    // _target.Eyes.position; убрал про глаза

        Vector3 _directionTowardsEnemy = _enemyPosition - Eyes.position;

        RaycastHit _hit;

        //направить луч на текущего врага
        if (Physics.Raycast(Eyes.position, _directionTowardsEnemy, out _hit, Mathf.Infinity))
        {
            //если рейкаст попал в цель, то мы знаем, что можем его увидеть
            if (_hit.transform == _target.transform)
            {
                _canSeeIt = true;
            }
        }

        return _canSeeIt;
    }
}
