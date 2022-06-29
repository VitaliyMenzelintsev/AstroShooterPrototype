using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CompanionRangeAI : CharacterBase
{
    [HideInInspector]
    public GameObject[] BadEntities;
    [HideInInspector]
    public GameObject[] CoverSpots;
    //[HideInInspector]
    public Transform FollowDestination;
    //[HideInInspector]
    public GameObject NearestEnemy;
    //[HideInInspector]
    public GameObject NearestCoverSpot;

    public Transform FollowPoint;    // в инспекторе задать точку около игрока
    public Transform Player;
    public Transform Eyes;

    public Vitals MyVitals;
    [SerializeField]
    private float _minAttackDistance = 7f;
    [SerializeField]
    private float _maxAttackDistance = 13f;
    [SerializeField]
    private float _damageDealt = 15f;
    [SerializeField]
    private float _fireCooldown = 1f;
    [SerializeField]
    private float _coverChangeCooldown = 5f;
    private float _currentCoverChangeCooldown;
    private float _currentFireCooldown = 0f;


    private NavMeshAgent _navMeshAI;
    private Animator _characterAnimator;
    private CharacterBase _targetEntity;
    private Transform _myTransform;
    private bool _hasEnemy;

    public enum State { Idle, Move, Attack, Death }
    public State _currentState;

    // СДЕЛАТЬ АНИМАТОР 


    private void Awake()
    {
        _myTransform = transform;

        _navMeshAI = GetComponent<NavMeshAgent>();

        _characterAnimator = GetComponent<Animator>();

        FollowDestination = FollowPoint;

        _targetEntity = FollowDestination.GetComponent<CharacterBase>();
    }


    protected override void Start()
    {
        base.Start();

        StartCoroutine(FindEnemy());

        StartCoroutine(FindCover());

        StartCoroutine(TargetChanger());

        //_characterAnimator.SetBool("Move", true);
        _currentState = State.Move;

        _currentCoverChangeCooldown = _coverChangeCooldown;

    }


    private void Update()
    {
        if (MyVitals.GetCurrentHealth() > 0)
        {
            switch (_currentState)
            {
                case State.Idle:
                    StateIdle();
                    break;
                case State.Move:
                    StateMove();
                    break;
                case State.Attack:
                    StateAttack();
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

            //if (_currentCover != null)
            //{
            //    _coverManager.ExitCover(_currentCover);
            //}

            _currentState = State.Death;

            Destroy(gameObject, 25f);
        }
    }


    private void StateAttack()
    {
        if (NearestEnemy != null
            && NearestEnemy.GetComponent<Vitals>().GetCurrentHealth() > 0)
        {
            if (!CanSeeEnemy(NearestEnemy))
            {
                GameObject _alternativeTarget = NearestEnemy;

                NearestEnemy = _alternativeTarget;

                return;
            }

            _myTransform.LookAt(NearestEnemy.transform);

            if (Vector3.Distance(_myTransform.position, NearestEnemy.transform.position) <= _maxAttackDistance
                && Vector3.Distance(_myTransform.position, NearestEnemy.transform.position) >= _minAttackDistance)
            {
                //attack
                if (_currentFireCooldown <= 0)
                {
                    _characterAnimator.SetBool("Move", false);

                    _characterAnimator.SetTrigger("Fire");

                    NearestEnemy.GetComponent<Vitals>().GetHit(_damageDealt);

                    _currentFireCooldown = _fireCooldown;
                }
                else
                {
                    _currentFireCooldown -= 1 * Time.deltaTime;
                }
            }
            else
            {
                if (_currentCoverChangeCooldown <= 0)    // Смена укрытий
                {
                    _currentCoverChangeCooldown = _coverChangeCooldown;

                    _characterAnimator.SetBool("Move", false);
                    _currentState = State.Idle;
                }
                else
                {
                    _currentCoverChangeCooldown -= 1 * Time.deltaTime;
                }
            }
        }
        else
        {
            _characterAnimator.SetBool("Move", false);
            _currentState = State.Idle;
        }
    }


    private void StateMove()  
    {
        _characterAnimator.SetBool("Move", true);
        _navMeshAI.SetDestination(FollowDestination.position);

        if (Vector3.Distance(FollowDestination.position, _myTransform.position) < 1.4f)
        {
            if (_hasEnemy == true)
            {
                _currentState = State.Attack;
            }
            else
            {
                if (Vector3.Distance(Player.position, _myTransform.position) > 3f)
                {
                    _navMeshAI.SetDestination(FollowDestination.position);
                }

                else
                {
                    _characterAnimator.SetBool("Move", false);
                    _currentState = State.Idle;
                }
            }
        }
    }


    private void StateIdle()
    {
        if(Vector3.Distance(Player.position, transform.position) > 3f)
        {
            //_characterAnimator.SetBool("Move", true);
            _currentState = State.Move;
        }

        if(_hasEnemy == true)
        {
            _characterAnimator.SetBool("Move", false);
            //_characterAnimator.SetTrigger("Fire");
            _currentState = State.Attack;
        }
        
    }


    IEnumerator FindCover() // НЕ РАБОТАЕТ
    {
        float _refreshRate = .5f;

        CoverSpots = GameObject.FindGameObjectsWithTag("Vault");

        if (CoverSpots != null)
        {
            GameObject _nearestCoverSpot = null;

            for (int i = 0; i < CoverSpots.Length; i++)
            {
                float _minDistance = Vector3.Distance(CoverSpots[i].transform.position, transform.position);

                float _alternativeDistance = Vector3.Distance(CoverSpots[i+1].transform.position, transform.position);

                if (_alternativeDistance < _minDistance)
                {
                    _nearestCoverSpot = CoverSpots[i+1];
                }
                else
                {
                    _nearestCoverSpot = CoverSpots[i];
                }
                NearestCoverSpot = _nearestCoverSpot;
            }
        }
        else
        {
            NearestCoverSpot = null;
        }

        yield return new WaitForSeconds(_refreshRate);
    }


    IEnumerator FindEnemy()
    {
        float _refreshRate = .5f;

        BadEntities = GameObject.FindGameObjectsWithTag("BadEntity");  // возможно придётся наполнять через лист(точно работает)

        if (BadEntities != null)
        {
            GameObject _nearestBadEntity = BadEntities[0];

            for (int i = 1; i < BadEntities.Length; i++)
            {
                float _distance = Vector3.Distance(_nearestBadEntity.transform.position, transform.position);

                float _anotherDistance = Vector3.Distance(BadEntities[i].transform.position, transform.position);

                if (_anotherDistance < _distance)
                {
                    _nearestBadEntity = BadEntities[i];
                }
                else
                {
                    _nearestBadEntity = BadEntities[0];
                }
            }

            NearestEnemy = _nearestBadEntity;

            _hasEnemy = true;
        }
        else
        {
            NearestEnemy = null;

            _hasEnemy = false;
        }

        yield return new WaitForSeconds(_refreshRate);
    }


    IEnumerator TargetChanger()
    {
        float _refreshRate = .5f;

        if (_hasEnemy)
        {
            FollowDestination = NearestCoverSpot.transform; // для милишника цель - враг
        }
        else
        {
            FollowDestination = FollowPoint;
        }
        yield return new WaitForSeconds(_refreshRate);
    }


    private bool CanSeeEnemy(GameObject _target) // если NPC из глаз видит ближайшего противника, то true
    {
        bool _canSeeIt = false;

        Vector3 _enemyPosition = _target.transform.position;

        Vector3 _directionTowardsEnemy = _enemyPosition - Eyes.position;

        RaycastHit _hit;

        if (Physics.Raycast(Eyes.position, _directionTowardsEnemy, out _hit, Mathf.Infinity))
        {
            if (_hit.transform == _target.transform)
            {
                _canSeeIt = true;
            }
        }

        return _canSeeIt;
    }
}

