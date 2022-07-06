using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]
[RequireComponent(typeof(Animator))]

public class CompanionRangeBehavior : MonoBehaviour
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
    private CompanionCoverManager _coverManager;
    public Team _currentTarget; // паблик для тестов

    [SerializeField]
    private Gun _currentGun;
    [SerializeField]
    private float _minAttackDistance = 7;
    [SerializeField]
    private float _maxAttackDistance = 13;
    [SerializeField]
    private float _moveSpeed = 3f;
    [SerializeField]
    private float _fireCooldown = 0.5f;
    private float _currentFireCooldown = 0;

    private Path _currentPath = null;
    private CompanionCoverSpot _currentCover = null;
    private float _coverChangeCooldown = 50;
    private float _currentCoverChangeCooldown;
    private Vector3 _targetLastKnownPosition;

    public enum AI_States
    {
        idle,
        followThePlayer,
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

        _navMeshAgent = GetComponent<NavMeshAgent>();  

        _characterAnimator = GetComponent<Animator>();

        _coverManager = GameObject.FindObjectOfType<CompanionCoverManager>();

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
                case AI_States.followThePlayer:
                    StateFollowThePlayer();
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
                case AI_States.death:
                    StateDeath();
                    break;
                default:
                    break;
            }
        }
        else
        {
            _characterAnimator.SetBool("Move", false);


            _characterAnimator.SetBool("Dead", true);


            if (_currentCover != null)
                _coverManager.ExitCover(_currentCover);
          

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

    private void StateDeath()
    {
        
    }


    private void StateFollowThePlayer()
    {
        if (_currentCover != null)
        {
            _coverManager.ExitCover(_currentCover);
        }

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

                // смотреть за проекцией курсора на поверхность уровня на высоте 1.8 м

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
            if (_currentTarget != null && _currentTarget.GetComponent<Vitals>().GetCurrentHealth() > 0)
            {
                if (_currentCover != null)
                {
                    _coverManager.ExitCover(_currentCover); 
                }

                _currentCover = _coverManager.GetCoverTowardsTarget(this, _currentTarget.transform.position, _maxAttackDistance, _minAttackDistance, _currentCover);

                if (_currentCover != null)
                {
                    if (Vector3.Distance(_myTransform.position, _currentCover.transform.position) > 0.2F) // если расстояние до укрытия больше 20 см.
                    {
                        _currentPath = CalculatePath(_myTransform.position, _currentCover.transform.position);

                        _characterAnimator.SetBool("Move", true);
                        _characterAnimator.SetBool("HasEnemy", true);  // !!
                        _state = AI_States.moveToCover;
                    }
                    else
                    {
                        _state = AI_States.combat; // !! можно добавить передачу анимации Fire
                    }
                }
                else
                {
                    if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _maxAttackDistance
                        && Vector3.Distance(_myTransform.position, _currentTarget.transform.position) >= _minAttackDistance)
                    {
                        _state = AI_States.combat; // !! можно добавить передачу анимации Fire
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
        else
        {
            _characterAnimator.SetBool("Move", true); // !!
            _characterAnimator.SetBool("HasEnemy", false); // !!
            _state = AI_States.followThePlayer;
        }
    }


    private void StateMoveToCover()
    {
        if (_currentTarget != null)
        {
            if (_currentCover != null
            && _currentCover.AmICoveredFrom(_currentTarget.transform.position))
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

                            _currentCover = _coverManager.GetCoverTowardsTarget(this, _currentTarget.transform.position, _maxAttackDistance, _minAttackDistance, _currentCover);

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

                    _state = AI_States.idle;
                }
            }
            else
            {
                // если нет цели, нет укрытия или укрытие не защищает, то:
                _characterAnimator.SetBool("Move", false);
                _state = AI_States.idle;
            }

        }
        else
        {
            _characterAnimator.SetBool("Move", true); // !!
            _characterAnimator.SetBool("HasEnemy", false); // !!
            _state = AI_States.followThePlayer;
        }
    }


    private void StateCombat()
    {
        if (_currentTarget != null)
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

                        _currentPath = CalculatePath(_myTransform.position, _targetLastKnownPosition);  // идём к последнему известному месту врага
                        _characterAnimator.SetBool("Move", true);

                        if (_currentCover != null)
                        {
                            _coverManager.ExitCover(_currentCover);
                        }

                        _characterAnimator.SetBool("Move", true); //!!
                        _characterAnimator.SetBool("HasEnemy", true); //!!
                        _state = AI_States.investigate; // не понимаю, как реализовать переход в аниматоре 
                    }
                    else
                    {
                        _currentTarget = _alternativeTarget;
                    }
                    return;
                }

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
                else
                {
                    if (_currentCoverChangeCooldown <= 0)    // СМЕНА УКРЫТИЙ ТУТ
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
                _characterAnimator.SetBool("Move", false); // !!
                _state = AI_States.idle;  
            }
        }
        else
        {
            _characterAnimator.SetBool("Move", true);
            _characterAnimator.SetBool("HasEnemy", false);
            _state = AI_States.followThePlayer;
        }
    }


    private void StateInvestigate()
    {
        if (_currentPath != null) // эта штука уже была
        {
            Team _alternativeTarget = GetNewTarget();

            if (_currentPath.ReachedEndNode() || _alternativeTarget != null)
            { 
                _currentPath = null;
                _currentTarget = _alternativeTarget;

                //если мы дошли до конца, мы начнем искать цель
                _characterAnimator.SetBool("Move", false);
                _characterAnimator.SetBool("HasEnemy", false); //!!
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

            //выбирать текущего солдата в качестве цели, только если мы не в одной команде и если у него осталось здоровье
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
