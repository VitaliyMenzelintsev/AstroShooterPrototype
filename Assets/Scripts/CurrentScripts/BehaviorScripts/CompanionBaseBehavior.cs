using UnityEngine;

public abstract class CompanionBaseBehavior : AIBaseBehavior
{
    [SerializeField]
    public Team CurrentTarget = null;
}
