using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCompanionAI : BaseAI
{
    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        CharacterAnimator.SetFloat("_distanceToTarget", Vector3.Distance(transform.position, NearestGoodEntity.transform.position));
    }

    private void Hit()
    {
        Debug.Log("Range Robot hits");
    }

    public void StartHitting()
    {
        InvokeRepeating("Hit", 0.5f, 0.5f);
    }

    public void StopHitting()
    {
        CancelInvoke("Hit");
    }
}
