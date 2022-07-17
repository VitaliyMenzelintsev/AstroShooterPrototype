﻿using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _walkSpeed = 3.4f;
    [SerializeField]
    private float _crouchSpeed = 2.6f;
    [SerializeField]
    private float _sprintSpeed = 4.0f;
    [SerializeField]
    private float _currentSpeed;
    [SerializeField]
    private float _rotationSpeed = 5f;
    private Transform _cameraTransform;
    [SerializeField]
    private BaseGun _currentGun;
    [SerializeField]
    private float _animationSmoothTime = 0.2f;  // смягчение скорости для анимации

    private float _gravityValue = -9.81f;

    private CharacterController _controller;
    private PlayerInput _playerInput;

    private Vector3 _playerVelocity;
    private bool _groundedPlayer;

    private InputAction _moveAction;
    private InputAction _crouchAction; // ?
    private InputAction _sprintAction; // ?
    private InputAction _shootAction;
    private InputAction _reloadAction;

    private Vector3 _viewPoint;

    private Animator _animator;
    private int _moveX;
    private int _moveZ;
    private int _shootAnimation;

    private Vector2 _currentAnimationBlendVector;
    private Vector2 _animationVelocity;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        _cameraTransform = Camera.main.transform;

        _moveAction = _playerInput.actions["Move"];
        _crouchAction = _playerInput.actions["Crouch"];
        _sprintAction = _playerInput.actions["Sprint"];
        _shootAction = _playerInput.actions["Shoot"];
        _reloadAction = _playerInput.actions["Reload"];

        _animator = GetComponent<Animator>();
        _shootAnimation = Animator.StringToHash("Rifle_Shooting");
        _moveX = Animator.StringToHash("MoveX");
        _moveZ = Animator.StringToHash("MoveZ");

    }

    private void Update()
    {
        if (_crouchAction.inProgress)
        {
            _animator.SetInteger("MoveState", 1);
            _currentSpeed = _crouchSpeed;
        }
        else if (_sprintAction.inProgress)
        {
            _animator.SetInteger("MoveState", 2);
            _currentSpeed = _sprintSpeed;
        }
        else
        {
            _animator.SetInteger("MoveState", 0);
            _currentSpeed = _walkSpeed;
        }


        Move();


        VectorSending();

        
        CursorDetermination();


        Aim(_viewPoint);


        RotationTowardsCursor();
    }


    private void Move()
    {
        // воздействие гравитации и модель движения игрока
        _groundedPlayer = _controller.isGrounded;


        if (_groundedPlayer && _playerVelocity.y < 0)
            _playerVelocity.y = 0f;


        _playerVelocity.y += _gravityValue * Time.deltaTime;
        _controller.Move(_playerVelocity * Time.deltaTime);


        Vector2 input = _moveAction.ReadValue<Vector2>();


        // "смягчение" данных input, чтобы анимации были плавнее
        _currentAnimationBlendVector = Vector2.SmoothDamp(_currentAnimationBlendVector, input, ref _animationVelocity, _animationSmoothTime);
        Vector3 move = new Vector3(_currentAnimationBlendVector.x, 0, _currentAnimationBlendVector.y);


        // движение относительно камеры
        move = move.x * _cameraTransform.right.normalized + move.z * _cameraTransform.forward.normalized;
        move.y = 0f;
        _controller.Move(move * Time.deltaTime * _currentSpeed);
    }


    private void VectorSending()
    {
        // передача в аниматор данных инпута
        _animator.SetFloat(_moveX, _currentAnimationBlendVector.x);
        _animator.SetFloat(_moveZ, _currentAnimationBlendVector.y);
    }


    private void CursorDetermination()
    {
        Ray _ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit _hit;
        float _rayDistance = 50f;

        if (Physics.Raycast(_ray, out _hit, _rayDistance))
        {
            _viewPoint = _hit.point;
        }
    }


    private void RotationTowardsCursor()
    {
        Vector3 _lookPoint = _viewPoint;
        _lookPoint.y = transform.position.y;
        Quaternion targetRotation = Quaternion.LookRotation(_lookPoint - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }


    private void OnEnable()
    {
        _shootAction.performed += _ => ShootGun();
        _reloadAction.performed += _ => Reload();
    }


    private void OnDisable()
    {
        _shootAction.performed -= _ => ShootGun();
        _reloadAction.performed -= _ => Reload();
    }


    private void ShootGun()
    {
        _animator.CrossFade(_shootAnimation, _animationSmoothTime);
        _currentGun.Shoot(_viewPoint);
    }


    private void Aim(Vector3 _aimPoint)
    {
        Mathf.Clamp(_aimPoint.y, -60, 60);
        _currentGun.Aim(_aimPoint);
    }


    private void Reload()
    {
        _currentGun.Reload();
    }
}