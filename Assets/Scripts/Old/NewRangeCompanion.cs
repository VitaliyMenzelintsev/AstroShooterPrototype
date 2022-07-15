//using UnityEngine.AI;
//using UnityEngine;

//public class CompanionRangeBehavior : MonoBehaviour
//{
//[HideInInspector]
//public Team MyTeam;
//[HideInInspector]
//public Vitals MyVitals;
//public Transform Eyes;

//[SerializeField]
//private Transform _followPoint;
//[SerializeField]
//private Transform _player;
//private NavMeshAgent _navMeshAgent;
//private Transform _myTransform;
//private Animator _characterAnimator;
//private CompanionCoverManager _coverManager;

//[SerializeField]
//private Team _currentTarget;
//[SerializeField]
//private BaseGun _currentGun;
//[SerializeField]
//private float _minAttackDistance = 1.5f;
//[SerializeField]
//private float _maxAttackDistance = 13f;
//[SerializeField]
//private float _punchDistance = 1.5f;
//[SerializeField]
//private CompanionCoverSpot _currentCover = null;
//[SerializeField]
//Team[] _allCharacters;

//public AI_States _state = AI_States.idle;


//private void Start()
//{
//    _allCharacters = GameObject.FindObjectsOfType<Team>();

//    _myTransform = transform;

//    MyTeam = GetComponent<Team>();

//    MyVitals = GetComponent<Vitals>();

//    _navMeshAgent = GetComponent<NavMeshAgent>();

//    _characterAnimator = GetComponent<Animator>();

//    _coverManager = GameObject.FindObjectOfType<CompanionCoverManager>();
//}

//private void FixedUpdate()
//{
//    if (MyVitals.IsAlive())
//    {
//        switch (_state)
//        {
//            case AI_States.idle:
//                StateIdle();
//                break;
//            case AI_States.followThePlayer:
//                StateFollowThePlayer();
//                break;
//            case AI_States.moveToCover:
//                StateMoveToCover();
//                break;
//            case AI_States.rangeCombat:
//                StateRangeCombat();
//                break;
//            case AI_States.meleeCombat:
//                StateMeleeCombat();
//                break;
//            case AI_States.death:
//                StateDeath();
//                break;
//        }
//    }
//    else
//    {
//        _state = AI_States.death;

//        _characterAnimator.SetBool("Move", false);

//        _characterAnimator.SetBool("Dead", true);

//        ExitCover();
//    }
//}


//private void OnTriggerEnter(Collider other)  // переработать рессурект/смерть
//{
//    GetNewTarget();

//    if (_currentTarget == null)
//    {
//        MyVitals.GetRessurect();

//        _state = AI_States.idle;

//        _characterAnimator.SetBool("Dead", false);

//        _characterAnimator.SetBool("HasEnemy", false);
//    }
//}

//private void StateDeath()
//{
//    // в будущем: блокирование способностей
//}


//private void StateAll()
//{
//    if (IsTargetAlive())
//    {
//        if (!IsCoverExist())
//            _currentCover = _coverManager.GetCover(this, _currentTarget);

//        if (IsCoverExist())
//        {
//            if (IsNotInCover())
//            {
//                _navMeshAgent.SetDestination(_currentCover.transform.position);            // действие move to cover
//            }
//            else
//            {
//                if (IsRangeDistance())
//                {
//                    _myTransform.LookAt(_currentTarget.transform);

//                    _characterAnimator.SetTrigger("Fire");

//                    _currentGun.Aim(_currentTarget.Eyes.position);

//                    _currentGun.Shoot(_currentTarget.Eyes.position); //  действие range combat;
//                }
//                else if (IsMeleeDistance())
//                {
//                    ExitCover();

//                    _characterAnimator.SetTrigger("Punch");  // действие melee combat
//                }
//            }
//        }
//        else
//        {
//            if (IsRangeDistance())
//            {
//                _myTransform.LookAt(_currentTarget.transform);

//                _characterAnimator.SetTrigger("Fire");

//                _currentGun.Aim(_currentTarget.Eyes.position);

//                _currentGun.Shoot(_currentTarget.Eyes.position); //  действие range combat;
//            }
//            else if (IsMeleeDistance())
//            {
//                ExitCover();

//                _characterAnimator.SetTrigger("Punch");  // действие melee combat
//            }
//        }
//    }
//    else
//    {
//        GetNewTarget();

//        if (IsTargetAlive())
//        {
//            return;
//        }
//        else
//        {
//            if (IsPlayerFar())
//            {
//                ExitCover();

//                _navMeshAgent.SetDestination(_followPoint.position);      // действие follow the player
//            }
//            else
//            {
//                _state = AI_States.idle;
//            }
//        }
//    }
//}

//private void StateIdle()
//{
//    if (IsTargetAlive())
//    {
//        if (!IsCoverExist())
//            _currentCover = _coverManager.GetCover(this, _currentTarget);

