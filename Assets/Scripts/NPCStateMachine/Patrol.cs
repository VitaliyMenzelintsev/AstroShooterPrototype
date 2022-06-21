using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : NPCBaseStateMachine
{
    GameObject[] WayPoints;
    int _currentWP;

    private void Awake()
    {
        WayPoints = GameObject.FindGameObjectsWithTag("WayPoint"); // попробовать закинуть точки через инспектор
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        base.OnStateEnter(_animator, _stateInfo, _layerIndex);
        _currentWP = 0;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (WayPoints.Length == 0) return;
        if(Vector3.Distance(WayPoints[_currentWP].transform.position,   // может лучше перемещать через НавМешАгент Сет Дестинейшн
            NPC.transform.position) < Accuracy)
        {
            _currentWP++;
            if(_currentWP >= WayPoints.Length)
            {
                _currentWP = 0;
            }
        }

        // rotate towards target
        var _direction = WayPoints[_currentWP].transform.position - NPC.transform.position;
        NPC.transform.rotation = Quaternion.Slerp(NPC.transform.rotation,    // кватернион слёрп плавно поворачивает в сторону нужной точки (исопльзовать для поворота игрока)
            Quaternion.LookRotation(_direction),
            RotationSpeed * Time.deltaTime);                                          // 2.0 - скорость поворота
        NPC.transform.Translate(0, 0, Time.deltaTime * Speed);                // может лучше перемещать через НавМешАгент Сет Дестинейшн
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
