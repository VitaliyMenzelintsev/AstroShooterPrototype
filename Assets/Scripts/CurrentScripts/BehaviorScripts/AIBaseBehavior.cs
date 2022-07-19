using UnityEngine;

[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]

public abstract class AIBaseBehavior : MonoBehaviour
{
    [HideInInspector]
    public Team MyTeam;
    [HideInInspector]
    public Vitals MyVitals;
    public Transform Eyes;
    [SerializeField]
    public GameObject CurrentTarget = null;
    protected TargetManager _targetManager;
    [SerializeField]
    protected int _myTeamNumber;
    [SerializeField]
    protected BaseGun _currentGun;
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


    public abstract void StateSkill(bool _isESkill, GameObject _target);
   

    public void GetNewTarget()
    {
        CurrentTarget = _targetManager.GetNewTarget(_myTeamNumber, Eyes, true);
    }

    public bool IsDistanceCorrect()
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


    public bool IsTargetAlive()
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
