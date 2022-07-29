using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField]
    private float _time;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _angle;
    private float _gravity = 9.81f;



    private void Start()
    {
        transform.localEulerAngles = new Vector3(transform.parent.eulerAngles.x, 0, transform.parent.eulerAngles.z);
        transform.parent.eulerAngles = new Vector3(0, transform.parent.eulerAngles.y, 0);

        _angle = -transform.localEulerAngles.x;
        _angle = _angle * Mathf.Deg2Rad;

    }




    private void Update()
    {
        _time += Time.deltaTime;
        float _vz = _speed * _time * Mathf.Cos(_angle) * Time.deltaTime;
        float _vy = _speed * _time * Mathf.Sin(_angle) * Time.deltaTime - _gravity * _time * _time / 2 * Time.deltaTime;
        Vector3 _futurePosition = new Vector3(transform.localPosition.x, transform.localPosition.y + _vy, transform.localPosition.z + _vz);
        transform.rotation = Quaternion.LookRotation(transform.localPosition - _futurePosition);
        transform.eulerAngles += new Vector3(0, transform.parent.eulerAngles.y, 0);
        transform.localPosition = _futurePosition;

    }
}
