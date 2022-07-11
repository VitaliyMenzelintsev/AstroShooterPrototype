using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]
[RequireComponent(typeof(Animator))]

public class EnemyRangeBehavior : MonoBehaviour
{
    [HideInInspector]
    public Team MyTeam;
    [HideInInspector]
    public Vitals MyVitals;
    public Transform Eyes;

    private NavMeshAgent _navMeshAgent;
    private Transform _myTransform;
    private Animator _characterAnimator;
    private EnemyCoverManager _coverManager;

    [SerializeField] // ����
    private Team _currentTarget;
    [SerializeField]
    private Gun _currentGun;
    [SerializeField]
    private float _minAttackDistance = 5;
    [SerializeField]
    private float _maxAttackDistance = 13;
    [SerializeField]
    private float _punchDistance = 1;
    [SerializeField]
    private float _moveSpeed = 3.2f;
    [SerializeField]
    private float _fireCooldown = 0.5f;
    private float _currentFireCooldown = 0;
    [SerializeField]
    private EnemyCoverSpot _currentCover = null;
    private Team[] _allCharacters;


    public enum AI_States
    {
        idle,
        moveToCover,
        rangeCombat,
        meleeCombat,
        death
    }


    public AI_States _state = AI_States.idle;

    private void Start()
    {
        _allCharacters = GameObject.FindObjectsOfType<Team>();

        _myTransform = transform;

        MyTeam = GetComponent<Team>();

        MyVitals = GetComponent<Vitals>();

        _navMeshAgent = GetComponent<NavMeshAgent>();

        _characterAnimator = GetComponent<Animator>();

        _coverManager = GameObject.FindObjectOfType<EnemyCoverManager>();
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
                case AI_States.moveToCover:
                    StateMoveToCover();
                    break;
                case AI_States.rangeCombat:
                    StateRangeCombat();
                    break;
                case AI_States.meleeCombat:
                    StateMeleeCombat();
                    break;
                default:
                    break;
            }
        }
        else
        {
            _characterAnimator.SetBool("Move", false); // ������������ ��������� ������

            Destroy(GetComponent<CapsuleCollider>());

            Destroy(GetComponent<Team>());

            _characterAnimator.SetBool("Dead", true);

            if (_currentCover != null)
            {
                _coverManager.ExitCover(_currentCover);
            }

            _state = AI_States.death;

            Destroy(gameObject, 10f);
        }
    }


    private void StateIdle()
    {
        if (_currentTarget != null) // ���� ���� ����
        {
            if (_currentTarget.GetComponent<Vitals>().GetCurrentHealth() > 0) // ���� ���� ����
            {
                if (_currentCover != null) // ���� ���������� �������
                {
                    if (Vector3.Distance(_myTransform.position, _currentCover.transform.position) > 0.2F) // ���� ���������� �� ������� ������ 20 ��.
                    {
                        _characterAnimator.SetBool("Move", true);    // ��������� � ����
                        _characterAnimator.SetBool("HasEnemy", true);
                        _state = AI_States.moveToCover;
                    }
                    else // ���� �������� ��� � ������� (< 0.2f �� �������)
                    {
                        if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _maxAttackDistance // �������� �� ��������� �� � ���� �� ����: ���� ������, ���� ��������� ��������
                          && Vector3.Distance(_myTransform.position, _currentTarget.transform.position) >= _minAttackDistance)
                        {
                            _state = AI_States.rangeCombat;
                        }
                    }
                }
                else // ���� ��� �������
                {
                    _currentCover = _coverManager.GetCover(this);  // ����������� �������
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
                _characterAnimator.SetBool("HasEnemy", false);

                _characterAnimator.SetBool("Move", false);

                _state = AI_States.idle;
            }
        }
    }


    private void StateMoveToCover()
    {
        if (_currentTarget != null) // ���� ���� ����
        {
            if (_currentCover != null) // ���� ���������� �������
            {
                //_coverManager.ExitCover(_currentCover);

                _navMeshAgent.SetDestination(_currentCover.transform.position); // ��� � �������

                if (Vector3.Distance(this.transform.position, _currentCover.transform.position) <= 0.2f) //���� ����� �� �������
                {
                    _characterAnimator.SetBool("Move", false); // �������� ���

                    _state = AI_States.rangeCombat;

                    return; //??
                }
            }
            else // ���� ������� ���
            {
                _characterAnimator.SetBool("Move", false);  // ��������������� � �������� ���

                _state = AI_States.rangeCombat;

                return;  //??
            }

        }
        else // ���� ���� ���, �����
        {
            _characterAnimator.SetBool("Move", false);

            _characterAnimator.SetBool("HasEnemy", false);

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
                        _characterAnimator.SetBool("Move", false);  // ��������� � ��������

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
                else if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _minAttackDistance)
                {
                    _characterAnimator.SetTrigger("Punch");

                    _state = AI_States.meleeCombat;
                }
                else // ���� ��������� �� ����������, �������� ������ 
                {
                    _characterAnimator.SetBool("Move", false);

                    _state = AI_States.idle;
                }
            }
            else // ���� ���� ������, �������� ������
            {
                _characterAnimator.SetBool("Move", false);

                _state = AI_States.idle;
            }
        }
        else // ���� ���� ���, �������� ������
        {
            _characterAnimator.SetBool("Move", false);

            _state = AI_States.idle;
        }
    }


    private void StateMeleeCombat()
    {
        if (_currentTarget != null) // ���� ���� ����
        {
            if (_currentTarget.GetComponent<Vitals>().GetCurrentHealth() > 0) // ���� ���� ����
            {
                _myTransform.LookAt(_currentTarget.transform); // ������� �� ����

                // ���� ��������� ��� ����� ����������
                if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _punchDistance)
                {
                    // �������
                    if (_currentFireCooldown <= 0)
                    {
                        _characterAnimator.SetTrigger("Punch");

                        _currentGun.Punch();

                        _currentFireCooldown = _fireCooldown;
                    }
                    else
                    {
                        _currentFireCooldown -= 1 * Time.deltaTime;
                    }
                }
                else // ���� ��������� �� ����������, ������������ � ��������� �����
                {
                    _characterAnimator.SetBool("Move", false);

                    _state = AI_States.rangeCombat;
                }
            }
        }
    }


    private Team GetNewTarget()
    {
        Team _bestTarget = null;

        for (int i = 0; i < _allCharacters.Length; i++)
        {
            Team _currentCharacter = _allCharacters[i];

            //�������� �������� ������� � �������� ����, ������ ���� �� �� � ����� ������� � ���� � ���� �������� ��������
            if (_currentCharacter.GetComponent<Team>().GetTeamNumber() != MyTeam.GetTeamNumber()
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
