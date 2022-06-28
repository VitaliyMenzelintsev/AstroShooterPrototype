//using UnityEngine;
//using UnityEngine.AI;

//[RequireComponent(typeof(Team))]
//[RequireComponent(typeof(Vitals))]
//[RequireComponent(typeof(Animator))]

//public class EnemyMeleeBehavior : MonoBehaviour
//{
//    [HideInInspector]
//    public Team MyTeam;
//    [HideInInspector]
//    public Vitals MyVitals;
//    public Transform Eyes;

//    private Transform _myTransform;
//    private Animator _characterAnimator;
//    public Team _currentTarget;

//    [SerializeField]
//    private float _minAttackDistance = 0.5f;
//    [SerializeField]
//    private float _maxAttackDistance = 2.5f;
//    [SerializeField]
//    private float _moveSpeed = 3.6f;
//    [SerializeField]
//    private float _damageDealt = 70f;
//    [SerializeField]
//    private float _fireCooldown = 2.667f;
//    private float _currentFireCooldown = 0;

//    private Path _currentPath = null;

//    private Vector3 _targetLastKnownPosition;

//    public enum AI_States
//    {
//        idle,
//        investigate,
//        combat,
//        death
//    }

//    public AI_States _state = AI_States.idle;

//    private void Start()
//    {
//        _myTransform = transform;

//        MyTeam = GetComponent<Team>();

//        MyVitals = GetComponent<Vitals>();

//        _characterAnimator = GetComponent<Animator>();
//    }

//    private void Update()
//    {
//        if (MyVitals.GetCurrentHealth() > 0)
//        {
//            switch (_state)
//            {
//                case AI_States.idle:
//                    StateIdle();
//                    break;
//                case AI_States.investigate:
//                    StateInvestigate();
//                    break;
//                case AI_States.combat:
//                    StateCombat();
//                    break;
//                default:
//                    break;
//            }
//        }
//        else
//        {
//            _characterAnimator.SetBool("Move", false);

//            Destroy(GetComponent<CapsuleCollider>());

//            _characterAnimator.SetBool("Dead", true);

//            _state = AI_States.death;

//            Destroy(gameObject, 7f);
//        }
//    }


//    private void StateIdle()
//    {
//        //_characterAnimator.SetBool("Move", false);

//        if (_currentTarget != null && _currentTarget.GetComponent<Vitals>().GetCurrentHealth() > 0)
//        {
//            if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _maxAttackDistance
//                && Vector3.Distance(_myTransform.position, _currentTarget.transform.position) >= _minAttackDistance)
//            {
//                _characterAnimator.SetTrigger("Fire");
//                // если дистанция подходит для атаки - то атакуем
//                _state = AI_States.combat;
//            }
//            else
//            {
//                _characterAnimator.SetBool("Move", true);
//                // если дистанция не подходит для атаки, то начинаем преследование
//                _state = AI_States.investigate;
//            }
//        }
//        else
//        {
//            //ищем новую цель
//            Team _bestTarget = GetNewTarget();

//            if (_bestTarget != null)
//            {
//                _currentTarget = _bestTarget;
//            }
//        }
//    }


//    //private void StateMoveToCover()
//    //{
//    //    if (_currentTarget != null)
//    //    {
//    //        if (_currentPath != null)
//    //        {
//    //            if (_currentPath.ReachedEndNode())
//    //            { //если мы дошли до конца, мы начнем искать цель
//    //                _characterAnimator.SetBool("Move", false);

//    //                _currentPath = null;

//    //                _state = AI_States.combat;

//    //                return;
//    //            }

//    //            Vector3 _nodePosition = _currentPath.GetNextNode();

//    //            if (Vector3.Distance(_myTransform.position, _nodePosition) < 0.1f)
//    //            {
//    //                //if we reached the current node, then we'll begin going towards the next node
//    //                _currentPath._currentPathIndex++;
//    //            }
//    //            else
//    //            {
//    //                //else we'll move towards current node
//    //                _myTransform.LookAt(_nodePosition);

//    //                _myTransform.Translate(Vector3.forward * _moveSpeed * Time.deltaTime);
//    //            }
//    //        }
//    //        else
//    //        {
//    //            //if we don't have a path, we'll look for a target
//    //            _characterAnimator.SetBool("Move", false);

//    //            _state = AI_States.idle;
//    //        }
//    //    }
//    //    else
//    //    {
//    //        _characterAnimator.SetBool("Move", false);

//    //        _state = AI_States.idle;
//    //    }
//    //}


//    private void StateCombat()
//    {
//        _characterAnimator.SetBool("Move", false);

//        // запустить поиск цели

//        if (_currentTarget != null
//            && _currentTarget.GetComponent<Vitals>().GetCurrentHealth() > 0)
//        {
//            //если цель убегает во время боя
//            if (!CanSeeTarget(_currentTarget))
//            {
//                Team _alternativeTarget = GetNewTarget(); // возможно стоит отменить переключение целей

