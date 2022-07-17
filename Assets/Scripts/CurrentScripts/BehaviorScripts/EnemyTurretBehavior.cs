using UnityEngine;

[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]

public class EnemyTurretBehavior : EnemyBehavior
{
    [SerializeField]
    private Transform _partToRotate;                                             // определили поворачивающуюся деталь
    private float _turnSpeed = 5f;                                               // скорость поворота башни

    private void Start()
    {
        MyTeam = GetComponent<Team>();

        MyVitals = GetComponent<Vitals>();

        _allCharacters = GameObject.FindObjectsOfType<Team>();
    }


    private void FixedUpdate()
    {
        if (MyVitals.IsAlive())
        {
            if (IsTargetAlive())
                {
                if (IsDistanceCorrect())
                {
                    StateRangeCombat();
                }
                else
                {
                    StateIdle();
                }
            }
            else
            {
                _currentTarget = GetNewTarget();

                StateIdle();
            }
        }
        else
        {
            StateDeath();
        }
    }

    private void StateDeath()
    {
        Destroy(this, 3f);
    }

    private void StateIdle()
    {
       
    }

    private void StateRangeCombat()
    {

        LockOnTarget();

        _currentGun.Shoot(_currentTarget.Eyes.position);

    }



    private void LockOnTarget()
    {
        Vector3 _direction = _currentTarget.transform.position - transform.position;

        Quaternion _lookRotation = Quaternion.LookRotation(_direction);

        Vector3 _rotation = Quaternion.Lerp(_partToRotate.rotation, _lookRotation, Time.deltaTime * _turnSpeed).eulerAngles;

        _partToRotate.rotation = Quaternion.Euler(0f, _rotation.y, 0f);             // задаём вращение верхушке башни (X и Z freeze)
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _maxAttackDistance);
    }

}
