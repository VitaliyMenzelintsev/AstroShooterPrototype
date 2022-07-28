using UnityEngine;

[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]
public abstract class BaseCharacter : MonoBehaviour, IVisible
{
    [HideInInspector]
    public Team MyTeam;
    [HideInInspector]
    public Vitals MyVitals;
    [HideInInspector]
    public float _speed = 3.2f;
    [SerializeField]
    protected Transform Head;
    //[SerializeField]
    public GameObject CurrentTarget = null;
    [SerializeField]
    protected int _myTeamNumber;
    [SerializeField]
    protected BaseGun _currentGun;
    [SerializeField]
    protected float _minAttackDistance = 1.5f;
    [SerializeField]
    protected float _maxAttackDistance = 12f;
    protected TargetManager _targetManager;
    protected Collider[] _myColliders;



    public virtual void Start()
    {
        MyTeam = GetComponent<Team>();

        MyVitals = GetComponent<Vitals>();

        _myTeamNumber = MyTeam.GetTeamNumber();

        _targetManager = GameObject.FindObjectOfType<TargetManager>();

        _myColliders = GetComponentsInChildren<Collider>();
    }


    public Transform GetHeadTransform()
    {
        return Head;
    } 


    public float GetMaxAttackDistance()
    {
        return _maxAttackDistance;
    }


    public GameObject GetMyTarget()
    {
        return CurrentTarget;
    }


    protected void GetNewTarget()
    {
        CurrentTarget = _targetManager.GetNewTarget(_myTeamNumber, Head, true);
    }


    protected bool IsDistanceCorrect()
    {
        if (Vector3.Distance(transform.position, CurrentTarget.transform.position) <= _maxAttackDistance
                && Vector3.Distance(transform.position, CurrentTarget.transform.position) >= _minAttackDistance)
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
        if (CurrentTarget != null
            && CurrentTarget.GetComponent<Vitals>().IsAlive())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
