using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;


[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]

public class PlayerController : BaseCharacter
{
    [SerializeField, Range(1, 6)]
    private float _walkSpeed = 4.4f;
    [SerializeField, Range(1, 6)]
    private float _crouchSpeed = 3.6f;
    [SerializeField, Range(1, 6)]
    private float _sprintSpeed = 5.0f;
    [SerializeField, Range(1, 6)]
    private float _currentSpeed;
    private float _rotationSpeed = 7f;
    private Transform _cameraTransform;

    private float _animationSmoothTime = 0.2f;  // смягчение скорости для анимации

    private float _gravityValue = -9.81f;

    private CharacterController _controller;
    private PlayerInput _playerInput;

    private Vector3 _playerVelocity;
    private bool _groundedPlayer;

    private InputAction _moveAction;
    private InputAction _crouchAction;
    private InputAction _sprintAction;
    private InputAction _shootAction;
    private InputAction _reloadAction;
    private InputAction _healPartyAction;
    private InputAction _skillEButtonAction;
    private InputAction _skillQButtonAction;

    private Vector3 _viewPoint;

    private Animator _animator;
    private int _moveX;
    private int _moveZ;
    private int _shootAnimation;

    private Vector3 _move;
    private Vector2 _blendVector;
    private Vector2 _animationVelocity;
    [SerializeField]
    private GameObject[] _companions;


    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
        _controller = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();

        _shootAction = _playerInput.actions["Shoot"];
        _moveAction = _playerInput.actions["Move"];
        _crouchAction = _playerInput.actions["Crouch"];
        _sprintAction = _playerInput.actions["Sprint"];
        _reloadAction = _playerInput.actions["Reload"];
        _healPartyAction = _playerInput.actions["HealParty"];
        _skillEButtonAction = _playerInput.actions["EButtonSkill"];
        _skillQButtonAction = _playerInput.actions["QButtonSkill"];

        _animator = GetComponent<Animator>();
        _shootAnimation = Animator.StringToHash("Rifle_Shooting");
        _moveX = Animator.StringToHash("MoveX");
        _moveZ = Animator.StringToHash("MoveZ");
    }


    private void FixedUpdate()
    {
        SetSpeed();


        Move();


        SetAnimation();


        CursorDetermination();


        Aim(_viewPoint);


        RotationTowardsCursor();


        GetNewTarget();


        Shooting();
    }


    private void Shooting()
    {
        if (_shootAction.inProgress)
        {
            ShootGun();
        }
    }



    private void Move()
    {
        // воздействие гравитации и модель движения игрока
        _groundedPlayer = _controller.isGrounded;


        if (_groundedPlayer && _playerVelocity.y < 0)
            _playerVelocity.y = 0f;


        _playerVelocity.y += _gravityValue * Time.deltaTime;
        _controller.Move(_playerVelocity * Time.deltaTime);


        Vector2 _input = _moveAction.ReadValue<Vector2>();


        // "смягчение" данных input, чтобы анимации были плавнее
        _blendVector = Vector2.SmoothDamp(_blendVector, _input, ref _animationVelocity, _animationSmoothTime);
       _move = new Vector3(_blendVector.x, 0, _blendVector.y);


        // движение относительно камеры
        _move = _move.x * _cameraTransform.right.normalized + _move.z * _cameraTransform.forward.normalized * 2;
        _move.y = 0f;

        _controller.Move(_move * Time.fixedDeltaTime * _currentSpeed);
    }


    private void RotationTowardsCursor()
    {
        Vector3 _lookPoint = _viewPoint;
        _lookPoint.y = transform.position.y;
        Quaternion targetRotation = Quaternion.LookRotation(_lookPoint - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }



    private void CursorDetermination()
    {
        Ray _ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit _hit;
        float _rayDistance = 50f;

        if (Physics.Raycast(_ray, out _hit, _rayDistance))
        {
            _viewPoint = _hit.point;

            if (_hit.collider != null
                     && _hit.collider.GetComponent<Vitals>())      // TryGetComponent(out IDamageable _damageableObject)
            {
                CurrentTarget = _hit.collider.gameObject;
            }
        }
    }


    public Vector3 GetViewPoint()
    {
        return _viewPoint;
    }



    private void SetSpeed()
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
    }



    private void SetAnimation()
    {
        Vector3 _localMove = transform.InverseTransformDirection(_move);

        // передача в аниматор данных инпута
        _animator.SetFloat(_moveX, _localMove.x);
        _animator.SetFloat(_moveZ, _localMove.z);
    }



    private new void GetNewTarget()
    {
        if(CurrentTarget == null)
        CurrentTarget = _targetManager.GetNewTarget(_myTeamNumber, Eyes, true);
    }



    private void OnEnable()
    {
        //_shootAction.performed += _ => ShootGun();
        _reloadAction.performed += _ => Reload();
        _healPartyAction.performed += _ => HealParty();
        _skillEButtonAction.performed += _ => ActivateESkill();
        _skillQButtonAction.performed += _ => ActivateQSkill();
    }



    private void OnDisable()
    {
        /*_shootAction.performed -= _ => ShootGun()*/;
        _reloadAction.performed -= _ => Reload();
        _healPartyAction.performed -= _ => HealParty();
        _skillEButtonAction.performed -= _ => ActivateESkill();
        _skillQButtonAction.performed -= _ => ActivateQSkill();
    }



    private void ShootGun()
    {
        _animator.CrossFade(_shootAnimation, _animationSmoothTime);
        _currentGun.Shoot(_viewPoint);
    }



    private void HealParty()
    {
        for (int i = 0; i < _companions.Length; i++)
        {
            if (_companions[i].GetComponent<Vitals>().IsAlive())
            {
                _companions[i].GetComponent<Vitals>().GetHeal(100f);
            }
            else
            {
                _companions[i].GetComponent<Vitals>().GetRessurect();
            }
        }

        MyVitals.GetHeal(100f);
    }




    private void SpeedUpParty() //если нажата кнопка увеличивается скорость напарников
    {
        for (int i = 0; i < _companions.Length; i++)
        {
            _companions[i].GetComponent<NavMeshAgent>().speed += 1.5f;
        }
    }




    //private void SetPriorityTarget() // по нажатию Space заставляем компаньонов атаковать цель игрока 
    //{
    //    for (int i = 0; i < _companions.Length; i++)
    //    {
    //        _companions[i].GetComponent<CompanionBaseBehavior>().CurrentTarget = _currentGun.CurrentTarget;
    //    }
    //}



    private void ActivateESkill()
    {
        for (int i = 0; i < _companions.Length; i++)
        {
            Debug.Log("Игрок отдал приказ применить способность");
            _companions[i].GetComponent<CompanionBaseBehavior>().StateSkill(true, CurrentTarget);
        }
    }



    private void ActivateQSkill()
    {
        for (int i = 0; i < _companions.Length; i++)
        {

            _companions[i].GetComponent<CompanionBaseBehavior>().StateSkill(false, CurrentTarget);
        }
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



    public void SpeedChange(float _value)
    {
        _walkSpeed += _value;
        _crouchSpeed += _value;
        _sprintSpeed += _value;
    }
}