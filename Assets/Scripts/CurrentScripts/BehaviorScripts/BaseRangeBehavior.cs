using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]
[RequireComponent(typeof(Animator))]
public abstract class BaseRangeBehavior : MonoBehaviour
{
    [HideInInspector]
    public Team MyTeam;
    [HideInInspector]
    public Vitals MyVitals;
    public Transform Eyes;
    public Transform FollowPoint;
    public Transform Player;

    private NavMeshAgent _navMeshAgent;
    private Transform _myTransform;
    private Animator _characterAnimator;
    private CompanionCoverManager _coverManager;

    [SerializeField] // ����
    private Team _currentTarget;
    [SerializeField]
    private Gun _currentGun;
    [SerializeField]
    private float _minAttackDistance = 7;
    [SerializeField]
    private float _maxAttackDistance = 13;
    [SerializeField]
    private float _punchDistance = 1;
    [SerializeField]
    private float _moveSpeed = 3f;
    //[SerializeField]
    //private float _fireCooldown = 0.5f;
    //private float _currentFireCooldown = 0;
    [SerializeField]
    private CompanionCoverSpot _currentCover = null;

    public AI_States _state = AI_States.idle;

    public virtual void Start()
    {
        _myTransform = transform;

        MyTeam = GetComponent<Team>();

        MyVitals = GetComponent<Vitals>();

        _navMeshAgent = GetComponent<NavMeshAgent>();

        _characterAnimator = GetComponent<Animator>();

        _coverManager = GameObject.FindObjectOfType<CompanionCoverManager>();
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
                case AI_States.followThePlayer:
                    StateFollowThePlayer();
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
                case AI_States.death:
                    StateDeath();
                    break;
            }
        }
        else
        {
            DeathActions();
        }
    }
    protected virtual void DeathActions()
    {
        _state = AI_States.death;
        _characterAnimator.SetBool("Move", false);
        _characterAnimator.SetBool("Dead", true);
        if (_currentCover != null)
            _coverManager.ExitCover(ref _currentCover);
    }

    private void StateDeath()
    {
        // � �������: ������������ ������������
    }


    private void StateIdle()
    {
        if (_currentTarget != null && _currentTarget.GetComponent<Vitals>().IsAlive()) // ���� ���� ����
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

                if (Vector3.Distance(transform.position, Player.position) > 3f)
                {
                    _characterAnimator.SetBool("Move", true);
                    _state = AI_States.followThePlayer;
                }
            }
            // ����� ����� � ������ �� ������
        }
    }


    private void StateFollowThePlayer()
    {
        _currentTarget = GetNewTarget(); // �������, ���� �� ����


        if (_currentTarget == null) // ���� ���� ���
        {
            if (_currentCover != null) // ����������� ���������� �������
                _coverManager.ExitCover(ref _currentCover);

            _navMeshAgent.SetDestination(FollowPoint.position); // ��� �� �������

            if (Vector3.Distance(FollowPoint.position, _myTransform.position) < 0.3f) // ���� ��������� �� ����� ���������� ������
            {
                _characterAnimator.SetBool("Move", false); // ���������������
                _state = AI_States.idle;
            }
        }
        else
        {
            _characterAnimator.SetBool("Move", false);  // ��������������� ������������ �� ������� ��� ���
            _state = AI_States.idle;
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
        if (_currentTarget != null
            && _currentTarget.GetComponent<Vitals>().IsAlive()) // ���� ���� ����
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

                _characterAnimator.SetTrigger("Fire");

                _currentGun.Aim(_currentTarget.Eyes.position);

                _currentGun.Shoot(_currentTarget.Eyes.position);


            }
            else if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) < _minAttackDistance)
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

        else // ���� ���� ���, �������� ������
        {
            _characterAnimator.SetBool("Move", false);


            if (_currentCover != null)
                _coverManager.ExitCover(ref _currentCover);


            _state = AI_States.idle;
        }
    }


    private void StateMeleeCombat()
    {
        if (_currentTarget != null) // ���� ���� ����
        {
            if (_currentTarget.GetComponent<Vitals>().IsAlive()) // ���� ���� ����
            {
                _myTransform.LookAt(_currentTarget.transform); // ������� �� ����

                // ���� ��������� ��� ����� ����������
                if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _punchDistance)
                {
                    // �������

                    _characterAnimator.SetTrigger("Punch");

                    _currentGun.Punch();

                }
                else // ���� ��������� �� ����������, ������������ � ��������� �����
                {
                    _characterAnimator.SetBool("Move", false);

                    _state = AI_States.rangeCombat;
                }
            }
            else // ���� ���� ������
            {
                _characterAnimator.SetBool("Move", false);

                _state = AI_States.idle;
            }
        }
        else // ���� ���� ���
        {
            _characterAnimator.SetBool("Move", false);

            _state = AI_States.idle;
        }
    }


    private Team GetNewTarget()
    {
        Team[] _allCharacters = GameObject.FindObjectsOfType<Team>();

        Team _bestTarget = null;

        for (int i = 0; i < _allCharacters.Length; i++)
        {
            Team _currentCharacter = _allCharacters[i];

            //�������� �������� ������� � �������� ����, ������ ���� �� �� � ����� ������� � ���� � ���� �������� ��������
            if (_currentCharacter.GetComponent<Team>().GetTeamNumber() != MyTeam.GetTeamNumber()
                && _currentCharacter.GetComponent<Vitals>().IsAlive())
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

        Vector3 _enemyPosition = _target.Eyes.position;

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