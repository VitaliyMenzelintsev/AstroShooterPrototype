using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]
[RequireComponent(typeof(Animator))]

public class EnemyRangeBehavior : MonoBehaviour
{
    [HideInInspector]
    public Team MyTeam;
    [HideInInspector]
    public Vitals MyVitals;
    public Transform Eyes;

    private Transform _myTransform;
    private Animator _characterAnimator;
    private EnemyCoverManager _coverManager;
    public Team _currentTarget;

    [SerializeField]
    private Gun _currentGun;
    [SerializeField]
    private float _minAttackDistance = 5;
    [SerializeField]
    private float _maxAttackDistance = 13;
    [SerializeField]
    private float _moveSpeed = 3.2f;
    //[SerializeField]
    //private float _damageDealt = 17F;
    [SerializeField]
    private float _fireCooldown = 0F;
    private float _currentFireCooldown = 0;

    private Path _currentPath = null;
    public EnemyCoverSpot _currentCover = null;
    private float _coverChangeCooldown = 5;
    private float _currentCoverChangeCooldown;

    private Vector3 _targetLastKnownPosition;

    public enum AI_States
    {
        idle,
        moveToCover,
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

        _characterAnimator = GetComponent<Animator>();

        _coverManager = GameObject.FindObjectOfType<EnemyCoverManager>();

        _currentCoverChangeCooldown = _coverChangeCooldown;
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
                case AI_States.investigate:
                    StateInvestigate();
                    break;
                case AI_States.moveToCover:
                    StateMoveToCover();
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

            _characterAnimator.SetBool("Dead", true);

            if (_currentCover != null)
            {
                _coverManager.ExitCover(_currentCover);
            }

            _state = AI_States.death;

            Destroy(gameObject, 1f);
        }
    }


    private void StateIdle()
    {
        if (_currentTarget != null && _currentTarget.GetComponent<Vitals>().GetCurrentHealth() > 0)
        {
            if (_currentCover != null)
            {
                _coverManager.ExitCover(_currentCover);
            }

            //_currentCover = _coverManager.GetCoverTowardsTarget(this, _currentTarget.transform.position, _maxAttackDistance, _minAttackDistance, _currentCover);

            if (_currentCover != null)
            {
                if (Vector3.Distance(_myTransform.position, _currentCover.transform.position) > 0.2F) // если расстояние до укрытия больше 20 см.
                {
                    _currentPath = CalculatePath(_myTransform.position, _currentCover.transform.position);

                    _characterAnimator.SetBool("Move", true);

                    _state = AI_States.moveToCover;
                }
                else
                {
                    _state = AI_States.combat;
                }
            }
            else
            {
                if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _maxAttackDistance
                    && Vector3.Distance(_myTransform.position, _currentTarget.transform.position) >= _minAttackDistance)
                {
                    _state = AI_States.combat;
                }
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


    private void StateMoveToCover()
    {
        if (_currentTarget != null
            && _currentCover != null
           /* && _currentCover.AmICoveredFrom(_currentTarget.transform.position)*/)
        {
            if (_currentPath != null)
            {
                Team _alternativeTarget = GetNewTarget();

                if (_alternativeTarget != null && _alternativeTarget != _currentTarget)
                {
                    float _distanceToCurrentTarget = Vector3.Distance(_myTransform.position, _currentTarget.transform.position);

                    float _distanceToAlternativeTarget = Vector3.Distance(_myTransform.position, _alternativeTarget.transform.position);

                    float _distanceBetweenTargets = Vector3.Distance(_currentTarget.transform.position, _alternativeTarget.transform.position);

                    if (Mathf.Abs(_distanceToAlternativeTarget - _distanceToCurrentTarget) > 5 && _distanceBetweenTargets > 5)
                    {
                        _currentTarget = _alternativeTarget;

                        _coverManager.ExitCover(_currentCover);

                        //_currentCover = _coverManager.GetCoverTowardsTarget(this, _currentTarget.transform.position, _maxAttackDistance, _minAttackDistance, _currentCover);

                        _currentPath = CalculatePath(_myTransform.position, _currentCover.transform.position);

                        return;
                    }
                }

                if (_currentPath.ReachedEndNode())
                { //если мы дошли до конца, мы начнем искать цель
                    _characterAnimator.SetBool("Move", false);

                    _currentPath = null;

                    _state = AI_States.combat;

                    return;
                }

                Vector3 _nodePosition = _currentPath.GetNextNode();

                if (Vector3.Distance(_myTransform.position, _nodePosition) < 0.1f)
                {
                    _currentPath._currentPathIndex++;
                }
                else
                {
                    _myTransform.LookAt(_nodePosition); 

                    _myTransform.Translate(Vector3.forward * _moveSpeed * Time.deltaTime);
                }
            }
            else
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


    private void StateCombat()
    {
        if (_currentTarget != null
            && _currentTarget.GetComponent<Vitals>().GetCurrentHealth() > 0)
        {
            //если цель убегает во время боя
            if (!CanSeeTarget(_currentTarget))
            {
                Team _alternativeTarget = GetNewTarget();

                if (_alternativeTarget == null)
                {
                    _targetLastKnownPosition = _currentTarget.transform.position;

                    _currentPath = CalculatePath(_myTransform.position, _targetLastKnownPosition);
                    _characterAnimator.SetBool("Move", true);

                    if (_currentCover != null)
                    {
                        _coverManager.ExitCover(_currentCover);
                    }

                    _state = AI_States.investigate;
                }
                else
                {
                    _currentTarget = _alternativeTarget;
                }
                return;
            }

            _myTransform.LookAt(_currentTarget.Eyes.transform);

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
            else
            {
                if (_currentCoverChangeCooldown <= 0) 
                {
                    _currentCoverChangeCooldown = _coverChangeCooldown;

                    _characterAnimator.SetBool("Move", false);

                    _state = AI_States.idle;
                }
                else
                {
                    _currentCoverChangeCooldown -= 1 * Time.deltaTime;
                }
            }
        }
        else
        {
            _state = AI_States.idle;  

        }
    }


    private void StateInvestigate()
    {
        if (_currentPath != null)
        {
            Team _alternativeTarget = GetNewTarget();

            if (_currentPath.ReachedEndNode() || _alternativeTarget != null)
            { //если мы дошли до конца, мы начнем искать цель
                _characterAnimator.SetBool("Move", false);

                _currentPath = null;
                _currentTarget = _alternativeTarget;

                _state = AI_States.idle;
                return;
            }

            Vector3 _nodePosition = _currentPath.GetNextNode();

            if (Vector3.Distance(_myTransform.position, _nodePosition) < 1)
            {
                //если мы достигли текущего узла, то мы начнем двигаться к следующему узлу
                _currentPath._currentPathIndex++;
            }
            else
            {
                //иначе мы будем двигаться к текущему узлу
                _myTransform.LookAt(_nodePosition);
                _myTransform.Translate(Vector3.forward * _moveSpeed * Time.deltaTime);
            }

        }
        else
        {
            //если у нас нет пути, мы будем искать цель
            _characterAnimator.SetBool("Move", false);

            _currentPath = null;
            _currentTarget = null;

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
                && _currentCharacter.GetComponent<Vitals>().GetCurrentHealth() > 0)
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


    private Path CalculatePath(Vector3 _source, Vector3 _destination)
    {
        NavMeshPath _navMeshPath = new NavMeshPath();

        NavMesh.CalculatePath(_source, _destination, NavMesh.AllAreas, _navMeshPath);

        Path _path = new Path(_navMeshPath.corners);

        return _path;
    }
}
