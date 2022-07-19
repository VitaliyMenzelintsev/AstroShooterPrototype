using UnityEngine;

public abstract class EnemyBaseBehavior : AIBaseBehavior
{
    public Transform Antenna;

    public Transform GetAntenna()
    {
        return Antenna;
    }
}
