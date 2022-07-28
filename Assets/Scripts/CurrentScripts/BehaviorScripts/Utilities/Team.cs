using UnityEngine;

public class Team : MonoBehaviour, ITeamable
{
    [SerializeField]
    private int _teamNumber;
  
    public int GetTeamNumber()
    {
        return _teamNumber;
    }
}