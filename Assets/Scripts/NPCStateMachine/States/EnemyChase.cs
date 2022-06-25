using UnityEngine;

public class EnemyChase : NPCBaseFSM
{
    override public void OnStateEnter(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        base.OnStateEnter(_animator, _stateInfo, _layerIndex);

        Target = NPC.GetComponent<RangeEnemyAI>().GetTarget();
    }

    override public void OnStateUpdate(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        Speed = 4.0f;

        var direction = Target.transform.position - NPC.transform.position;

        NPC.transform.rotation = Quaternion.Slerp(NPC.transform.rotation, Quaternion.LookRotation(direction), RotationSpeed * Time.deltaTime);

        NPC.transform.Translate(0, 0, Time.deltaTime * Speed);
    }

    override public void OnStateExit(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {

    }
}