//        if (IsCoverExist())
//        {
//            if (IsNotInCover())
//            {
//                _state = AI_States.moveToCover;
//            }
//            else
//            {
//                if (IsRangeDistance())
//                {
//                    _state = AI_States.rangeCombat;
//                }
//                else if (IsMeleeDistance())
//                {
//                    _state = AI_States.meleeCombat;
//                }
//            }
//        }
//        else
//        {
//            if (IsRangeDistance())
//            {
//                _state = AI_States.rangeCombat;
//            }
//            else if (IsMeleeDistance())
//            {
//                _state = AI_States.meleeCombat;
//            }
//        }
//    }
//    else
//    {
//        GetNewTarget();

//        if (IsTargetAlive())
//        {
//            return;
//        }
//        else
//        {
//            if (IsPlayerFar())
//            {
//                _state = AI_States.followThePlayer;
//            }
//            else
//            {
//                _characterAnimator.SetBool("Move", false);

//                _state = AI_States.idle;
//            }
//        }
//    }
//}


//private void StateFollowThePlayer()
//{
//    if (IsTargetAlive())
//    {
//        if (!IsCoverExist())
//            _currentCover = _coverManager.GetCover(this, _currentTarget);

//        if (IsCoverExist())
//        {
//            if (IsNotInCover())
//            {
//                _state = AI_States.moveToCover;
//            }
//            else
//            {
//                if (IsRangeDistance())
//                {
//                    _state = AI_States.rangeCombat;
//                }
//                else if (IsMeleeDistance())
//                {
//                    _state = AI_States.meleeCombat;
//                }
//            }
//        }
//        else
//        {
//            if (IsRangeDistance())
//            {
//                _state = AI_States.rangeCombat;
//            }
//            else if (IsMeleeDistance())
//            {
//                _state = AI_States.meleeCombat;
//            }
//        }
//    }
//    else
//    {
//        GetNewTarget();

//        if (IsTargetAlive())
//        {
//            return;
//        }
//        else
//        {
//            if (IsPlayerFar())
//            {
//                ExitCover();

//                _characterAnimator.SetBool("Move", true);

//                _characterAnimator.SetBool("HasEnemy", false);

//                _navMeshAgent.SetDestination(_followPoint.position);      // действие follow the player
//            }
//            else
//            {
//                _state = AI_States.idle;
//            }
//        }
//    }
//}


//private void StateMoveToCover()
//{
//    if (IsTargetAlive())
//    {
//        if (!IsCoverExist())
//            _currentCover = _coverManager.GetCover(this, _currentTarget);

//        if (IsCoverExist())
//        {
//            if (IsNotInCover())
//            {
//                _characterAnimator.SetBool("Move", true);

//                _characterAnimator.SetBool("HasEnemy", true);

//                _navMeshAgent.SetDestination(_currentCover.transform.position);            // действие move to cover
//            }
//            else
//            {
//                if (IsRangeDistance())
//                {
//                    _state = AI_States.rangeCombat;
//                }
//                else if (IsMeleeDistance())
//                {
//                    _state = AI_States.meleeCombat;
//                }
//            }
//        }
//        else
//        {
//            if (IsRangeDistance())
//            {
//                _state = AI_States.rangeCombat;
//            }
//            else if (IsMeleeDistance())
//            {
//                _state = AI_States.meleeCombat;
//            }
//        }
//    }
//    else
//    {
//        GetNewTarget();

//        if (IsTargetAlive())
//        {
//            return;
//        }
//        else
//        {
//            if (IsPlayerFar())
//            {
//                _state = AI_States.followThePlayer;
//            }
//            else
//            {
//                _state = AI_States.idle;
//            }
//        }
//    }
//}


//private void StateRangeCombat()
//{
//    if (IsTargetAlive())
//    {
//        if (!IsCoverExist())
//            _currentCover = _coverManager.GetCover(this, _currentTarget);

//        if (IsCoverExist())
//        {
//            if (IsNotInCover())
//            {
//                _state = AI_States.moveToCover;
//            }
//            else
//            {
//                if (IsRangeDistance())
//                {
//                    _myTransform.LookAt(_currentTarget.transform);

//                    _characterAnimator.SetTrigger("Fire");

//                    _currentGun.Aim(_currentTarget.Eyes.position);

//                    _currentGun.Shoot(_currentTarget.Eyes.position); //  действие range combat
//                }
//                else if (IsMeleeDistance())
//                {
//                    _state = AI_States.meleeCombat;
//                }
//            }
//        }
//        else
//        {
//            if (IsRangeDistance())
//            {
//                _myTransform.LookAt(_currentTarget.transform);

//                _characterAnimator.SetTrigger("Fire");

//                _currentGun.Aim(_currentTarget.Eyes.position);

//                _currentGun.Shoot(_currentTarget.Eyes.position);   // действие range combat
//            }
//            else if (IsMeleeDistance())
//            {
//                _state = AI_States.meleeCombat;
//            }
//        }
//    }
//    else
//    {
//        GetNewTarget();

