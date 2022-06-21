using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : NPCBaseStateMachine
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        base.OnStateEnter(_animator, _stateInfo, _layerIndex);
        NPC.GetComponent<MeleeRobotAI>().StartHitting();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        NPC.transform.LookAt(Opponent.transform.position); // добавить слёрп
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        NPC.GetComponent<MeleeRobotAI>().StopHitting();
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
