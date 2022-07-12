using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]
public class DroneBehavior : MonoBehaviour
{
    [HideInInspector]
    public Team MyTeam;
    [HideInInspector]
    public Vitals MyVitals;
    public Transform Eyes;

    private NavMeshAgent _navMeshAgent;
    private Transform _myTransform;

    [SerializeField] // ����
    private Team _currentTarget;
    [SerializeField]
    private Gun _currentGun;
    [SerializeField]
    private float _minAttackDistance = 2;
    [SerializeField]
    private float _maxAttackDistance = 5;
    [SerializeField]
    private float _moveSpeed = 3.2f;
    [SerializeField]
    private float _fireCooldown = 0.5f;
    private float _currentFireCooldown = 0;
    private Team[] _allCharacters;


    public enum AI_States
    {
        idle,
        moveToTarget,
        rangeCombat,
        death
    }


    public AI_States _state = AI_States.idle;


    private void Start()
    {

        _myTransform = transform;

        MyTeam = GetComponent<Team>();

        MyVitals = GetComponent<Vitals>();

        _navMeshAgent = GetComponent<NavMeshAgent>();
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
                case AI_States.rangeCombat:
                    StateRangeCombat();
                    break;
                case AI_States.moveToTarget:
                    StateMoveToTarget();
                    break;
                default:
                    break;
            }
        }
        else
        {
            _state = AI_States.death;

            Destroy(gameObject, 3f);
        }
    }


    private void StateIdle()
    {
        if (_currentTarget != null) // ���� ���� ����
        {
            if (_currentTarget.GetComponent<Vitals>().GetCurrentHealth() > 0) // ���� ���� ����
            {
                if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _maxAttackDistance 
                  && Vector3.Distance(_myTransform.position, _currentTarget.transform.position) >= _minAttackDistance)
                {
                    _state = AI_States.rangeCombat;
                }
                else
                {
                    _state = AI_States.moveToTarget;
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
        else // ���� ���� ���
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


    private void StateMoveToTarget()
    {
        if (_currentTarget != null 
            && _currentTarget.GetComponent<Vitals>().GetCurrentHealth() > 0)
        {
            if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) >= _maxAttackDistance)
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
        if (_currentTarget != null) // ���� ���� ����
        {
            if (_currentTarget.GetComponent<Vitals>().GetCurrentHealth() > 0) // ���� ���� ����
            {
                if (!CanSeeTarget(_currentTarget)) // ���� ���� ������� �� ���� ���������
                {
                    Team _alternativeTarget = GetNewTarget(); // ���� �������������� ����

                    if (_alternativeTarget == null) // ���� �������������� ���� ���
                    {
                        _state = AI_States.idle;
                    }
                    else // ���� �������������� ���� ���� - ��� ���������� ��������
                    {
                        _currentTarget = _alternativeTarget;
                    }
                    return;
                }

                _myTransform.LookAt(_currentTarget.transform); // ������� �� ����

                // ���� ��������� ��� ����� ����������
                if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _maxAttackDistance
                    && Vector3.Distance(_myTransform.position, _currentTarget.transform.position) >= _minAttackDistance)
                {
                    // �������
                    if (_currentFireCooldown <= 0)
                    {
                        _currentGun.Aim(_currentTarget.Eyes.position);

                        _currentGun.Shoot(_currentTarget.Eyes.position);

                        _currentFireCooldown = _fireCooldown;
                    }
                    else
                    {
                        _currentFireCooldown -= 1 * Time.deltaTime;
                    }
                }
                else // ���� ��������� �� ����������, �������� ������ 
                {
                    _state = AI_States.idle;
                }
            }
            else // ���� ���� ������, �������� ������
            {
                _state = AI_States.idle;
            }
        }
        else // ���� ���� ���, �������� ������
        {
            _state = AI_States.idle;
        }
    }


    private Team GetNewTarget()
    {
        _allCharacters = GameObject.FindObjectsOfType<Team>(); // ������ ���� � GetTarget

        Team _bestTarget = null;

        for (int i = 0; i < _allCharacters.Length; i++)
        {
            Team _currentCharacter = _allCharacters[i];

            if (_currentCharacter.GetComponent<Team>().GetTeamNumber() == MyTeam.GetTeamNumber()
                && _currentCharacter.GetComponent<Vitals>().GetCurrentHealth() > 0)
            {
                //���� ������� ����� � ����, �� �� �����, ��� ����� ��� �������
                if (CanSeeTarget(_currentCharacter))
                {
                    if (_bestTarget == null)
                    {
                        _bestTarget = _currentCharacter;
                    }
                    else
                    {
                        //���� ������� ���� �����, ��� ������ ����, �� ������� ������� ���� 
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
