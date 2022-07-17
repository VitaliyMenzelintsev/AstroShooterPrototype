using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]
[RequireComponent(typeof(Animator))]

public class CompanionMeleeBehavior : CompanionBaseBehavior
{
    [HideInInspector]
    public Team MyTeam;
    [HideInInspector]
    public Vitals MyVitals;
    public Transform Eyes;

    [SerializeField]
    private Transform _followPoint;
    [SerializeField]
    private Transform _player;
    private NavMeshAgent _navMeshAgent;
    private Animator _characterAnimator;

    [SerializeField]
    private BaseGun _currentGun;
    [SerializeField]
    private float _minAttackDistance = 0.5f;
    [SerializeField]
    private float _maxAttackDistance = 1.5f;
    private Team[] _allCharacters;


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
                CurrentTarget = GetNewTarget();

                if (IsTargetAlive())
                {
                    StateIdle();
                }
                else
                {
                    if (IsPlayerFar())
                    {
                        StateFollowThePlayer();
                    }
                    else
                    {
                        StateIdle();
                    }
                }
            }
        }
        else
        {
            StateDeath();
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        GetNewTarget();

        if (CurrentTarget == null)
        {
            MyVitals.GetRessurect();

            StateIdle();

            _characterAnimator.SetBool("Dead", false);

            _characterAnimator.SetBool("HasEnemy", false);
        }
    }



    private void StateDeath()
    {
        _characterAnimator.SetBool("Move", false);

        _characterAnimator.SetBool("Dead", true);
    }



    private void StateIdle()
    {
        _characterAnimator.SetBool("HasEnemy", false);

        _characterAnimator.SetBool("Move", false);

        _characterAnimator.SetBool("HasEnemy", false);
    }



    private void StateFollowThePlayer()
    {
        _characterAnimator.SetBool("Move", true);

        _characterAnimator.SetBool("HasEnemy", false);

        transform.LookAt(_player);

        _navMeshAgent.SetDestination(_followPoint.position);            // действие Follow The Player
    }



    private void StateInvestigate()
    {
        _characterAnimator.SetBool("HasEnemy", true);

        _characterAnimator.SetBool("Move", true);

        _navMeshAgent.SetDestination(CurrentTarget.transform.position); // действие Investigate
    }


     
    private void StateCombat()
    {
        _characterAnimator.SetBool("Move", false);

        _characterAnimator.SetTrigger("Fire");

        transform.LookAt(CurrentTarget.transform);

        _currentGun.Shoot(CurrentTarget.Eyes.position);
    }



    private Team GetNewTarget()
    {
        Team _bestTarget = null;

        for (int i = 0; i < _allCharacters.Length; i++)
        {
            Team _currentCharacter = _allCharacters[i];

            //выбирать текущего персонажа в качестве цели, только если мы не в одной команде и если у него осталось здоровье
            if (_currentCharacter != null
                && _currentCharacter.GetComponent<Team>().GetTeamNumber() != MyTeam.GetTeamNumber()
                && _currentCharacter.GetComponent<Vitals>().IsAlive()
                && Vector3.Distance(transform.position, _currentCharacter.transform.position) <= _maxAttackDistance)
            {
                //если цель видно
                if (CanSeeTarget(_currentCharacter))
                {
                    if (_bestTarget == null)
                    {
                        _bestTarget = _currentCharacter;
                    }
                    else
                    {
                        //если текущая цель ближе, чем лучшая цель, то выбрать текущую цель 
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

        Vector3 _enemyPosition = _target.Eyes.position;

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



    private bool IsTargetAlive()
    {
        if (CurrentTarget != null
            && CurrentTarget.GetComponent<Vitals>().IsAlive())
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    private bool IsDistanceCorrect()
    {
        if (Vector3.Distance(transform.position, CurrentTarget.transform.position) <= _maxAttackDistance
                && Vector3.Distance(transform.position, CurrentTarget.transform.position) >= _minAttackDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    private bool IsPlayerFar()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) > 3f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
