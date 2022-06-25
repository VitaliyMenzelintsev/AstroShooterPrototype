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
    [HideInInspector]
    public Transform FollowDestination;
    public Transform FollowPoint;    // в инспекторе задать точку около игрока
    public Transform Player;

    public GameObject NearestEnemy;
    public GameObject NearestCoverSpot;
    public NavMeshAgent NavMeshAI;


    public enum State { Idle, Move, Attack /* Attack = Cover, атакуем только из укрытия */ }
    private State _currentState;

    private CharacterBase _targetEntity;
    private bool _hasEnemy;


    // ДОБАВИТЬ СОСТОЯНИЕ АТАКИ


    private void Awake()
    {
        NavMeshAI = GetComponent<NavMeshAgent>();

        FollowDestination = GameObject.FindGameObjectWithTag("Player").transform;

        _targetEntity = FollowDestination.GetComponent<CharacterBase>();
    }


    protected override void Start()
    {
        base.Start();

        StartCoroutine(MoveTo());

        StartCoroutine(FindEnemy());

        StartCoroutine(FindCover());

        _currentState = State.Move;
        _targetEntity.OnDeath += OnTargetDeath;
    }


    private void Update()
    {
        TargetChanger();

        // смотреть за курсором
    }


    private void OnTargetDeath()
    {
        _hasEnemy = false;
        _currentState = State.Idle;
    }


    private void TargetChanger()
    {
        if (_hasEnemy)
        {
            FollowDestination = NearestCoverSpot.transform; // для милишника цель - враг
        }
        else
        {
            FollowDestination = FollowPoint;
        }
    }


    private void StateAttack()
    {
    //    if (_currentTarget != null
    //&& _currentTarget.GetComponent<Vitals>().GetCurrentHealth() > 0)
    //    {
    //        //if the target escapes during combat
    //        if (!CanSeeTarget(_currentTarget))
    //        {
    //            EnemyRangeBehavior _alternativeTarget = GetNewTarget();

    //            _currentTarget = _alternativeTarget;

    //            return;
    //        }

    //        _myTransform.LookAt(_currentTarget.transform);

    //        if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _maxAttackDistance
    //            && Vector3.Distance(_myTransform.position, _currentTarget.transform.position) >= _minAttackDistance)
    //        {
    //            //attack
    //            if (_currentFireCooldown <= 0)
    //            {
    //                _characterAnimator.SetTrigger("fire");

    //                _currentTarget.GetComponent<Vitals>().GetHit(_damageDealt);

    //                _currentFireCooldown = _fireCooldown;
    //            }
    //            else
    //            {
    //                _currentFireCooldown -= 1 * Time.deltaTime;
    //            }
    //        }
    //        else
    //        {
    //            if (_currentCoverChangeCooldown <= 0)    // СМЕНА УКРЫТИЙ ТУТ
    //            {
    //                _currentCoverChangeCooldown = _coverChangeCooldown;

    //                _characterAnimator.SetBool("move", false);

    //                _state = AI_States.idle;
    //            }
    //            else
    //            {
    //                _currentCoverChangeCooldown -= 1 * Time.deltaTime;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        _state = AI_States.idle;
    //    }
    }

    IEnumerator FindCover()
    {
        float _refreshRate = .5f;

        CoverSpots = GameObject.FindGameObjectsWithTag("Vault");

        if (CoverSpots != null)
        {
            GameObject _nearestCoverSpot = CoverSpots[0];

            for (int i = 1; i < CoverSpots.Length; i++)
            {
                float _distance = Vector3.Distance(_nearestCoverSpot.transform.position, transform.position);

                float _anotherDistance = Vector3.Distance(CoverSpots[i].transform.position, transform.position);

                if (_anotherDistance < _distance)
                {
                    _nearestCoverSpot = CoverSpots[i];
                }
                else
                {
                    _nearestCoverSpot = CoverSpots[0];
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


    IEnumerator MoveTo()  // в зависимости от условий подставляем сюда либо место ближайшего укрытия, либо точку около игрока
    {
        float _refreshRate = .25f;

        if (_currentState == State.Move)
        {
            //Vector3 directionToTarget = (Target.position - transform.position).normalized;
            //Vector3 targetPosition = Target.position - directionToTarget * (_myCollisionRadius * _targetCollisionRadius + _attackDistance / 2);

            if (!dead)
            {
                NavMeshAI.SetDestination(FollowDestination.position);
                if (Vector3.Distance(FollowDestination.position, transform.position) < 0.2f)
                {
                    if (_hasEnemy == true)
                    {
                        _currentState = State.Attack;
                        StateAttack();
                    }
                    else
                    {
                        _currentState = State.Idle;
                    }
                }
            }
        }
        yield return new WaitForSeconds(_refreshRate);

    }
}

