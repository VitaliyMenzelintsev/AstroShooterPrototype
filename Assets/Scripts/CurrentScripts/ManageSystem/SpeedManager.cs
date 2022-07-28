using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpeedManager : MonoBehaviour
{ 
    
    public void TemporarilyAIChange(GameObject _target, bool _isOn, float _value, float _time)
    {
        if (_isOn)
        {
            StartCoroutine(Timer(_target, _value, _time));

            _target.GetComponent<NavMeshAgent>().speed += _value;
        }
        else
        {
            _target.GetComponent<NavMeshAgent>().speed -= _value;
        }
    }


    public void TemporarilyPlayerChange(GameObject _target, bool _isOn, float _value, float _time)
    {
        if (_isOn)
        {
            StartCoroutine(Timer(_target, _value, _time));

            _target.GetComponent<PlayerController>().SpeedChange(_value);
        }
        else
        {
            _target.GetComponent<PlayerController>().SpeedChange(-_value);
        }
            
    }
     

    public void PermanentAIChange(GameObject _target, bool _isOn, float _value)
    {
        if (_isOn)
        {
            _target.GetComponent<NavMeshAgent>().speed += _value;
        }
        else
        {
            _target.GetComponent<NavMeshAgent>().speed -= _value;
        }
    }
     

    public void PermanentPlayerChange(GameObject _target, bool _isOn, float _value)
    {
        if (_isOn)
        {
            _target.GetComponent<PlayerController>().SpeedChange(_value);
        }
        else
        {
            _target.GetComponent<PlayerController>().SpeedChange(-_value);
        }
    }


    private IEnumerator Timer(GameObject _target, float _value, float _time)
    {
        yield return new WaitForSeconds(_time);

        TemporarilyAIChange(_target, false, _value, _time);
    }
}
