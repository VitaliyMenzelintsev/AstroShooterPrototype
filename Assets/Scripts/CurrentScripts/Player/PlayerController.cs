using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed = 3.4f;
    [SerializeField]
    private float crouchSpeed = 2.6f;
    [SerializeField]
    private float sprintSpeed = 4.0f;
    [SerializeField]
    private float currentSpeed;
    [SerializeField]
    private float rotationSpeed = 5f;
    private Transform cameraTransform;
    [SerializeField]
    private Gun _currentGun;
    [SerializeField]
    private float animationSmoothTime = 0.2f;  // смягчение скорости для анимации
    //[SerializeField]
    //private float animationPlayTransition = 0.2f;
    //[SerializeField]
    //private Transform aimTarget;

    private float gravityValue = -9.81f;

    private CharacterController controller;
    private PlayerInput playerInput;

    private Vector3 playerVelocity;
    private bool groundedPlayer;

    private InputAction moveAction;
    private InputAction crouchAction; // ?
    private InputAction sprintAction; // ?
    private InputAction shootAction;
    private InputAction reloadAction;


    private Vector3 _aimPoint;

    private Animator animator;
    private int moveXAnimationParameterID;
    private int moveZAnimationParameterID;
    private int shootAnimation;
    private int crouchAnimation;

    private Vector2 currentAnimationBlendVector;
    private Vector2 animationVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;

        moveAction = playerInput.actions["Move"];
        crouchAction = playerInput.actions["Crouch"];
        sprintAction = playerInput.actions["Sprint"];
        shootAction = playerInput.actions["Shoot"];
        reloadAction = playerInput.actions["Reload"];

        animator = GetComponent<Animator>();
        shootAnimation = Animator.StringToHash("Rifle_Shooting");
        moveXAnimationParameterID = Animator.StringToHash("MoveX");
        moveZAnimationParameterID = Animator.StringToHash("MoveZ");
        //crouchAnimation = Animator.StringToHash("Crouch");

        // выключение курсора во время игры и лок его на центре
        Cursor.lockState = CursorLockMode.Locked; 
    }

    private void Update()
    {
        if (crouchAction.inProgress)
        {
            animator.SetInteger("MoveState", 1);
            currentSpeed = crouchSpeed;
        }
        else if (sprintAction.inProgress)
        {
            animator.SetInteger("MoveState", 2);
            currentSpeed = sprintSpeed;
        }
        else
        {
            animator.SetInteger("MoveState", 0);
            currentSpeed = walkSpeed;
        }


        // чтение инпута в виде вектора
        Vector2 input = moveAction.ReadValue<Vector2>();


        // "смягчение" данных input, чтобы анимации были плавнее
        currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, input, ref animationVelocity, animationSmoothTime);
        Vector3 move = new Vector3(currentAnimationBlendVector.x, 0, currentAnimationBlendVector.y);


        // воздействие гравитации и модель движения игрока
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
            playerVelocity.y = 0f;

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);


        // движение относительно камеры
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;  
        move.y = 0f;
        controller.Move(move * Time.deltaTime * currentSpeed);


        // передача в аниматор данных инпута
        animator.SetFloat(moveXAnimationParameterID, currentAnimationBlendVector.x);
        animator.SetFloat(moveZAnimationParameterID, currentAnimationBlendVector.y);


        // поворот в сторону курсора
        float targetAngle = cameraTransform.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);


        // определение точки для стрельбы
        Ray _ray = new Ray(cameraTransform.position, cameraTransform.forward);
        float _rayDistance = 100f;
        _aimPoint = _ray.GetPoint(_rayDistance);


        // прицеливание оружия в точку
        Aim(_aimPoint);

    }

    private void OnEnable()
    {
        shootAction.performed += _ => ShootGun();
        reloadAction.performed += _ => Reload();
    }

    private void OnDisable()
    {
        shootAction.performed -= _ => ShootGun();
        reloadAction.performed -= _ => Reload();
    }

    private void ShootGun()
    {
        animator.CrossFade(shootAnimation, animationSmoothTime);
        _currentGun.OnTriggerHold(_aimPoint);
    }

    private void Aim(Vector3 _aimPoint)
    {
        _currentGun.Aim(_aimPoint);
    }

    private void Reload()
    {
        _currentGun.Reload();
    }
}