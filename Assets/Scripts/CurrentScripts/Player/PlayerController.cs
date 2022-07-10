﻿using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed = 3.4f;
    [SerializeField]
    private float crouchSpeed = 2.6f;
    private float currentSpeed;
    [SerializeField]
    private float rotationSpeed = 5f;
    private Transform cameraTransform;
    [SerializeField]
    private Gun _currentGun;
    [SerializeField]
    private float animationSmoothTime = 0.2f;  // смягчение скорости для анимации
    [SerializeField]
    private float animationPlayTransition = 0.2f;
    [SerializeField]
    private Transform aimTarget;


    private CharacterController controller;
    private PlayerInput playerInput;
    //private Vector3 playerVelocity;

    private InputAction moveAction;
    private InputAction crouchAction;
    private InputAction shootAction;

    private Vector3 _aimPoint;

    private Animator animator;
    private int moveXAnimationParameterID;
    private int moveZAnimationParameterID;
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
        shootAction = playerInput.actions["Shoot"];

        animator = GetComponent<Animator>();
        moveXAnimationParameterID = Animator.StringToHash("MoveX");
        moveZAnimationParameterID = Animator.StringToHash("MoveZ");
        crouchAnimation = Animator.StringToHash("Crouch");

        // выключение курсора во время игры и лок его на центре
        Cursor.lockState = CursorLockMode.Locked; 
    }

    private void Update()
    {
        // определение точки для стрельбы
        Ray _ray = new Ray(cameraTransform.position, cameraTransform.forward);
        float _rayDistance = 100f;
        _aimPoint = _ray.GetPoint(_rayDistance);

        // прицеливание оружия в точку
        Aim(_aimPoint);

        // Присед вкл/выкл
        if (crouchAction.inProgress)
        {
            animator.SetBool("Crouch", true);
            currentSpeed = crouchSpeed;
        }
        else
        {
            animator.SetBool("Crouch", false);
            currentSpeed = walkSpeed;
        }

        // чтение инпута в виде вектора
        Vector2 input = moveAction.ReadValue<Vector2>();

        // "смягчение" данных input, чтобы анимации были плавнее
        currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, input, ref animationVelocity, animationSmoothTime);
        Vector3 move = new Vector3(currentAnimationBlendVector.x, 0, currentAnimationBlendVector.y);

        // движение игрока относительно камеры
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;  
        move.y = 0f;
        controller.Move(move * Time.deltaTime * currentSpeed);

        // передача в аниматор данных инпута
        animator.SetFloat(moveXAnimationParameterID, currentAnimationBlendVector.x);
        animator.SetFloat(moveZAnimationParameterID, currentAnimationBlendVector.y);

        //// отключил и всё работает ????
        //controller.Move(playerVelocity * Time.deltaTime);

        // поворот в сторону курсора
        float targetAngle = cameraTransform.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
        _currentGun.OnTriggerHold(_aimPoint);
    }

    public void Aim(Vector3 _aimPoint)
    {
        _currentGun.Aim(_aimPoint);
    }
}