using UnityEngine;

public class EnemyMeleeAttack : NPCBaseFSM
{


    override public void OnStateEnter(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        base.OnStateEnter(_animator, _stateInfo, _layerIndex);
        NPC.GetComponent<MeleeEnemyAI>().StartHitting();
    }

    override public void OnStateUpdate(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        GameObject _nearestGoodEntity = GoodEntities[0];

        for (int i = 1; i < GoodEntities.Length; i++)
        {
            var _distance = Vector3.Distance(_nearestGoodEntity.transform.position, NPC.transform.position);

            var _anotherDistance = Vector3.Distance(GoodEntities[i].transform.position, NPC.transform.position);

            if (_anotherDistance < _distance)
            {
                _nearestGoodEntity = GoodEntities[i];
            }
            else
            {
                _nearestGoodEntity = GoodEntities[0];
            }
        }

        NPC.transform.LookAt(_nearestGoodEntity.transform.position);
    }

    override public void OnStateExit(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        NPC.GetComponent<MeleeEnemyAI>().StopHitting();
    }
}
