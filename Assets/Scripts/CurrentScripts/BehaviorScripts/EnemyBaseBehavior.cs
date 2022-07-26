using UnityEngine;

public abstract class EnemyBaseBehavior : BaseAIBehavior
{
    public Transform BuffPoint;

    public Transform GetBuffPoint()
    {
        return BuffPoint;
    }
}
