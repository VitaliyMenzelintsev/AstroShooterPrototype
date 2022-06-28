using UnityEngine;

public class Team : MonoBehaviour
{
    [SerializeField]
    private int _teamNumber;
    
    public Transform Eyes;



    public int GetTeamNumber()
    {
        return _teamNumber;
    }
}