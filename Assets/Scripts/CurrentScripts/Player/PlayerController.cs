using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController), typeof(PlayerInputOld))]
// Висит на игроке
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float rotationSpeed = 5f;
    private Transform cameraTransform;
    [SerializeField]
    private Gun _currentGun;
    private Camera _viewCamera;

    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector3 playerVelocity;

    private InputAction moveAction;
    private InputAction crouchAction;
    private InputAction shootAction;

    private Vector3 _point;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;
        _viewCamera = Camera.main;
        moveAction = playerInput.actions["Move"];
        crouchAction = playerInput.actions["Crouch"];
        shootAction = playerInput.actions["Shoot"];
    }

    private void OnEnable()
    {
        shootAction.performed += _ => ShootGun();
    }

    private void OnDisable()
    {
        shootAction.performed -= _ => ShootGun();
    }

    private void ShootGun()
    {
        _currentGun.OnTriggerHold(_point);

        //RaycastHit _hit;
        //Ray _ray = new Ray(cameraTransform.position, cameraTransform.forward);
        //if (Physics.Raycast(_ray, out _hit, Mathf.Infinity))
        //{
        //    _point = _ray.GetPoint(Mathf.Infinity);
        //}
        //_currentGun.Shoot(_point);
    }

    public void Aim(Vector3 _aimPoint)
    {
        _currentGun.Aim(_aimPoint);
    }

    private void Update()
    {
        Ray _ray = new Ray(cameraTransform.position, cameraTransform.forward);
        Plane _groundPlane = new Plane(Vector3.up, Vector3.up * 1.8f);
        float _rayDistance;

        if (_groundPlane.Raycast(_ray, out _rayDistance))
        {
            _point = _ray.GetPoint(_rayDistance);

            if ((new Vector2(_point.x, _point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
                Aim(_point);
        }

       
        if (playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;  // движение игрока относительно камеры
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);

        controller.Move(playerVelocity * Time.deltaTime);

        // поворот в сторону курсора
        float targetAngle = cameraTransform.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}