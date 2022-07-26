//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Door : MonoBehaviour
//{
//    public bool IsOpen = false;
//    [SerializeField]
//    private bool _isRotatingDoor = true;
//    [SerializeField]
//    private float _speed = 1f;
//    [SerializeField]
//    private float _rotatingAngle = 110f;
//    [SerializeField]
//    private float _forwardDirection = 0f;

//    private Vector3 _startRotation;
//    private Vector3 _forward;

//    private Coroutine Animationcoroutine;


//    private void Awake()
//    {
//        _startRotation = transform.rotation.eulerAngles;
//        _forward = transform.right;
//    }






//    public void Open(Vector3 UserPosition)
//    {
//        if (!IsOpen)
//        {
//            if(Animationcoroutine != null)
//            {
//                StopCoroutine(Animationcoroutine);
//            }

//            if (_isRotatingDoor)
//            {
//                float _dot = Vector3.Dot(_forward, (UserPosition))
//            }

//        }
//    }
//}
