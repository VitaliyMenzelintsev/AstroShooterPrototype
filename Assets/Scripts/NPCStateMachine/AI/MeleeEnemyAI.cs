using UnityEngine;

public class MeleeEnemyAI : BaseAI
{

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        CharacterAnimator.SetFloat("_distanceToTarget", Vector3.Distance(transform.position, Target.transform.position));
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
