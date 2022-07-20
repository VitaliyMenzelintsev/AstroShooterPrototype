using UnityEngine;

[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]
public abstract class BaseCharacter : MonoBehaviour
{
    [SerializeField]
    protected Transform Eyes;
    protected GameObject CurrentTarget = null;

    public Transform GetEyesPosition()
    {
        return Eyes;
    }

    public GameObject GetMyTarget()
    {
        return CurrentTarget;
    }
}
