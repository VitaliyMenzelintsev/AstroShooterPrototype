using UnityEngine;

public abstract class EnemyBaseBehavior : BaseAIBehavior
{
    [SerializeField]
    private Transform BuffPoint;

    public Transform GetBuffPoint()
    {
        return BuffPoint;
    }
}
