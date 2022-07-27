using UnityEngine;

public abstract class EnemyBaseBehavior : BaseAIBehavior
{
    public Transform BuffPoint;
    protected float _speed = 3.8f;

    public Transform GetBuffPoint()
    {
        return BuffPoint;
    }
}
