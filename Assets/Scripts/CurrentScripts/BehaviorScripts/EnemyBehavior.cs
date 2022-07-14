using UnityEngine;

public abstract class EnemyBehavior : MonoBehaviour
{
    [HideInInspector]
    public Team MyTeam;
    [HideInInspector]
    public Vitals MyVitals;
    public Transform Eyes;

    [SerializeField]
    protected BaseGun _currentGun;
    [SerializeField]
    protected Team _currentTarget;
    protected Transform _myTransform;
    [SerializeField]
    protected float _minAttackDistance = 5;
    [SerializeField]
    protected float _maxAttackDistance = 13;
    protected Team[] _allCharacters;

    protected bool IsDistanceCorrect()
    {
        if(Vector3.Distance(_myTransform.position, _currentTarget.transform.position) <= _maxAttackDistance
                && Vector3.Distance(_myTransform.position, _currentTarget.transform.position) >= _minAttackDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected bool IsTargetAlive()
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
