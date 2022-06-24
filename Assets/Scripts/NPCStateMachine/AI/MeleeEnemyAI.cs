using UnityEngine;

public class MeleeEnemyAI : BaseAI
{

    public override void Start()
    {
        base.Start();
        FindNearestGoodEntity();
    }

    private void Update()
    {
        _characterAnimator.SetFloat("_distanceToTarget", Vector3.Distance(transform.position, NearestGoodEntity.transform.position));
    }

    private void Hit()
    {
        Debug.Log("Melee Robot hits");
    }

    public void StartHitting()
    {
        InvokeRepeating("Hit", 1.5f, 1.5f);
    }

    public void StopHitting()
    {
        CancelInvoke("Hit");
    }
}
