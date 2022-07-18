using UnityEngine;

public abstract class EnemyBaseBehavior : AIBaseBehavior
{
    [HideInInspector]
    public Team MyTeam;
    [HideInInspector]
    public Vitals MyVitals;
    public Transform Eyes;

    [SerializeField]
    public BaseGun _currentGun;
    [SerializeField]
    public Team _currentTarget;
    [SerializeField]
    public float _minAttackDistance = 1;
    [SerializeField]
    public float _maxAttackDistance = 10;
    public Team[] _allCharacters;

    public bool IsDistanceCorrect()
    {
        if(Vector3.Distance(transform.position, _currentTarget.transform.position) <= _maxAttackDistance
                && Vector3.Distance(transform.position, _currentTarget.transform.position) >= _minAttackDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public bool IsTargetAlive()
    {
        if (_currentTarget != null
            && _currentTarget.GetComponent<Vitals>().IsAlive())
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public virtual bool CanSeeTarget(Team _target)
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


    public virtual Team GetNewTarget()
    {
        Team _bestTarget = null;

        for (int i = 0; i < _allCharacters.Length; i++)
        {
            Team _currentCharacter = _allCharacters[i];

            //�������� �������� ������� � �������� ����, ������ ���� �� �� � ����� ������� � ���� � ���� �������� ��������
            if (_currentCharacter != null
                && _currentCharacter.GetComponent<Team>().GetTeamNumber() != MyTeam.GetTeamNumber()
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
                        if (Vector3.Distance(_currentCharacter.transform.position, transform.position) < Vector3.Distance(_bestTarget.transform.position, transform.position))
                        {
                            _bestTarget = _currentCharacter;
                        }
                    }
                }
            }
        }
        return _bestTarget;
    }
}
