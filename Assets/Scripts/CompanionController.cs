using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CompanionController : LivingEntity
{
    public enum State { Idle, Chasing, Attacking }
    private State _currentState;
    private NavMeshAgent _navMeshAgent;

    //public Transform LookPoint;
    public Transform FollowTarget;

    private bool _hasTarget;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        _navMeshAgent.SetDestination(FollowTarget.position);
    }

    public void LookAt(Vector3 _lookPoint)
    {
        Vector3 _heightCorrectedPoint = new Vector3(_lookPoint.x, transform.position.y, _lookPoint.z);
        transform.LookAt(_heightCorrectedPoint);
    }
}