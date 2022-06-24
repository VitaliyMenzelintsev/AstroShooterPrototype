using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CompanionController : LivingEntity
{
    public Transform FollowPoint;
    public Transform Player;
    public enum State { Idle, Chasing, Attacking }
    private State _currentState;
    private NavMeshAgent _navMeshAgent;
    private GunController _gunController;
    //private Animator _npcAnimator;
    private Camera _viewCamera;
    private bool _hasTarget;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _gunController = GetComponent<GunController>();
        //_npcAnimator = GetComponent<Animator>();
        _viewCamera = Camera.main;
    }

    private void Update()
    {
        // Movement
        if(Vector3.Distance(Player.position, transform.position) > 3)
        _navMeshAgent.SetDestination(FollowPoint.position); // ввести проверку, если координаты цели изменились, то запустить поиск пути

        // Look input
        Ray _ray = _viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane _groundPlane = new Plane(Vector3.up, Vector3.up * 1.8f);
        float _rayDistance;

        if (_groundPlane.Raycast(_ray, out _rayDistance))
        {
            Vector3 _point = _ray.GetPoint(_rayDistance);
            LookAt(_point);

            if ((new Vector2(_point.x, _point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
                _gunController.Aim(_point);
        }
    }

    public void LookAt(Vector3 _lookPoint)
    {
        Vector3 _heightCorrectedPoint = new Vector3(_lookPoint.x, transform.position.y, _lookPoint.z);
        transform.LookAt(_heightCorrectedPoint);
    }
}