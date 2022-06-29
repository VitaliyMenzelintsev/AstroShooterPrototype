using UnityEngine;

public class FollowToPlayer : NPCBaseFSM
{
    override public void OnStateEnter(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        base.OnStateEnter(_animator, _stateInfo, _layerIndex);
        FollowPoint = NPC.GetComponent<RangeCompanionAI>().GetFollowPoint();
    }

    override public void OnStateUpdate(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        Speed = 4.0f;

        var direction = FollowPoint.transform.position - NPC.transform.position;

        NPC.transform.Translate(direction* Time.deltaTime * Speed);
    }

    override public void OnStateExit(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {

    }
}
