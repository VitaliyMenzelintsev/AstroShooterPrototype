using System.Collections;
using UnityEngine;

public class BehaviourSwitcher : MonoBehaviour
{
    [SerializeField]
    private CompanionMeleeBehavior _myMeleeScript;
    [SerializeField]
    private CompanionRangeBehavior _myRangeScript;
    [SerializeField]
    private float _myCurrentHealth;
    [SerializeField]
    private float _myMaxHealth;

    private void Start()
    {
        _myMeleeScript = GetComponent<CompanionMeleeBehavior>();
        _myRangeScript = GetComponent<CompanionRangeBehavior>();

        InvokeRepeating("HealthChecker", 2f, 0.5f);
    }


    private void HealthChecker()
    {
        _myCurrentHealth = GetComponent<Vitals>().GetCurrentHealth();
        _myMaxHealth = GetComponent<Vitals>().GetMaxHealth();

        if (_myCurrentHealth <= _myMaxHealth / 3)
        {
            _myMeleeScript.enabled  = false;
            _myRangeScript.enabled = true;


        }
        else
        {
            _myMeleeScript.enabled = true;
            _myRangeScript.enabled = false;
        }
    }
}
