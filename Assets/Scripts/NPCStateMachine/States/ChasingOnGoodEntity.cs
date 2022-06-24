using UnityEngine;

public class ChasingOnGoodEntity : NPCBaseStateMachine
{
    override public void OnStateEnter(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        base.OnStateEnter(_animator, _stateInfo, _layerIndex);
    }

    override public void OnStateUpdate(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        Speed = 4.0f;

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

        NPC.transform.rotation = Quaternion.Slerp(NPC.transform.rotation, 
                Quaternion.LookRotation(_nearestGoodEntity.transform.position),
                RotationSpeed * Time.deltaTime);

        NPC.transform.Translate(0, 0, Time.deltaTime * Speed);
    }

    override public void OnStateExit(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {

    }
}
