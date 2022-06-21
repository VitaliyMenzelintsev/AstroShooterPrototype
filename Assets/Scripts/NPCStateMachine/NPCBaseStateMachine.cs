using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBaseStateMachine : StateMachineBehaviour
{
    public GameObject NPC;
    public GameObject Opponent;
    public float Speed = 3.0f;
    public float RotationSpeed = 2.0f;
    public float Accuracy = 1.0f;

    public override void OnStateEnter(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        NPC = _animator.gameObject;
        Opponent = NPC.GetComponent<RobotAI>().GetPlayer();  // FindObjectFithTag("Player") ???
    }
}
