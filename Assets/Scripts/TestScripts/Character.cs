using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    [HideInInspector]
    public Team MyTeam;
    [HideInInspector]
    public Vitals MyVitals;
    public Transform Eyes;
    
    private Transform _myTransform;
    private Animator _characterAnimator;
    private CoverManager _coverManager;
    private Character _curTarget;

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
    private float _currentFireCooldown = 0;

    private Vector3 _targetLastKnownPosition;
    private Path _currentPath = null;
    private CoverSpot _currentCover = null;
    private float _coverChangeCooldown = 5;
    private float _currentCoverChangeCooldown;

    public enum AI_States 
    {
        idle,
        moveToCover,
        combat,
        investigate
    }
    public AI_States state = AI_States.idle;

    private void Start()
    {
        _myTransform = transform;
        MyTeam = GetComponent<Team>();
        MyVitals = GetComponent<Vitals>();
        _characterAnimator = GetComponent<Animator>();

        _coverManager = GameObject.FindObjectOfType<CoverManager>();
        _currentCoverChangeCooldown = _coverChangeCooldown;
    }

    private void Update()
    {
        if (MyVitals.GetCurHealth() > 0)
        {
            switch (state)
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
                case AI_States.investigate:
                    StateInvestigate();
                    break;
                default:
                    break;
            }
        }
        else
        {
            _characterAnimator.SetBool("move", false);

            //to be able to investigate last known position of dead soldiers, we'll need to implement dead, instead of destroying the soldier gameobject
            if (GetComponent<CapsuleCollider>() != null)
            {
                Destroy(GetComponent<CapsuleCollider>());
            }

            if (_currentCover != null)
            {
                _coverManager.ExitCover(_currentCover);
            }

            Quaternion _deathRotation = Quaternion.Euler(90, _myTransform.rotation.eulerAngles.y, _myTransform.rotation.eulerAngles.z);
            if (_myTransform.rotation != _deathRotation)
            {
                _myTransform.rotation = _deathRotation; //maybe it will work?
            }
        }
    }

    private void StateIdle()
    {
        if (_curTarget != null && _curTarget.GetComponent<Vitals>().GetCurHealth() > 0)
        {
            if (_currentCover != null)
            {
                _coverManager.ExitCover(_currentCover);
            }

            _currentCover = _coverManager.GetCoverTowardsTarget(this, _curTarget.transform.position, _maxAttackDistance, _minAttackDistance, _currentCover);

            if (_currentCover != null)
            {
                if (Vector3.Distance(_myTransform.position, _currentCover.transform.position) > 0.2F)
                {
                    _currentPath = CalculatePath(_myTransform.position, _currentCover.transform.position);

                    _characterAnimator.SetBool("move", true);

                    state = AI_States.moveToCover;
                }
                else
                {
                    state = AI_States.combat;
                }
            }
            else
            {
                if (Vector3.Distance(_myTransform.position, _curTarget.transform.position) <= _maxAttackDistance && Vector3.Distance(_myTransform.position, _curTarget.transform.position) >= _minAttackDistance)
                {
                    //attack
                    state = AI_States.combat;
                }
            }
        }
        else
        {
            //find new target
            Character _bestTarget = GetNewTarget();

            if (_bestTarget != null)
            {
                _curTarget = _bestTarget;
            }
        }
    }
    private void StateMoveToCover()
    {
        if (_curTarget != null && _currentCover != null && _currentCover.AmICoveredFrom(_curTarget.transform.position))
        {
            if (_currentPath != null)
            {
                Character _alternativeTarget = GetNewTarget();

                if (_alternativeTarget != null && _alternativeTarget != _curTarget)
                {
                    float _distanceToCurTarget = Vector3.Distance(_myTransform.position, _curTarget.transform.position);
                    float _distanceToAlternativeTarget = Vector3.Distance(_myTransform.position, _alternativeTarget.transform.position);
                    float _distanceBetweenTargets = Vector3.Distance(_curTarget.transform.position, _alternativeTarget.transform.position);

                    if (Mathf.Abs(_distanceToAlternativeTarget - _distanceToCurTarget) > 5 && _distanceBetweenTargets > 5)
                    {
                        _curTarget = _alternativeTarget;
                        _coverManager.ExitCover(_currentCover);
                        _currentCover = _coverManager.GetCoverTowardsTarget(this, _curTarget.transform.position, _maxAttackDistance, _minAttackDistance, _currentCover);
                        _currentPath = CalculatePath(_myTransform.position, _currentCover.transform.position);
                        return;
                    }
                }

                if (_currentPath.ReachedEndNode())
                { //if we reached the end, we'll start looking for a target
                    _characterAnimator.SetBool("move", false);

                    _currentPath = null;

                    state = AI_States.combat;
                    return;
                }

                Vector3 _nodePosition = _currentPath.GetNextNode();

                if (Vector3.Distance(_myTransform.position, _nodePosition) < 1)
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

                state = AI_States.idle;
            }
        }
        else
        {
            _characterAnimator.SetBool("move", false);

            state = AI_States.idle;
        }
    }

    private void StateCombat()
    {
        if (_curTarget != null && _curTarget.GetComponent<Vitals>().GetCurHealth() > 0)
        {
            //if the target escapes during combat
            if (!CanISeeTheTarget(_curTarget))
            {
                Character _alternativeTarget = GetNewTarget();

                if (_alternativeTarget == null)
                {
                    //If I can't see the target anymore, I'll need to Investigate last known position
                    _targetLastKnownPosition = _curTarget.transform.position;

                    //we'll need to calculate a path towards the target's last known position and we'll do so using the Unity NavMesh combined with some custom code
                    _currentPath = CalculatePath(_myTransform.position, _targetLastKnownPosition);
                    _characterAnimator.SetBool("move", true);

                    if (_currentCover != null)
                    {
                        _coverManager.ExitCover(_currentCover);
                    }
                    state = AI_States.investigate;
                }
                else
                {
                    _curTarget = _alternativeTarget;
                }
                return;
            }

            _myTransform.LookAt(_curTarget.transform);

            if (Vector3.Distance(_myTransform.position, _curTarget.transform.position) <= _maxAttackDistance 
                && Vector3.Distance(_myTransform.position, _curTarget.transform.position) >= _minAttackDistance)
            {
                //attack
                if (_currentFireCooldown <= 0)
                {
                    _characterAnimator.SetTrigger("fire");

                    _curTarget.GetComponent<Vitals>().GetHit(_damageDealt);

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

                    state = AI_States.idle;
                }
                else
                {
                    _currentCoverChangeCooldown -= 1 * Time.deltaTime;
                }
            }
        }
        else
        {
            state = AI_States.idle;
        }
    }

    private void StateInvestigate()
    {
        if (_currentPath != null)
        {
            Character _alternativeTarget = GetNewTarget();

            if (_currentPath.ReachedEndNode() || _alternativeTarget != null)
            { //if we reached the end, we'll start looking for a target
                _characterAnimator.SetBool("move", false);

                _currentPath = null;
                _curTarget = _alternativeTarget;

                state = AI_States.idle;
                return;
            }

            Vector3 _nodePosition = _currentPath.GetNextNode();

            if (Vector3.Distance(_myTransform.position, _nodePosition) < 1)
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

            _currentPath = null;
            _curTarget = null;

            state = AI_States.idle;
        }
    }

    Character GetNewTarget()
    {
        Character[] _allCharacters = GameObject.FindObjectsOfType<Character>();
        Character _bestTarget = null;
        for (int i = 0; i < _allCharacters.Length; i++)
        {
            Character _currentCharacter = _allCharacters[i];

            //only select current soldier as target, if we are not on the same team and if it got health left
            if (_currentCharacter.GetComponent<Team>().GetTeamNumber() != MyTeam.GetTeamNumber() && _currentCharacter.GetComponent<Vitals>().GetCurHealth() > 0)
            {
                //if the raycast hit the target, then we know that we can see it
                if (CanISeeTheTarget(_currentCharacter))
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

    private bool CanISeeTheTarget(Character _target)
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

    Path CalculatePath(Vector3 _source, Vector3 _destination)
    {
        NavMeshPath _navMeshPath = new NavMeshPath();
        NavMesh.CalculatePath(_source, _destination, NavMesh.AllAreas, _navMeshPath); //calculates a path using the Unity NavMesh

        Path path = new Path(_navMeshPath.corners);

        return path;
    }
}