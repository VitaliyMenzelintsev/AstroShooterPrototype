using UnityEngine;

public abstract class EnemyBehavior : MonoBehaviour
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
    public float _minAttackDistance = 5;
    [SerializeField]
    public float _maxAttackDistance = 13;
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
}
