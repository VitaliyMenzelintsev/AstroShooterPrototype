using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]
[RequireComponent(typeof(Animator))]

public class CompanionMeleeBehavior : MonoBehaviour
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
    private Transform _myTransform;
    private Animator _characterAnimator;

    [SerializeField]
    private Team _currentTarget = null;
    [SerializeField]
    private BaseGun _currentGun;
    [SerializeField]
    private float _minAttackDistance = 0.5f;
    [SerializeField]
    private float _maxAttackDistance = 1.5f;
    private Team[] _allCharacters;


    public AI_States _state = AI_States.idle;

    private void Start()
    {
        _allCharacters = GameObject.FindObjectsOfType<Team>();

        _myTransform = transform;

        MyTeam = GetComponent<Team>();

        MyVitals = GetComponent<Vitals>();

        _navMeshAgent = GetComponent<NavMeshAgent>();

        _characterAnimator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (MyVitals.IsAlive())
        {
            switch (_state)
            {
                case AI_States.idle:
                    StateIdle();
                    break;
                case AI_States.followThePlayer:
                    StateFollowThePlayer();
                    break;
                case AI_States.investigate:
                    StateInvestigate();
                    break;
                case AI_States.meleeCombat:
                    StateCombat();
                    break;
            }
        }
        else
        {
            _characterAnimator.SetBool("Move", false);

            _characterAnimator.SetBool("Dead", true);

            _state = AI_States.death;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        GetNewTarget();

        if (_currentTarget == null)
        {
            MyVitals.GetRessurect();

            _state = AI_States.idle;

            _characterAnimator.SetBool("Dead", false);

            _characterAnimator.SetBool("HasEnemy", false);
        }
    }



    private void StateIdle()
    {
        if (IsTargetAlive())
        {
            if (IsDistanceCorrect())
            {
                _state = AI_States.meleeCombat;
            }
            else
            {
                _state = AI_States.investigate;
            }

        }
        else
        {
            Team _bestTarget = GetNewTarget();

            if (_bestTarget != null)
            {
                _characterAnimator.SetBool("HasEnemy", true);

                _currentTarget = _bestTarget;
            }
            else
            {
                _characterAnimator.SetBool("HasEnemy", false);

                if (Vector3.Distance(_myTransform.position, _player.position) > 3f)
                {
                    _state = AI_States.followThePlayer;
                }
                else
                {
                    _characterAnimator.SetBool("Move", false);

                    _state = AI_States.idle;         // действие Idle
                }

            }
        }
    }


    private void StateFollowThePlayer()
    {
        if (IsTargetAlive())
        {
            if (IsDistanceCorrect())
            {
                _state = AI_States.meleeCombat;
            }
            else
            {
                _state = AI_States.investigate;
            }

        }
        else
        {
            Team _bestTarget = GetNewTarget();

            if (_bestTarget != null)
            {
                _characterAnimator.SetBool("HasEnemy", true);

                _currentTarget = _bestTarget;

            }
            else
            {
                _characterAnimator.SetBool("HasEnemy", false);

                if (Vector3.Distance(_myTransform.position, _player.position) > 3f)
                {
                    _characterAnimator.SetBool("Move", true);

                    _myTransform.LookAt(_player);

                    _navMeshAgent.SetDestination(_followPoint.position);            // действие Follow The Player

                    if (Vector3.Distance(_followPoint.position, _myTransform.position) < 0.3f)
                    {
                        _characterAnimator.SetBool("Move", false);

                        _state = AI_States.idle;
                    }
                }
                else
                {
                    _characterAnimator.SetBool("Move", false);

                    _state = AI_States.idle;
                }
            }
        }
    }



    private void StateInvestigate()
    {
        if (IsTargetAlive())
        {
            if (IsDistanceCorrect())
            {

                _state = AI_States.meleeCombat;
            }
            else
            {
                _characterAnimator.SetBool("Move", true);

                _navMeshAgent.SetDestination(_currentTarget.transform.position); // действие Investigate
            }

        }
        else
        {
            Team _bestTarget = GetNewTarget();

            if (_bestTarget != null)
            {
                _characterAnimator.SetBool("HasEnemy", true);

                _currentTarget = _bestTarget;
            }
            else
            {
                _characterAnimator.SetBool("HasEnemy", false);

                if (Vector3.Distance(_myTransform.position, _player.position) > 3f)
                {
                    _state = AI_States.followThePlayer;
                }
                else
                {
                    _characterAnimator.SetBool("Move", false);

                    _state = AI_States.idle;
                }
            }
        }
    }


    private void StateCombat()
    {
        if (IsTargetAlive())
        {
            if (IsDistanceCorrect())
            {
                _characterAnimator.SetBool("Move", false);

                _characterAnimator.SetTrigger("Fire");

                _myTransform.LookAt(_currentTarget.transform);

                _currentGun.Shoot(_currentTarget.Eyes.position);

            }
            else
            {

                _state = AI_States.investigate;
            }

        }
        else
        {
            Team _bestTarget = GetNewTarget();

            if (_bestTarget != null)
            {
                _characterAnimator.SetBool("HasEnemy", true);

                _currentTarget = _bestTarget;
            }
            else
            {
                _characterAnimator.SetBool("HasEnemy", false);

                if (Vector3.Distance(_myTransform.position, _player.position) > 3f)
                {
                    _state = AI_States.followThePlayer;
                }
                else
                {
                    _characterAnimator.SetBool("Move", false);

                    _state = AI_States.idle;
                }
            }
        }

    }


    private Team GetNewTarget()
    {
        Team _bestTarget = null;

        for (int i = 0; i < _allCharacters.Length; i++)
        {
            Team _currentCharacter = _allCharacters[i];

            //выбирать текущего врага в качестве цели, только если мы не в одной команде и если у него осталось здоровье
            if (_currentCharacter.GetComponent<Team>().GetTeamNumber() != MyTeam.GetTeamNumber()
                && _currentCharacter.GetComponent<Vitals>().IsAlive())
            {
                //если рейкаст попал в цель, то мы знаем, что можем его увидеть
                if (CanSeeTarget(_currentCharacter))
                {
                    if (_bestTarget == null)
                    {
                        _bestTarget = _currentCharacter;
                    }
                    else
                    {
                        //если текущий враг ближе, чем лучшая цель, то выбрать текущего солдата в качестве лучшей цели
                        if (Vector3.Distance(_currentCharacter.transform.position, _myTransform.position) < Vector3.Distance(_bestTarget.transform.position, _myTransform.position))
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
        if (_currentTarget != null
            && _currentTarget.GetComponent<Vitals>().IsAlive())
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
        if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _maxAttackDistance
                && Vector3.Distance(_myTransform.position, _currentTarget.transform.position) >= _minAttackDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
