using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]
[RequireComponent(typeof(Animator))]

public class EnemyMeleeBehavior : EnemyBehavior
{
    private NavMeshAgent _navMeshAgent;
    private Animator _characterAnimator;
 
    //[SerializeField]
    //private float _fireCooldown = 2.667f;


    public AI_States _state = AI_States.idle;

    private void Start()
    {
        _allCharacters = GameObject.FindObjectsOfType<Team>();

        _myTransform = transform;

        MyTeam = GetComponent<Team>();

        MyVitals = GetComponent<Vitals>();

        _navMeshAgent = GetComponent<NavMeshAgent>();

        _characterAnimator = GetComponent<Animator>();
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
                case AI_States.meleeCombat:
                    StateCombat();
                    break;
                case AI_States.investigate:
                    StateInvestigate();
                    break;
                case AI_States.death:
                    break;
                default:
                    StateIdle();
                    break;
            }
        }
        else
        {
            _characterAnimator.SetBool("Move", false);

            _characterAnimator.SetBool("Dead", true);

            Destroy(GetComponent<CapsuleCollider>());

            Destroy(GetComponent<NavMeshAgent>());

            _state = AI_States.death;

            Destroy(gameObject, 7f);
        }
    }


    private void StateIdle()
    {
        if (IsTargetAlive())
        {
            if (IsDistanceCorrect())
            {
                _state = AI_States.meleeCombat;
            }
            else
            {
                _state = AI_States.investigate;
            }
        }
        else
        {
            //���� ����� ����
            Team _bestTarget = GetNewTarget();

            if (_bestTarget != null)
            {
                _currentTarget = _bestTarget;
            }
        }
    }

    private void StateCombat()
    {
        if (IsTargetAlive())
        {
            if (IsDistanceCorrect())
            {
                // �����
                {
                    _myTransform.LookAt(_currentTarget.transform);

                    _characterAnimator.SetTrigger("Fire");

                    _currentGun.Shoot(_currentTarget.Eyes.position);
                }
            }

             if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) > _maxAttackDistance)
            {
                _state = AI_States.investigate;
            }
        }
        else
        {
            _characterAnimator.SetBool("Move", false);

            _state = AI_States.idle;
        }
    }


    private void StateInvestigate()
    {
        if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _maxAttackDistance)
        {
            _characterAnimator.SetBool("Move", false);

            _state = AI_States.meleeCombat;
        }
        else
        {
            _characterAnimator.SetBool("Move", true);

            _navMeshAgent.SetDestination(_currentTarget.transform.position);
        }

        if (_currentTarget == null)
        {
            _characterAnimator.SetBool("Move", false);

            _state = AI_States.idle;
        }
    }


    private Team GetNewTarget()
    {
        Team _bestTarget = null;

        for (int i = 0; i < _allCharacters.Length; i++)
        {
            Team _currentCharacter = _allCharacters[i];

            //�������� �������� ���������� � �������� ����, ������ ���� �� �� � ����� ������� � ���� � ���� �������� ��������
            if (_currentCharacter.GetComponent<Team>().GetTeamNumber() != MyTeam.GetTeamNumber()
                && _currentCharacter.GetComponent<Vitals>().IsAlive())
            {
                //���� ������� ����� � ����, �� �����, ��� ����� �
                if (CanSeeTarget(_currentCharacter))
                {
                    if (_bestTarget == null)
                    {
                        _bestTarget = _currentCharacter;
                    }
                    else
                    {
                        //���� ������� ���� �����, ��� ������ ����, �� ������� �������� ����� � �������� ������ ����
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

        Vector3 _enemyPosition = _target.Eyes.position; ;    // _target.Eyes.position; ����� ��� �����

        Vector3 _directionTowardsEnemy = _enemyPosition - Eyes.position;

        RaycastHit _hit;

        //��������� ��� �� �������� �����
        if (Physics.Raycast(Eyes.position, _directionTowardsEnemy, out _hit, Mathf.Infinity))
        {
            //���� ������� ����� � ����, �� �� �����, ��� ����� ��� �������
            if (_hit.transform == _target.transform)
            {
                _canSeeIt = true;
            }
        }

        return _canSeeIt;
    }
}