//                if (_alternativeTarget == null)
//                {
//                    _targetLastKnownPosition = _currentTarget.transform.position;

//                    _currentPath = CalculatePath(_myTransform.position, _targetLastKnownPosition);

//                    _state = AI_States.investigate;
//                }
//                else
//                {
//                    _currentTarget = _alternativeTarget;
//                }
//                return;
//            }

//            _myTransform.LookAt(_currentTarget.transform);

//            if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _maxAttackDistance
//                && Vector3.Distance(_myTransform.position, _currentTarget.transform.position) >= _minAttackDistance)
//            {
//                // Атака
//                if (_currentFireCooldown <= 0)
//                {
//                    _characterAnimator.SetTrigger("Fire");

//                    _currentTarget.GetComponent<Vitals>().GetHit(_damageDealt);

//                    _currentFireCooldown = _fireCooldown;
//                }
//                else
//                {
//                    _currentFireCooldown -= 1 * Time.deltaTime;
//                }
//            }
//            else
//            {
//                _state = AI_States.investigate;
//            }
//        }
//        else
//        {
//            _state = AI_States.idle;
//        }
//    }


//    private void StateInvestigate()
//    {
//        _currentPath = CalculatePath(_myTransform.position, _currentTarget.transform.position);

//        if (_currentPath != null)
//        {
//            Team _alternativeTarget = GetNewTarget();

//            if (_currentPath.ReachedEndNode() || _alternativeTarget != null)
//            { //если мы дошли до конца, мы начнем искать цель
//                _characterAnimator.SetBool("Move", false);

//                _currentPath = null;
//                _currentTarget = _alternativeTarget;

//                _state = AI_States.idle;
//                return;
//            }

//            Vector3 _nodePosition = _currentPath.GetNextNode();

//            if (Vector3.Distance(_myTransform.position, _nodePosition) < 1)
//            {
//                //если мы достигли текущего узла, то мы начнем двигаться к следующему узлу
//                _currentPath._currentPathIndex++;
//            }
//            else
//            {
//                //иначе мы будем двигаться к текущему узлу
//                _myTransform.LookAt(_nodePosition);
//                _myTransform.Translate(Vector3.forward * _moveSpeed * Time.deltaTime);
//            }

//        }
//        else
//        {
//            //если у нас нет пути, мы будем искать цель
//            _characterAnimator.SetBool("Move", false);

//            _currentPath = null;
//            _currentTarget = null;

//            _state = AI_States.idle;
//        }
//    }


//    private Team GetNewTarget()
//    {
//        Team[] _allCharacters = GameObject.FindObjectsOfType<Team>();

//        Team _bestTarget = null;

//        for (int i = 0; i < _allCharacters.Length; i++)
//        {
//            Team _currentCharacter = _allCharacters[i];

//            //выбирать текущего противника в качестве цели, только если мы не в одной команде и если у него осталось здоровье
//            if (_currentCharacter.GetComponent<Team>().GetTeamNumber() != MyTeam.GetTeamNumber()
//                && _currentCharacter.GetComponent<Vitals>().GetCurrentHealth() > 0)
//            {
//                //если рейкаст попал в цель, мы знаем, что видим её
//                if (CanSeeTarget(_currentCharacter))
//                {
//                    if (_bestTarget == null)
//                    {
//                        _bestTarget = _currentCharacter;
//                    }
//                    else
//                    {
//                        //если текущий враг ближе, чем лучшая цель, то выбрать текущего врага в качестве лучшей цели
//                        if (Vector3.Distance(_currentCharacter.transform.position, _myTransform.position) < Vector3.Distance(_bestTarget.transform.position, _myTransform.position))
//                        {
//                            _bestTarget = _currentCharacter;
//                        }
//                    }
//                }
//            }
//        }

//        return _bestTarget;
//    }


//    private bool CanSeeTarget(Team _target)
//    {
//        bool _canSeeIt = false;

//        Vector3 _enemyPosition = _target.Eyes.position; ;    // _target.Eyes.position; убрал про глаза

//        Vector3 _directionTowardsEnemy = _enemyPosition - Eyes.position;

//        RaycastHit _hit;

//        //направить луч на текущего врага
//        if (Physics.Raycast(Eyes.position, _directionTowardsEnemy, out _hit, Mathf.Infinity))
//        {
//            //если рейкаст попал в цель, то мы знаем, что можем его увидеть
//            if (_hit.transform == _target.transform)
//            {
//                _canSeeIt = true;
//            }
//        }

//        return _canSeeIt;
//    }


//    private Path CalculatePath(Vector3 _source, Vector3 _destination)
//    {
//        NavMeshPath _navMeshPath = new NavMeshPath();

//        NavMesh.CalculatePath(_source, _destination, NavMesh.AllAreas, _navMeshPath);

//        Path _path = new Path(_navMeshPath.corners);

//        return _path;
//    }
//}
