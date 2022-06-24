using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CompanionRangeBehavior : MonoBehaviour
{
    public Transform FollowPoint;
    public Transform Player;
    [HideInInspector]
    public Team MyTeam;
    [HideInInspector]
    public Vitals MyVitals;
    public Transform Eyes; // не нужно - целимся туда же, куда игрок (курсор)

    [SerializeField]
    private float _minAttackDistance = 10;
    [SerializeField]
    private float _maxAttackDistance = 25;
    [SerializeField]
    private float _moveSpeed = 15;
    [SerializeField]
    private float _damageDealt = 50F;
    [SerializeField]
    private float _fireCooldown = 1F;

    private Transform _myTransform;
    private Animator _characterAnimator;
    private CompanionCoverManager _coverManager;
    private EnemyRangeBehavior _currentTarget;  //!!!!!!!!!!!!!!!
    private CompanionCoverSpot _currentCover = null;
    private Path _currentPath = null;
    private float _coverChangeCooldown = 5;
    private float _currentCoverChangeCooldown;
    private float _currentFireCooldown = 0;

    //private NavMeshAgent _navMeshAgent;
    //private Camera _viewCamera;
    //private GunController _gunController;

    public enum AI_States
    {
        idle,
        moveToCover,
        combat,
        death
    }

    public AI_States _state = AI_States.idle;

    private void Start()
    {
        //_navMeshAgent = GetComponent<NavMeshAgent>();

        //_viewCamera = Camera.main;

        //_gunController = GetComponent<GunController>();

        MyTeam = GetComponent<Team>();

        MyVitals = GetComponent<Vitals>();

        _characterAnimator = GetComponent<Animator>();

        _coverManager = GameObject.FindObjectOfType<CompanionCoverManager>();

        _currentCoverChangeCooldown = _coverChangeCooldown;
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
            _characterAnimator.SetBool("move", false);

            Destroy(GetComponent<CapsuleCollider>());

            _characterAnimator.SetBool("dead", true);

            if (_currentCover != null)
            {
                _coverManager.ExitCover(_currentCover);
            }

            _state = AI_States.death;

            Destroy(gameObject, 25f);
        }
    }

    public void LookAt(Vector3 _lookPoint)
    {
        Vector3 _heightCorrectedPoint = new Vector3(_lookPoint.x, transform.position.y, _lookPoint.z);
        transform.LookAt(_heightCorrectedPoint);
    }

    private void StateIdle()
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
                if (Vector3.Distance(_myTransform.position, _currentCover.transform.position) > 0.2F)
                {
                    _currentPath = CalculatePath(_myTransform.position, _currentCover.transform.position);

                    _characterAnimator.SetBool("move", true);

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
                    //attack
                    _state = AI_States.combat;
                }
            }
        }
        else
        {
            //find new target
            EnemyRangeBehavior _bestTarget = GetNewTarget();

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
            && _currentCover.AmICoveredFrom(_currentTarget.transform.position))
        {
            if (_currentPath != null)
            {
                EnemyRangeBehavior _alternativeTarget = GetNewTarget();

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
                { //if we reached the end, we'll start looking for a target
                    _characterAnimator.SetBool("move", false);

                    _currentPath = null;

                    _state = AI_States.combat;

                    return;
                }

                Vector3 _nodePosition = _currentPath.GetNextNode();

                if (Vector3.Distance(_myTransform.position, _nodePosition) < 0.1f)  // 0.1 расстояние до точки укрытия
                {
                    //if we reached the current node, then we'll begin going towards the next node
                    _currentPath._currentPathIndex++;
                }
                else
                {
                    //else we'll move towards current node
                    _myTransform.LookAt(_nodePosition);

                    _myTransform.Translate(Vector3.forward * _moveSpeed * Time.deltaTime);
                }

            }
            else
            {
                //if we don't have a path, we'll look for a target
                _characterAnimator.SetBool("move", false);

                _state = AI_States.idle;
            }
        }
        else
        {
            _characterAnimator.SetBool("move", false);

            _state = AI_States.idle;
        }
    }

    private void StateCombat()
    {
        if (_currentTarget != null
            && _currentTarget.GetComponent<Vitals>().GetCurrentHealth() > 0)
        {
            //if the target escapes during combat
            if (!CanSeeTarget(_currentTarget))
            {
                EnemyRangeBehavior _alternativeTarget = GetNewTarget();

                _currentTarget = _alternativeTarget;

                return;
            }

            _myTransform.LookAt(_currentTarget.transform);

            if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _maxAttackDistance
                && Vector3.Distance(_myTransform.position, _currentTarget.transform.position) >= _minAttackDistance)
            {
                //attack
                if (_currentFireCooldown <= 0)
                {
                    _characterAnimator.SetTrigger("fire");

                    _currentTarget.GetComponent<Vitals>().GetHit(_damageDealt);

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

                    _characterAnimator.SetBool("move", false);

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

    private EnemyRangeBehavior GetNewTarget()
    {
        EnemyRangeBehavior[] _allCharacters = GameObject.FindObjectsOfType<EnemyRangeBehavior>();

        EnemyRangeBehavior _bestTarget = null;

        for (int i = 0; i < _allCharacters.Length; i++)
        {
            EnemyRangeBehavior _currentCharacter = _allCharacters[i];

            //only select current soldier as target, if we are not on the same team and if it got health left
            if (_currentCharacter.GetComponent<Team>().GetTeamNumber() != MyTeam.GetTeamNumber()
                && _currentCharacter.GetComponent<Vitals>().GetCurrentHealth() > 0)
            {
                //if the raycast hit the target, then we know that we can see it
                if (CanSeeTarget(_currentCharacter))
                {
                    if (_bestTarget == null)
                    {
                        _bestTarget = _currentCharacter;
                    }
                    else
                    {
                        //if current soldier is closer than best target, then choose current soldier as best target
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

    private bool CanSeeTarget(EnemyRangeBehavior _target)
    {
        bool _canSeeIt = false;

        //Can I see the Target Soldier?

        Vector3 _enemyPosition = _target.Eyes.position;

        Vector3 _directionTowardsEnemy = _enemyPosition - Eyes.position;

        RaycastHit _hit; //record of what we hit with the raycast

        //cast ray towards current soldier, make the raycast line infinity in length
        if (Physics.Raycast(Eyes.position, _directionTowardsEnemy, out _hit, Mathf.Infinity))
        {
            //if the raycast hit the target, then we know that we can see it
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

        NavMesh.CalculatePath(_source, _destination, NavMesh.AllAreas, _navMeshPath); //calculates a path using the Unity NavMesh

        Path _path = new Path(_navMeshPath.corners);

        return _path;
    }
}




//using UnityEngine;
//using UnityEngine.AI;

//[RequireComponent(typeof(NavMeshAgent))]
//public class CompanionBehavior : MonoBehaviour
//{
//    public Transform FollowPoint;
//    public Transform Player;
//    [HideInInspector]
//    public Team MyTeam;
//    [HideInInspector]
//    public Vitals MyVitals;

//    private NavMeshAgent _navMeshAgent;
//    private GunController _gunController;
//    //private Animator _npcAnimator;
//    private Camera _viewCamera;

//    public enum AI_States
//    {
//        idle,
//        moveToCover,
//        combat,
//        death
//    }

//    public AI_States _state = AI_States.idle;

//    private void Awake()
//    {
//        _navMeshAgent = GetComponent<NavMeshAgent>();

//        _gunController = GetComponent<GunController>();

//        //_npcAnimator = GetComponent<Animator>();

//        _viewCamera = Camera.main;

//        MyTeam = GetComponent<Team>();

//        MyVitals = GetComponent<Vitals>();
//    }

//    private void Update()
//    {
//        // Movement
//        if (Vector3.Distance(Player.position, transform.position) > 3)
//            _navMeshAgent.SetDestination(FollowPoint.position); // ввести проверку, если координаты цели изменились, то запустить поиск пути



//        // Look input
//        Ray _ray = _viewCamera.ScreenPointToRay(Input.mousePosition);
//        Plane _groundPlane = new Plane(Vector3.up, Vector3.up * 1.8f);
//        float _rayDistance;

//        if (_groundPlane.Raycast(_ray, out _rayDistance))
//        {
//            Vector3 _point = _ray.GetPoint(_rayDistance);
//            LookAt(_point);

//            if ((new Vector2(_point.x, _point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
//                _gunController.Aim(_point);
//        }
//    }

//    public void LookAt(Vector3 _lookPoint)
//    {
//        Vector3 _heightCorrectedPoint = new Vector3(_lookPoint.x, transform.position.y, _lookPoint.z);
//        transform.LookAt(_heightCorrectedPoint);
//    }
//}