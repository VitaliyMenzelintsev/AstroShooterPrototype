using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]
public class DroneBehavior : MonoBehaviour
{
    [HideInInspector]
    public Team MyTeam;
    [HideInInspector]
    public Vitals MyVitals;
    public Transform Eyes;

    private NavMeshAgent _navMeshAgent;
    private Transform _myTransform;

    [SerializeField] // тест
    private Team _currentTarget;
    [SerializeField]
    private Gun _currentGun;
    [SerializeField]
    private float _minAttackDistance = 2;
    [SerializeField]
    private float _maxAttackDistance = 5;
    [SerializeField]
    private float _moveSpeed = 3.2f;
    [SerializeField]
    private float _fireCooldown = 0.5f;
    private float _currentFireCooldown = 0;
    private Team[] _allCharacters;


    public enum AI_States
    {
        idle,
        moveToTarget,
        rangeCombat,
        death
    }


    public AI_States _state = AI_States.idle;


    private void Start()
    {

        _myTransform = transform;

        MyTeam = GetComponent<Team>();

        MyVitals = GetComponent<Vitals>();

        _navMeshAgent = GetComponent<NavMeshAgent>();
    }


    private void FixedUpdate()
    {
        if (MyVitals.GetCurrentHealth() > 0)
        {
            switch (_state)
            {
                case AI_States.idle:
                    StateIdle();
                    break;
                case AI_States.rangeCombat:
                    StateRangeCombat();
                    break;
                case AI_States.moveToTarget:
                    StateMoveToTarget();
                    break;
                default:
                    break;
            }
        }
        else
        {
            _state = AI_States.death;

            Destroy(gameObject, 3f);
        }
    }


    private void StateIdle()
    {
        if (_currentTarget != null) // если есть цель
        {
            if (_currentTarget.GetComponent<Vitals>().GetCurrentHealth() > 0) // если цель жива
            {
                if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _maxAttackDistance 
                  && Vector3.Distance(_myTransform.position, _currentTarget.transform.position) >= _minAttackDistance)
                {
                    _state = AI_States.rangeCombat;
                }
                else
                {
                    _state = AI_States.moveToTarget;
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
        else // если цели нет
        {
            Team _bestTarget = GetNewTarget();

            if (_bestTarget != null)
            {
                _currentTarget = _bestTarget;
            }
            else
            {
                _state = AI_States.idle;
            }
        }
    }


    private void StateMoveToTarget()
    {
        if (_currentTarget != null 
            && _currentTarget.GetComponent<Vitals>().GetCurrentHealth() > 0)
        {
            if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) >= _maxAttackDistance)
            {
                _navMeshAgent.SetDestination(_currentTarget.transform.position);
            }
            else
            {
                _state = AI_States.rangeCombat;
            }
        }
        else
        {
            _state = AI_States.idle;
        }
           
    }


    private void StateRangeCombat()
    {
        if (_currentTarget != null) // если есть цель
        {
            if (_currentTarget.GetComponent<Vitals>().GetCurrentHealth() > 0) // если цель жива
            {
                if (!CanSeeTarget(_currentTarget)) // если цель пропала из зоны видимости
                {
                    Team _alternativeTarget = GetNewTarget(); // ищем альтернативную цель

                    if (_alternativeTarget == null) // если альтернативной цели нет
                    {
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
                    if (_currentFireCooldown <= 0)
                    {
                        _currentGun.Aim(_currentTarget.Eyes.position);

                        _currentGun.Shoot(_currentTarget.Eyes.position);

                        _currentFireCooldown = _fireCooldown;
                    }
                    else
                    {
                        _currentFireCooldown -= 1 * Time.deltaTime;
                    }
                }
                else // если дистанция не подходящая, начинаем стоять 
                {
                    _state = AI_States.idle;
                }
            }
            else // если цель мертва, начинаем стоять
            {
                _state = AI_States.idle;
            }
        }
        else // если цели нет, начианем стоять
        {
            _state = AI_States.idle;
        }
    }


    private Team GetNewTarget()
    {
        _allCharacters = GameObject.FindObjectsOfType<Team>(); // раньше было в GetTarget

        Team _bestTarget = null;

        for (int i = 0; i < _allCharacters.Length; i++)
        {
            Team _currentCharacter = _allCharacters[i];

            if (_currentCharacter.GetComponent<Team>().GetTeamNumber() == MyTeam.GetTeamNumber()
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
