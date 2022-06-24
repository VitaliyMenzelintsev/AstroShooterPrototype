using UnityEngine;

public class RangeAttack : NPCBaseStateMachine
{
    override public void OnStateEnter(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        base.OnStateEnter(_animator, _stateInfo, _layerIndex);
        NPC.GetComponent<RangeRobotAI>().StartHitting();
    }

    override public void OnStateUpdate(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        NPC.transform.LookAt(Opponent.transform.position); // �������� ����
    }

    override public void OnStateExit(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        NPC.GetComponent<RangeRobotAI>().StopHitting();
    }
}