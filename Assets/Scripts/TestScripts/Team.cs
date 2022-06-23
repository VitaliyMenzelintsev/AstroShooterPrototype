using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    [SerializeField]
    private int _teamNumber;

    public int GetTeamNumber()
    {
        return _teamNumber;
    }
}