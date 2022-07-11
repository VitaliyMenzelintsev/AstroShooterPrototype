using UnityEngine;

[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]

public class TurretBehavior : MonoBehaviour
{
    [HideInInspector]
    public Team MyTeam;
    [HideInInspector]
    public Vitals MyVitals;

    [SerializeField]
    private LineRenderer _lineRenderer;      
    [SerializeField]
    private Transform _partToRotate;                                             // ���������� ���������������� ������
    [SerializeField]
    private Transform _firePoint;
    [SerializeField] 
    private Team _currentTarget;

    private Team[] _allCharacters;
    private Transform _myTransform;

    private float _turnSpeed = 15f;                                             // �������� �������� �����
    private float _range = 15f;
    private int _damageOverTime = 30;                                           // ���� � ������


    private void Start()
    {
        _allCharacters = GameObject.FindObjectsOfType<Team>();

        _myTransform = transform;

        MyVitals = GetComponent<Vitals>();

        _currentTarget = GetNewTarget();
    }


    private void Update()
    {
        if (MyVitals.GetCurrentHealth() > 0)
        {
            if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _range)
            {
                if (_lineRenderer.enabled)                                  // ��������� �����, ����� �������� ����
                    _lineRenderer.enabled = false;

                LockOnTarget();

                Laser();
            }
        }
        else
        {
            Destroy(gameObject, 1f);
        }

    }


    private void LockOnTarget()
    {
        // ����������� ���� �������� ����� �� �����
        Vector3 _direction = _currentTarget.transform.position - transform.position;

        Quaternion _lookRotation = Quaternion.LookRotation(_direction);

        Vector3 _rotation = Quaternion.Lerp(_partToRotate.rotation, _lookRotation, Time.deltaTime * _turnSpeed).eulerAngles;

        _partToRotate.rotation = Quaternion.Euler(0f, _rotation.y, 0f);             // ����� �������� �������� ����� (X � Z freeze)
    }


    private void Laser()
    {
        Vector3 _direction = transform.forward;

        if (!_lineRenderer.enabled)
            _lineRenderer.SetPosition(0, _firePoint.position);
            _lineRenderer.SetPosition(1, _direction);

        if (Physics.Raycast(_firePoint.position, _direction, out RaycastHit _hit, float.MaxValue))   // ���� ������ �� ���-��
        {
            _hit.collider.gameObject.GetComponent<Vitals>().GetHit(_damageOverTime * Time.deltaTime);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _range);
    }

    private Team GetNewTarget()
    {
        Team _bestTarget = null;

        for (int i = 0; i < _allCharacters.Length; i++)
        {
            Team _currentCharacter = _allCharacters[i];

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

        Vector3 _directionTowardsEnemy = _enemyPosition - _firePoint.position;

        RaycastHit _hit;

        //��������� ��� �� �������� �����
        if (Physics.Raycast(_firePoint.position, _directionTowardsEnemy, out _hit, Mathf.Infinity))
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
