using UnityEngine;

public class EnemyRangeAttack : NPCBaseFSM
{
    public GameObject NearestGoodEntity;

    private void Awake()
    {
        
    }
    override public void OnStateEnter(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        base.OnStateEnter(_animator, _stateInfo, _layerIndex);
        NPC.GetComponent<RangeEnemyAI>().StartHitting(); 
    }

    override public void OnStateUpdate(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {

    }

    override public void OnStateExit(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        NPC.GetComponent<RangeEnemyAI>().StopHitting();
    }
}