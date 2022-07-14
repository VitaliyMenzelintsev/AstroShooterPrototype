using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]
[RequireComponent(typeof(Animator))]

public class EnemyRangeBehavior : EnemyBehavior
{
    [HideInInspector]
    public Team MyTeam;
    [HideInInspector]
    public Vitals MyVitals;
    public Transform Eyes;

    private NavMeshAgent _navMeshAgent;
    private Transform _myTransform;
    private Animator _characterAnimator;
    private EnemyCoverManager _coverManager;

    [SerializeField]
    private Team _currentTarget;
    [SerializeField]
    private BaseGun _currentGun;
    [SerializeField]
    private float _minAttackDistance = 5;
    [SerializeField]
    private float _maxAttackDistance = 13;
    [SerializeField]
    private float _punchDistance = 1;
    [SerializeField]
    private EnemyCoverSpot _currentCover = null;
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

        _coverManager = GameObject.FindObjectOfType<EnemyCoverManager>();
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
                case AI_States.moveToCover:
                    StateMoveToCover();
                    break;
                case AI_States.rangeCombat:
                    StateRangeCombat();
                    break;
                case AI_States.meleeCombat:
                    StateMeleeCombat();
                    break;
                default:
                    break;
            }
        }
        else
        {
            _state = AI_States.death;

            _characterAnimator.SetBool("Move", false);

            _characterAnimator.SetBool("Dead", true);

            if (_currentCover != null)
                _coverManager.ExitCover(ref _currentCover);

            Destroy(GetComponent<CapsuleCollider>());

            Destroy(gameObject, 10f);
        }
    }


    private void StateIdle()
    {
        if (_currentTarget != null
            && _currentTarget.GetComponent<Vitals>().IsAlive()) // если есть цель
        {
            if (_currentCover == null) // запрашиваем укрытие только в случае, если у персонажа нет укрытия
                _currentCover = _coverManager.GetCover(this, _currentTarget);


            if (_currentCover != null) // если существует укрытие
            {
                if (Vector3.Distance(_myTransform.position, _currentCover.transform.position) > 0.2F) // если расстояние до укрытия больше 20 см.
                {
                    _characterAnimator.SetBool("Move", true);    // двигаемся к нему
                    _characterAnimator.SetBool("HasEnemy", true);
                    _state = AI_States.moveToCover;
                }
                else // если персонаж уже в укрытии (< 0.2f до укрытия)
                {
                    if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _maxAttackDistance // проверка на дистанцию ни к чему не ведёт: либо убрать, либо придумать действие
                      && Vector3.Distance(_myTransform.position, _currentTarget.transform.position) >= _minAttackDistance)
                    {
                        _state = AI_States.rangeCombat;
                    }
                }
            }
            else // если нет укрытия
            {
                _state = AI_States.rangeCombat;
            }
        }
        else // если цели нет
        {
            Team _bestTarget = GetNewTarget();

            if (_bestTarget != null)
            {
                _currentTarget = _bestTarget;
            }
            else
            {
                _characterAnimator.SetBool("HasEnemy", false);

                _characterAnimator.SetBool("Move", false);

                _state = AI_States.idle;
            }
        }
    }


    private void StateMoveToCover()
    {
        if (_currentTarget != null) // если есть цель
        {
            if (_currentCover != null) // если существует укрытие
            {
                _navMeshAgent.SetDestination(_currentCover.transform.position); // идём к укрытию

                if (Vector3.Distance(this.transform.position, _currentCover.transform.position) <= 0.2f) //если дошли до укрытия
                {
                    _characterAnimator.SetBool("Move", false); // начинаем бой

                    _state = AI_States.rangeCombat;

                    return; //??
                }
            }
            else // если укрытия нет
            {
                _characterAnimator.SetBool("Move", false);  // останавливаемся и начинаем бой

                _state = AI_States.rangeCombat;

                return;  //??
            }

        }
        else // если цели нет, стоим
        {
            _characterAnimator.SetBool("Move", false);

            _characterAnimator.SetBool("HasEnemy", false);

            _state = AI_States.idle;
        }
    }


    private void StateRangeCombat()
    {
        if (_currentTarget != null
            && _currentTarget.GetComponent<Vitals>().IsAlive()) // если цель жива
        {

            if (!CanSeeTarget(_currentTarget)) // если цель пропала из зоны видимости
            {
                Team _alternativeTarget = GetNewTarget(); // ищем альтернативную цель

                if (_alternativeTarget == null) // если альтернативной цели нет
                {
                    _characterAnimator.SetBool("Move", false);  // переходим в ожидание

                    _state = AI_States.idle;

                }
                else // если альтернативная цель есть - она становится основной
                {
                    _currentTarget = _alternativeTarget;
                }
                return;
            }

            _myTransform.LookAt(_currentTarget.transform); // смотрим на цель

            // если дистанция для атаки подходящая
            if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _maxAttackDistance
                && Vector3.Distance(_myTransform.position, _currentTarget.transform.position) >= _minAttackDistance)
            {
                // атакуем

                _characterAnimator.SetTrigger("Fire");

                _currentGun.Aim(_currentTarget.Eyes.position);

                _currentGun.Shoot(_currentTarget.Eyes.position);


            }
            else if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) < _minAttackDistance)
            {
                _characterAnimator.SetTrigger("Punch");

                _state = AI_States.meleeCombat;
            }
            else // если дистанция не подходящая, начинаем стоять 
            {
                _characterAnimator.SetBool("Move", false);

                _state = AI_States.idle;
            }
        }
        else // если цели нет, начианем стоять
        {
            _characterAnimator.SetBool("Move", false);


            if (_currentCover != null)
                _coverManager.ExitCover(ref _currentCover);


            _state = AI_States.idle;
        }
    }


    private void StateMeleeCombat()
    {
        if (_currentTarget != null
            && _currentTarget.GetComponent<Vitals>().IsAlive()) // если есть
        {
            _myTransform.LookAt(_currentTarget.transform); // смотрим на цель

            // если дистанция для атаки подходящая
            if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _punchDistance)
            {
                // атакуем

                _characterAnimator.SetTrigger("Punch");

                _currentGun.Punch();

            }
            else // если дистанция не подходящая, возвращаемся в состояние атаки
            {
                _characterAnimator.SetBool("Move", false);

                _state = AI_States.rangeCombat;
            }
        }
        else // если цели нет
        {
            _characterAnimator.SetBool("Move", false);

            _state = AI_States.idle;
        }
    }


    private Team GetNewTarget()
    {
        Team _bestTarget = null;

        for (int i = 0; i < _allCharacters.Length; i++)
        {
            Team _currentCharacter = _allCharacters[i];

            //выбирать текущего солдата в качестве цели, только если мы не в одной команде и если у него осталось здоровье
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
                        //если текущая цель ближе, чем лучшая цель, то выбрать текущую цель 
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

        Vector3 _enemyPosition = _target.Eyes.position; ;

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
