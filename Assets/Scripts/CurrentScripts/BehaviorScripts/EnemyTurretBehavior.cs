using UnityEngine;

[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]

public class EnemyTurretBehavior : EnemyBehavior
{
    [SerializeField]
    private Transform _partToRotate;                                             // ���������� ���������������� ������
    private float _turnSpeed = 5f;                                               // �������� �������� �����

    public AI_States _state = AI_States.idle;

    private void Start()
    {
        _allCharacters = GameObject.FindObjectsOfType<Team>();

        _myTransform = transform;

        MyTeam = GetComponent<Team>();

        MyVitals = GetComponent<Vitals>();
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
                case AI_States.death:
                    StateDeath();
                    break;
            }
        }
        else
        {
            _state = AI_States.death;
        }
    }


    private void StateIdle()
    {
        if (IsTargetAlive()) // ���� ���� ���� � ��� ����
        {
            if (IsDistanceCorrect())
            {
                _state = AI_States.rangeCombat;
            }
            else
            {
                _state = AI_States.idle;
            }
        }
        else // ���� ���� ��� ��� ��� ������
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

    private void StateRangeCombat()
    {
        if (IsTargetAlive())
        {
            if (IsDistanceCorrect())
            {
                // �����
                {
                    LockOnTarget();

                    _currentGun.Shoot(_currentTarget.Eyes.position);
                }
            }
            else
            {
                _state = AI_States.idle;
            }
        }
        else
        {
            _state = AI_States.idle;
        }
    }

    private void StateDeath()
    {
        Destroy(this, 3f); 
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

    private void LockOnTarget()
    {
        Vector3 _direction = _currentTarget.transform.position - transform.position;

        Quaternion _lookRotation = Quaternion.LookRotation(_direction);

        Vector3 _rotation = Quaternion.Lerp(_partToRotate.rotation, _lookRotation, Time.deltaTime * _turnSpeed).eulerAngles;

        _partToRotate.rotation = Quaternion.Euler(0f, _rotation.y, 0f);             // ����� �������� �������� ����� (X � Z freeze)
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _maxAttackDistance);
    }

    private Team GetNewTarget()
    {
        Team _bestTarget = null;

        for (int i = 0; i < _allCharacters.Length; i++)
        {
            Team _currentCharacter = _allCharacters[i];

            if (_currentCharacter.GetComponent<Team>().GetTeamNumber() != MyTeam.GetTeamNumber()
                && _currentCharacter.GetComponent<Vitals>().IsAlive())
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

        return _bestTarget;
    }
}
