using UnityEngine;
using System.Collections.Generic;

public class NPCBaseStateMachine : StateMachineBehaviour
{
    public GameObject NPC;
    public List<GameObject> GoodEntity;
    public List<GameObject> BadEntity;
    public GameObject _nearestGoodEntity;
    public GameObject _nearestBadEntity;

    public float Speed = 3.0f;
    public float RotationSpeed = 2.0f;
    public float Accuracy = 1.0f;

    public override void OnStateEnter(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        NPC = _animator.gameObject;
        GoodEntity = new List<GameObject> (GameObject.FindGameObjectsWithTag("GoodEntity"));
        BadEntity = new List<GameObject> (GameObject.FindGameObjectsWithTag("BadEntity"));

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

        GameObject[] _badEntities = GoodEntity.ToArray();
        GameObject _nearestBadEntity = _badEntities[0];

        for (int i = 1; i < _badEntities.Length; i++)
        {
            var _distance = Vector3.Distance(_nearestBadEntity.transform.position, NPC.transform.position);

            var _anotherDistance = Vector3.Distance(_badEntities[i].transform.position, NPC.transform.position);

            if (_anotherDistance < _distance)
            {
                _nearestBadEntity = _badEntities[i];
            }
            else
            {
                _nearestBadEntity = _badEntities[0];
            }
        }
    }
}