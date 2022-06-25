using UnityEngine; 

public class RangeEnemyAI : BaseAI
{
    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();

        GetTarget();

        CharacterAnimator.SetFloat("_distanceToTarget", Vector3.Distance(transform.position, Target.transform.position));
    }

    public override GameObject GetTarget()
    {
        return base.GetTarget();
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

