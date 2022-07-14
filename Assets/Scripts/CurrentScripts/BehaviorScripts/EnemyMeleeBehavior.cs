using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]
[RequireComponent(typeof(Animator))]

public class EnemyMeleeBehavior : EnemyBehavior
{
    [HideInInspector]
    public Team MyTeam;
    [HideInInspector]
    public Vitals MyVitals;
    public Transform Eyes;

    private NavMeshAgent _myNavMeshAgent;
    private Transform _myTransform;
    private Animator _characterAnimator;
    public Team _currentTarget = null;

    [SerializeField]
    private float _minAttackDistance = 0.5f;
    [SerializeField]
    private float _maxAttackDistance = 2.5f;
    [SerializeField]
    private float _damageDealt = 100f;
    [SerializeField]
    private float _fireCooldown = 2.667f;
    private float _currentFireCooldown = 0;


    public enum AI_States
    {
        idle,
        investigate,
        combat,
        death
    }

    public AI_States _state = AI_States.idle;

    private void Start()
    {
        _myTransform = transform;

        MyTeam = GetComponent<Team>();

        MyVitals = GetComponent<Vitals>();

        _myNavMeshAgent = GetComponent<NavMeshAgent>();

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
                case AI_States.combat:
                    StateCombat();
                    break;
                case AI_States.investigate:
                    StateInvestigate();
                    break;
                case AI_States.death:
                    break;
                default:
                    StateIdle();
                    break;
            }
        }
        else
        {
            _characterAnimator.SetBool("Move", false);

            _characterAnimator.SetBool("Dead", true);

            Destroy(GetComponent<CapsuleCollider>());

            Destroy(GetComponent<NavMeshAgent>());

            _state = AI_States.death;

            Destroy(gameObject, 7f);
        }
    }


    private void StateIdle()
    {
        if (_currentTarget != null
            && _currentTarget.GetComponent<Vitals>().IsAlive())
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
        else
        {
            //ищем новую цель
            Team _bestTarget = GetNewTarget();

            if (_bestTarget != null)
            {
                _currentTarget = _bestTarget;
            }
        }
    }

    private void StateCombat()
    {
        if (_currentTarget != null
            && _currentTarget.GetComponent<Vitals>().IsAlive())
        {
            _myTransform.LookAt(_currentTarget.transform);

            if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _maxAttackDistance
                && Vector3.Distance(_myTransform.position, _currentTarget.transform.position) >= _minAttackDistance)
            {
                // Атака
                if (_currentFireCooldown <= 0)
                {
                    _characterAnimator.SetTrigger("Fire");

                    _currentTarget.GetComponent<Vitals>().GetHit(_damageDealt);

                    _currentFireCooldown = _fireCooldown;
                }
                else
                {
                    _currentFireCooldown -= 1 * Time.deltaTime;
                }
            }

            else if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) > _maxAttackDistance)
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
        if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _maxAttackDistance)
        {
            _state = AI_States.combat;
        }
        else
        {
            _characterAnimator.SetBool("Move", true);

            _myNavMeshAgent.SetDestination(_currentTarget.transform.position);
        }

        if (_currentTarget == null)
        {
            _characterAnimator.SetBool("Move", false);

            _state = AI_States.idle;
        }
    }


    private Team GetNewTarget()
    {
        Team[] _allCharacters = GameObject.FindObjectsOfType<Team>();

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
