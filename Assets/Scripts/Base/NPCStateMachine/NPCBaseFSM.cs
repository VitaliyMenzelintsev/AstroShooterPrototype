using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class NPCBaseFSM : StateMachineBehaviour
{
    public GameObject NPC;
    public GameObject Target;
    public GameObject FollowPoint;
    public GameObject[] GoodEntities; 
    public GameObject[] BadEntiies; 

    public float Speed = 3.0f;
    public float RotationSpeed = 2.0f;
    public float Accuracy = 1.0f;


    override public void OnStateEnter(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        NPC = _animator.gameObject;
    }
}