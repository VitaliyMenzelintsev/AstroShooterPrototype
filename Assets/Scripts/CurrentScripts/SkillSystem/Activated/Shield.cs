using UnityEngine;

public class Shield : MonoBehaviour, ITeamable
{
    private Vitals _myVitals;
    private CoverManager _coverManager;
    [SerializeField]
    private CompanionCoverSpot[] _spots;
    public int _myOwnerTeamNumber;

    private void Start()
    {
        _myVitals = gameObject.GetComponent<Vitals>();

        _coverManager = GameObject.FindObjectOfType<CoverManager>();

        for (int i = 0; i < _spots.Length; i++)
        {
            _coverManager.AddTemporaryCoverSpots(_spots[i]); 
        }
    }

    private void Update()
    {
        if (!_myVitals.IsAlive())
        {
            EndSkill();
        }
    }

    public void EndTimer(float _skillDuration)
    {
        Invoke("EndSkill", _skillDuration);
    }

    private void RemoveSpots()
    {
        for (int i = 0; i < _spots.Length; i++)
        {
            _coverManager.RemoveTemporaryCoverSpots(_spots[i]);
        }
    }

    private void EndSkill()
    {
        RemoveSpots();

        Destroy(this.gameObject);
    }

    public int GetTeamNumber()
    {
        return _myOwnerTeamNumber;
    }
}
