using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAI : MonoBehaviour
{
    private Animator _animator;
    public GameObject Player;

    public GameObject GetPlayer()
    {
        return Player;
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _animator.SetFloat("_distanceToPlayer", Vector3.Distance(transform.position, Player.transform.position));
    }
}
