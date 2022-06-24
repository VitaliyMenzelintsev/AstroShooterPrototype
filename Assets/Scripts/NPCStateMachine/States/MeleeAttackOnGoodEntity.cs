using UnityEngine;

public class MeleeAttackOnGoodEntity : NPCBaseStateMachine
{


    override public void OnStateEnter(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        base.OnStateEnter(_animator, _stateInfo, _layerIndex);
        NPC.GetComponent<MeleeEnemyAI>().StartHitting();
    }

    override public void OnStateUpdate(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        NPC.transform.LookAt(_nearestGoodEntity.transform.position);
    }

    override public void OnStateExit(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        NPC.GetComponent<MeleeEnemyAI>().StopHitting();
    }
}
