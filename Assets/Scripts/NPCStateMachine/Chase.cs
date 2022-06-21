using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : NPCBaseStateMachine
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        base.OnStateEnter(_animator, _stateInfo, _layerIndex);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        var _direction = Opponent.transform.position - NPC.transform.position;
        NPC.transform.rotation = Quaternion.Slerp(NPC.transform.rotation,    // кватернион слёрп плавно поворачивает в сторону нужной точки (исопльзовать для поворота игрока)
            Quaternion.LookRotation(_direction),
            RotationSpeed * Time.deltaTime);                                          // 2.0 - скорость поворота
        NPC.transform.Translate(0, 0, Time.deltaTime * Speed);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
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
