using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShield : MonoBehaviour, ITeamable
{
    private Vitals _myVitals;
    public int _myOwnerTeamNumber;


    private void Start()
    {
        _myVitals = gameObject.GetComponent<Vitals>();

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


    private void EndSkill()
    {
        Destroy(this.gameObject);
    }


    public int GetTeamNumber()
    {
        return _myOwnerTeamNumber;
    }
}
