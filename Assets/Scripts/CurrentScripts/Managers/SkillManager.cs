using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkillManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _allCharacters; // зачем скилл менеджеру иметь список всех персонажей?


    void Start()
    {
        _allCharacters = GameObject.FindGameObjectsWithTag("Character");
    }

    public void SlowdownSkill(GameObject _target, GameObject _owner)
    {
        // Cooldown start
        // When cooldown ended we can use skill again
        // Start skill using timer
        // when skill using timer is over, skill disabled
        _target.GetComponent<NavMeshAgent>().speed -= 2f;
        _owner.GetComponent<AIBaseBehavior>().StateSkill();
    }

}
