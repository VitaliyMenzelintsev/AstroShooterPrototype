using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]
[RequireComponent(typeof(Animator))]

public class CompanionMeleeBehavior : MonoBehaviour
{
    [HideInInspector]
    public Team MyTeam;
    [HideInInspector]
    public Vitals MyVitals;
    public Transform Eyes;

    [SerializeField]
    private Transform _followPoint;
    [SerializeField]
    private Transform _player;
    private NavMeshAgent _navMeshAgent;
    private Transform _myTransform;
    private Animator _characterAnimator;

    [SerializeField]
    private Team _currentTarget;
    [SerializeField]
    private BulletGun _currentGun;
    [SerializeField]
    private float _minAttackDistance = 7;
    [SerializeField]
    private float _maxAttackDistance = 13;


    public AI_States _state = AI_States.idle;

    private void Start()
    {
        _myTransform = transform;

        MyTeam = GetComponent<Team>();

        MyVitals = GetComponent<Vitals>();

        _navMeshAgent = GetComponent<NavMeshAgent>();

        _characterAnimator = GetComponent<Animator>();
    }

    private void Update()
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
                case AI_States.investigate:
                    StateInvestigate();
                    break;
                case AI_States.meleeCombat:
                    StateCombat();
                    break;
            }
        }
        else
        {
            _characterAnimator.SetBool("Move", false);

            _characterAnimator.SetBool("Dead", true);

            _state = AI_States.death;
        }
    }


    private void OnTriggerEnter(Collider other)  // ������������ ���������/������
    {
        GetNewTarget();

        if (_currentTarget == null)
        {
            MyVitals.GetRessurect();

            _state = AI_States.idle;

            _characterAnimator.SetBool("Dead", false);

            _characterAnimator.SetBool("HasEnemy", false);
        }
    }


    private void StateFollowThePlayer()
    {
        _currentTarget = GetNewTarget();

        if (_currentTarget == null)
        {
            if (Vector3.Distance(transform.position, _player.position) > 3f)
            {
                _myTransform.LookAt(_player);
                _characterAnimator.SetBool("Move", true);
                _navMeshAgent.SetDestination(_followPoint.position);
            }
            else
            {
                _characterAnimator.SetBool("Move", false);

                if (Vector3.Distance(_followPoint.position, _myTransform.position) < 0.3f)
                {
                    _characterAnimator.SetBool("Move", false);
                    _state = AI_States.idle;
                }
            }
        }
        else
        {
            _characterAnimator.SetBool("Move", false);
            _state = AI_States.idle;
        }
    }


    private void StateIdle()
    {
        if (_currentTarget != null)
        {
            if (_currentTarget.GetComponent<Vitals>().IsAlive())
            {
                if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _maxAttackDistance
                    && Vector3.Distance(_myTransform.position, _currentTarget.transform.position) >= _minAttackDistance)
                {
                    _state = AI_States.meleeCombat;
                }
                else
                {
                    _state = AI_States.investigate;
                }
            }
        }
        else
        {
            Team _bestTarget = GetNewTarget();

            if (_bestTarget != null)
            {
                _currentTarget = _bestTarget;
            }
            else
            {
                _characterAnimator.SetBool("Move", true); // !!
                _characterAnimator.SetBool("HasEnemy", false); // !!
                _state = AI_States.followThePlayer;
            }
        }
    }


    private void StateCombat()
    {
        if (_currentTarget != null
           && _currentTarget.GetComponent<Vitals>().IsAlive())
        {
            _myTransform.LookAt(_currentTarget.transform);

            if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _maxAttackDistance
                && Vector3.Distance(_myTransform.position, _currentTarget.transform.position) >= _minAttackDistance)
            {
                _characterAnimator.SetTrigger("Fire");

                _currentGun.Aim(_currentTarget.Eyes.position);

                _currentGun.Shoot(_currentTarget.Eyes.position);
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
        if (_currentTarget != null)
        {
            if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= 2.5f)
            {
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
        else
        {
            _characterAnimator.SetBool("Move", true); //!
            _characterAnimator.SetBool("HasEnemy", false); //!!
            _state = AI_States.followThePlayer;
        }
    }


    private Team GetNewTarget()
    {
        Team[] _allCharacters = GameObject.FindObjectsOfType<Team>();

        Team _bestTarget = null;

        for (int i = 0; i < _allCharacters.Length; i++)
        {
            Team _currentCharacter = _allCharacters[i];

            //�������� �������� ����� � �������� ����, ������ ���� �� �� � ����� ������� � ���� � ���� �������� ��������
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
                        //���� ������� ���� �����, ��� ������ ����, �� ������� �������� ������� � �������� ������ ����
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
