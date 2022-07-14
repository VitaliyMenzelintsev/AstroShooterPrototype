using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]

public class EnemyDroneBehavior : EnemyBehavior
{
    private NavMeshAgent _navMeshAgent;
   
    public AI_States _state = AI_States.idle;


    private void Start()
    {
        _allCharacters = GameObject.FindObjectsOfType<Team>();

        _myTransform = transform;

        MyTeam = GetComponent<Team>();

        MyVitals = GetComponent<Vitals>();

        _navMeshAgent = GetComponent<NavMeshAgent>();
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
                case AI_States.rangeCombat:
                    StateRangeCombat();
                    break;
                case AI_States.investigate:
                    StateInvestigate();
                    break;
            }
        }
        else
        {
            _state = AI_States.death;

            Destroy(GetComponent<CapsuleCollider>());

            Destroy(gameObject, 2f);
        }
    }


    private void StateIdle()
    {
        if (IsTargetAlive()) // если цель жива
        {
            if (IsDistanceCorrect())
            {
                _state = AI_States.rangeCombat;
            }
            else
            {
                _state = AI_States.investigate;
            }
        }
        else // если цели нет или она мертва
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


    private void StateInvestigate()
    {
        if (IsTargetAlive())
        {
            if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) > _maxAttackDistance)
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
        _myTransform.LookAt(_currentTarget.transform); // смотрим на цель

        if (IsTargetAlive() // если цель жива
          && IsDistanceCorrect())
        {
            // атакуем

            _currentGun.Aim(_currentTarget.Eyes.position);

            _currentGun.Shoot(_currentTarget.Eyes.position);
        }
        else // если дистанция не подходящая, начинаем стоять 
        {
            _state = AI_States.idle;
        }
    }


    private Team GetNewTarget()
    {
        Team _bestTarget = null;

        for (int i = 0; i < _allCharacters.Length; i++)
        {
            Team _currentCharacter = _allCharacters[i];

            if (_currentCharacter.GetComponent<Team>().GetTeamNumber() == MyTeam.GetTeamNumber()
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
