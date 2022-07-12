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
    private Transform _partToRotate;                                             // определили поворачивающуюся деталь
    [SerializeField]
    private Transform _firePoint;
    [SerializeField]
    private Team _currentTarget;

    private Team[] _allCharacters;
    private Transform _myTransform;

    private float _turnSpeed = 3f;                                             // скорость поворота башни
    private float _range = 15f;
    //[SerializeField]
    private float _damageOverTime = 0.5f;                                           // урон в секунд


    private void Start()
    {
        _allCharacters = GameObject.FindObjectsOfType<Team>();

        _myTransform = transform;

        MyTeam = GetComponent<Team>();

        MyVitals = GetComponent<Vitals>();

        _lineRenderer = GetComponent<LineRenderer>();
    }


    private void FixedUpdate()
    {
        if (_currentTarget != null)
        {
            if (MyVitals.GetCurrentHealth() > 0)
            {
                if (Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _range)
                {
                    _lineRenderer.enabled = true;

                    LockOnTarget();

                    Laser();
                }
                else
                {
                    _lineRenderer.enabled = false;
                }
            }
            else
            {
                Destroy(gameObject, 1f);
            }
        }
        else
        {
            _currentTarget = GetNewTarget();
        }

    }


    private void LockOnTarget()
    {
        Vector3 _direction = _currentTarget.transform.position - transform.position;

        Quaternion _lookRotation = Quaternion.LookRotation(_direction);

        Vector3 _rotation = Quaternion.Lerp(_partToRotate.rotation, _lookRotation, Time.deltaTime * _turnSpeed).eulerAngles;

        _partToRotate.rotation = Quaternion.Euler(0f, _rotation.y, 0f);             // задаём вращение верхушке башни (X и Z freeze)
    }


    private void Laser()
    {
        Vector3 _direction = _firePoint.transform.forward;

        _lineRenderer.SetPosition(0, _firePoint.position);

        if (Physics.Raycast(_firePoint.position, _direction, out RaycastHit _hit, float.MaxValue))   // если попали во что-то
        {
            _lineRenderer.SetPosition(1, _hit.point);

            _hit.collider.gameObject.GetComponent<Vitals>().GetHit(_damageOverTime);
        }
        else
        {
            _lineRenderer.SetPosition(1, _firePoint.position + _direction * (_range - 1f));
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

        return _bestTarget;
    }
}
