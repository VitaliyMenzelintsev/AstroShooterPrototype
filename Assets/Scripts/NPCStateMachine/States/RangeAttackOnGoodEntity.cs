using UnityEngine;

public class RangeAttackOnGoodEntity : NPCBaseStateMachine
{
    override public void OnStateEnter(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        base.OnStateEnter(_animator, _stateInfo, _layerIndex);
        NPC.GetComponent<RangeEnemyAI>().StartHitting(); // передать в старт цель
    }

    override public void OnStateUpdate(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        // выбираем ближайшего доброго персонажа

        GameObject[] _goodEntities = GoodEntity.ToArray();

        GameObject _nearestGoodEntity = _goodEntities[0];

        for (int i = 1; i < _goodEntities.Length; i++)
        {
            var _distance = Vector3.Distance(_nearestGoodEntity.transform.position, NPC.transform.position);

            var _anotherDistance = Vector3.Distance(_goodEntities[i].transform.position, NPC.transform.position);

            if (_anotherDistance < _distance)
            {
                _nearestGoodEntity = _goodEntities[i];
            }
            else
            {
                _nearestGoodEntity = _goodEntities[0];
            }
        }

        NPC.transform.LookAt(_nearestGoodEntity.transform.position); 
    }

    override public void OnStateExit(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        NPC.GetComponent<RangeEnemyAI>().StopHitting();
    }
}