//        if (IsTargetAlive())
//        {
//            return;
//        }
//        else
//        {
//            if (IsPlayerFar())
//            {
//                _state = AI_States.followThePlayer;
//            }
//            else
//            {
//                _state = AI_States.idle;
//            }
//        }
//    }
//}


//private void StateMeleeCombat()
//{
//    if (IsTargetAlive())
//    {
//        if (!IsCoverExist())
//            _currentCover = _coverManager.GetCover(this, _currentTarget);

//        if (IsCoverExist())
//        {
//            if (IsNotInCover())
//            {
//                _state = AI_States.moveToCover;
//            }
//            else
//            {
//                if (IsRangeDistance())
//                {
//                    _state = AI_States.rangeCombat;
//                }
//                else if (IsMeleeDistance())
//                {
//                    ExitCover();

//                    _characterAnimator.SetTrigger("Punch");  // действие melee combat

//                    _currentGun.Punch();
//                }
//            }
//        }
//        else
//        {
//            if (IsRangeDistance())
//            {
//                _state = AI_States.rangeCombat;
//            }
//            else if (IsMeleeDistance())
//            {
//                ExitCover();

//                _characterAnimator.SetTrigger("Punch");   // действие melee combat

//                _currentGun.Punch();
//            }
//        }
//    }
//    else
//    {
//        GetNewTarget();

//        if (IsTargetAlive())
//        {
//            return;
//        }
//        else
//        {
//            if (IsPlayerFar())
//            {
//                _state = AI_States.followThePlayer;
//            }
//            else
//            {
//                _state = AI_States.idle;
//            }
//        }
//    }
//}


//private Team GetNewTarget()
//{
//    Team _bestTarget = null;

//    for (int i = 0; i < _allCharacters.Length; i++)
//    {
//        Team _currentCharacter = _allCharacters[i];

//        //выбирать текущего персонажа в качестве цели, только если мы не в одной команде и если у него осталось здоровье
//        if (_currentCharacter != null
//            && _currentCharacter.GetComponent<Team>().GetTeamNumber() != MyTeam.GetTeamNumber()
//            && _currentCharacter.GetComponent<Vitals>().IsAlive()
//            && Vector3.Distance(_myTransform.position, _currentCharacter.transform.position) <= _maxAttackDistance)
//        {
//            //если цель видно
//            if (CanSeeTarget(_currentCharacter))
//            {
//                if (_bestTarget == null)
//                {
//                    _bestTarget = _currentCharacter;
//                }
//                else
//                {
//                    //если текущая цель ближе, чем лучшая цель, то выбрать текущую цель 
//                    if (Vector3.Distance(_currentCharacter.transform.position, _myTransform.position) < Vector3.Distance(_bestTarget.transform.position, _myTransform.position))
//                    {
//                        _bestTarget = _currentCharacter;
//                    }
//                }
//            }
//        }
//    }

//    return _bestTarget;
//}


//private bool CanSeeTarget(Team _target)
//{
//    bool _canSeeIt = false;

//    Vector3 _enemyPosition = _target.Eyes.position;

//    Vector3 _directionTowardsEnemy = _enemyPosition - Eyes.position;

//    RaycastHit _hit;

//    //направить луч на текущего врага
//    if (Physics.Raycast(Eyes.position, _directionTowardsEnemy, out _hit, Mathf.Infinity))
//    {
//        //если рейкаст попал в цель, то мы знаем, что можем его увидеть
//        if (_hit.transform == _target.transform)
//        {
//            _canSeeIt = true;
//        }
//    }

//    return _canSeeIt;
//}


//private bool IsTargetAlive()
//{
//    if (_currentTarget != null
//        && _currentTarget.GetComponent<Vitals>().IsAlive())
//    {
//        return true;
//    }
//    else
//    {
//        return false;
//    }
//}


//private bool IsRangeDistance()
//{
//    if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _maxAttackDistance
//            && Vector3.Distance(_myTransform.position, _currentTarget.transform.position) >= _minAttackDistance)
//    {
//        return true;
//    }
//    else
//    {
//        return false;
//    }
//}


//private bool IsMeleeDistance()
//{
//    if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _minAttackDistance)
//    {
//        return true;
//    }
//    else
//    {
//        return false;
//    }
//}


//private bool IsPlayerFar()
//{
//    if (Vector3.Distance(_myTransform.position, _player.transform.position) > 3f)
//    {
//        return true;
//    }
//    else
//    {
//        return false;
//    }
//}


//private void ExitCover()
//{
//    if (_currentCover != null)
//        _coverManager.ExitCover(ref _currentCover);
//}


//private bool IsNotInCover()
//{
//    if (Vector3.Distance(_myTransform.position, _currentCover.transform.position) > 0.2F)
//    {
//        return true;
//    }
//    else
//    {
//        return false;
//    }
//}


//private bool IsCoverExist()
//{
//    if (_currentCover != null)
//    {
//        return true;
//    }
//    else
//    {
//        return false;
//    }
//}
//}
