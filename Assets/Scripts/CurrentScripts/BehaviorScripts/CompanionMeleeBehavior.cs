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
    public Transform FollowPoint;
    public Transform Player;

    private NavMeshAgent _navMeshAgent;
    private Transform _myTransform;
    private Animator _characterAnimator;
    public Team _currentTarget; // паблик для тестов

    [SerializeField]
    private Gun _currentGun;
    [SerializeField]
    private float _minAttackDistance = 7;
    [SerializeField]
    private float _maxAttackDistance = 13;
    [SerializeField]
    private float _moveSpeed = 3.4f;
    [SerializeField]
    private float _fireCooldown = 0f;
    private float _currentFireCooldown = 0;

    private Path _currentPath = null;

    private Vector3 _targetLastKnownPosition;

    public enum AI_States
    {
        idle,
        followThePlayer,
        combat,
        investigate,
        death
    }

    public AI_States _state = AI_States.idle;

    private void Start()
    {
        _myTransform = transform;

        MyTeam = GetComponent<Team>();

        MyVitals = GetComponent<Vitals>();

        _navMeshAgent = GetComponent<NavMeshAgent>();

        _characterAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (MyVitals.GetCurrentHealth() > 0)
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
                case AI_States.combat:
                    StateCombat();
                    break;
                default:
                    break;
            }
        }
        else
        {
            _characterAnimator.SetBool("Move", false);

            Destroy(GetComponent<CapsuleCollider>());

            Destroy(GetComponent<NavMeshAgent>());

            _characterAnimator.SetBool("Dead", true);

            _state = AI_States.death;
        }
    }


    private void StateFollowThePlayer()
    {
        _currentTarget = GetNewTarget();

        if (_currentTarget == null)
        {
            if (Vector3.Distance(transform.position, Player.position) > 3f)
            {
                _myTransform.LookAt(Player);
                _characterAnimator.SetBool("Move", true);
                _navMeshAgent.SetDestination(FollowPoint.position);
            }
            else
            {
                _characterAnimator.SetBool("Move", false);

                if (Vector3.Distance(FollowPoint.position, _myTransform.position) < 0.3f)
                {
                    _characterAnimator.SetBool("Move", false);
                    _state = AI_States.idle;
                }
            }
        }
        else
        {
            _characterAnimator.SetBool("Move", false);
            _state = AI_States.idle;
        }
    }


    private void StateIdle()
    {
        if (_currentTarget != null)
        {
            if (_currentTarget.GetComponent<Vitals>().GetCurrentHealth() > 0)
            {
                if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _maxAttackDistance
                    && Vector3.Distance(_myTransform.position, _currentTarget.transform.position) >= _minAttackDistance)
                {
                    _state = AI_States.combat;
                }
                else
                {
                    _state = AI_States.investigate;
                }
            }
        }
        else
        {
            Team _bestTarget = GetNewTarget();

            if (_bestTarget != null)
            {
                _currentTarget = _bestTarget;
            }
            else
            {
                _characterAnimator.SetBool("Move", true); // !!
                _characterAnimator.SetBool("HasEnemy", false); // !!
                _state = AI_States.followThePlayer;
            }
        }
    }


    private void StateCombat()
    {
        if (_currentTarget != null
           && _currentTarget.GetComponent<Vitals>().GetCurrentHealth() > 0)
        {
            _myTransform.LookAt(_currentTarget.transform);

            if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _maxAttackDistance
                && Vector3.Distance(_myTransform.position, _currentTarget.transform.position) >= _minAttackDistance)
            {
                // Атака
                if (_currentFireCooldown <= 0)
                {
                    _characterAnimator.SetTrigger("Fire");

                    _currentGun.Aim(_currentTarget.Eyes.position);

                    _currentGun.Shoot(_currentTarget.Eyes.position);

                    _currentFireCooldown = _fireCooldown;
                }
                else
                {
                    _currentFireCooldown -= 1 * Time.deltaTime;
                }
            }

            if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) > _maxAttackDistance)
            {
                _state = AI_States.investigate;
            }
        }
        else
        {
            _characterAnimator.SetBool("Move", false);

            _state = AI_States.idle;
        }
    }


    private void StateInvestigate()
    {
        if (_currentTarget != null)
        {
            if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= 2.5f)
            {
                _state = AI_States.combat;
            }
            else
            {
                _characterAnimator.SetBool("Move", true);

                _navMeshAgent.SetDestination(_currentTarget.transform.position);
            }

            if (_currentTarget == null)
            {
                _characterAnimator.SetBool("Move", false);

                _state = AI_States.idle;
            }

        }
        else
        {
            _characterAnimator.SetBool("Move", true); //!
            _characterAnimator.SetBool("HasEnemy", false); //!!
            _state = AI_States.followThePlayer;
        }
    }


    private Team GetNewTarget()
    {
        Team[] _allCharacters = GameObject.FindObjectsOfType<Team>();

        Team _bestTarget = null;

        for (int i = 0; i < _allCharacters.Length; i++)
        {
            Team _currentCharacter = _allCharacters[i];

            //выбирать текущего врага в качестве цели, только если мы не в одной команде и если у него осталось здоровье
            if (_currentCharacter.GetComponent<Team>().GetTeamNumber() != MyTeam.GetTeamNumber()
                && _currentCharacter.GetComponent<Vitals>().GetCurrentHealth() > 0)
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


    private Path CalculatePath(Vector3 _source, Vector3 _destination) // высчитывание пути
    {
        NavMeshPath _navMeshPath = new NavMeshPath();

        NavMesh.CalculatePath(_source, _destination, NavMesh.AllAreas, _navMeshPath);

        Path _path = new Path(_navMeshPath.corners);

        return _path;
    }
}
