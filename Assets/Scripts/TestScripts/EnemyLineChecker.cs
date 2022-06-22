using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class EnemyLineChecker : MonoBehaviour
{
    public CapsuleCollider Collider;
    public float FieldOfView = 90f;
    public LayerMask LineOfSightLayers;

    public delegate void GainSightEvent(Transform Target);
    public GainSightEvent OnGainSight;
    public delegate void LoseSightEvent(Transform Target);
    public LoseSightEvent OnLoseSight;

    private Coroutine CheckForLineOfSightCoroutine;

    private void Awake()
    {
        Collider = GetComponent<CapsuleCollider>();
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (!CheckLineOfSight(_other.transform))
        {
            CheckForLineOfSightCoroutine = StartCoroutine(CheckForLineOfSight(_other.transform));
        }
    }

    private void OnTriggerExit(Collider _other)
    {
        OnLoseSight?.Invoke(_other.transform);
        if (CheckForLineOfSightCoroutine != null)
        {
            StopCoroutine(CheckForLineOfSightCoroutine);
        }
    }

    private bool CheckLineOfSight(Transform _target)
    {
        Vector3 _direction = (_target.transform.position - transform.position).normalized;
        float _dotProduct = Vector3.Dot(transform.forward, _direction);
        if (_dotProduct >= Mathf.Cos(FieldOfView))
        {
            if (Physics.Raycast(transform.position, _direction, out RaycastHit hit, Collider.radius, LineOfSightLayers))
            {
                OnGainSight?.Invoke(_target);
                return true;
            }
        }
        return false;
    }

    private IEnumerator CheckForLineOfSight(Transform _target)
    {
        WaitForSeconds Wait = new WaitForSeconds(0.5f);

        while (!CheckLineOfSight(_target))
        {
            yield return Wait;
        }
    }
}