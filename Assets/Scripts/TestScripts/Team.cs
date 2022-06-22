using UnityEngine;

public class Team : MonoBehaviour
{
    [SerializeField] int _teamNumber;

    public int GetTeamNumber()
    {
        return _teamNumber;
    }
}