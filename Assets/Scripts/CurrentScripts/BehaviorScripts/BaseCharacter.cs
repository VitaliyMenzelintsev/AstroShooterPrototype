using UnityEngine;

[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]
public abstract class BaseCharacter : MonoBehaviour
{
    [HideInInspector]
    public Team MyTeam;
    [HideInInspector]
    public Vitals MyVitals;
    [SerializeField]
    protected Transform Eyes;
    [SerializeField]
    protected GameObject CurrentTarget = null;
    protected TargetManager _targetManager;
    [SerializeField]
    protected int _myTeamNumber;
    [SerializeField]
    public BaseGun _currentGun;
    [SerializeField]
    protected float _minAttackDistance = 1.5f;
    [SerializeField]
    protected float _maxAttackDistance = 13f;



    public virtual void Start()
    {
        MyTeam = GetComponent<Team>();

        MyVitals = GetComponent<Vitals>();

        _myTeamNumber = MyTeam.GetTeamNumber();

        _targetManager = GameObject.FindObjectOfType<TargetManager>();
    }


    public Transform GetEyesPosition()
    {
        return Eyes;
    } 


    public GameObject GetMyTarget()
    {
        return CurrentTarget;
    }


    protected void GetNewTarget()
    {
        CurrentTarget = _targetManager.GetNewTarget(_myTeamNumber, Eyes, true);
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
