using UnityEngine;

// Висит на игроке
public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private Gun _currentGun;
    [SerializeField]
    private Transform _crosshair;
    [SerializeField]
    private Transform _eyes;
    private Camera _viewCamera;
    private PlayerController _controller;
    private HashAnimationNames _animationBase;
    private Animator _characterAnimator;
    [SerializeField]
    private float _moveSpeed = 3.4f;
    
    private Vector3 _point;

    private Player_States _state;
    private int _currentState;

    private int IDLE;
    private int WALK_FORWARD;
    private int WALK_FORWARD_LEFT;
    private int WALK_FORWARD_RIGHT;
    private int WALK_BACKWARD;
    private int WALK_BACKWARD_LEFT;
    private int WALK_BACKWARD_RIGHT;
    private int WALK_LEFT;
    private int WALK_RIGHT;
    private int CROUCH_IDLE;
    private int CROUCH_FORWARD;
    private int CROUCH_FORWARD_LEFT;
    private int CROUCH_FORWARD_RIGHT;
    private int CROUCH_BACKWARD;
    private int CROUCH_BACKWARD_LEFT;
    private int CROUCH_BACKWARD_RIGHT;
    private int CROUCH_LEFT;
    private int CROUCH_RIGHT;


    public enum Player_States
    {
        IDLE,
        WALK_FORWARD,
        WALK_FORWARD_LEFT,
        WALK_FORWARD_RIGHT,
        WALK_BACKWARD,
        WALK_BACKWARD_LEFT,
        WALK_BACKWARD_RIGHT,
        WALK_LEFT,
        WALK_RIGHT,
        CROUCH_IDLE,
        CROUCH_FORWARD,
        CROUCH_FORWARD_LEFT,
        CROUCH_FORWARD_RIGHT,
        CROUCH_BACKWARD,
        CROUCH_BACKWARD_LEFT,
        CROUCH_BACKWARD_RIGHT,
        CROUCH_LEFT,
        CROUCH_RIGHT
    }


    private void Awake()
    {
        _animationBase = new HashAnimationNames();
        _characterAnimator = GetComponent<Animator>();
        _controller = GetComponent<PlayerController>();

        _viewCamera = Camera.main;

        // инициализация хэшированных анимаций 
        IDLE = _animationBase.IdleHash;
        WALK_FORWARD = _animationBase.WalkForwardHash;
        WALK_FORWARD_LEFT = _animationBase.WalkForwardLeftHash;
        WALK_FORWARD_RIGHT = _animationBase.WalkForwardRightHash;
        WALK_BACKWARD = _animationBase.WalkBackwardHash;
        WALK_BACKWARD_LEFT = _animationBase.WalkBackwardLeftHash;
        WALK_BACKWARD_RIGHT = _animationBase.WalkBackwardRightHash;
        WALK_LEFT = _animationBase.WalkLeftHash;
        WALK_RIGHT = _animationBase.WalkRightHash;
        CROUCH_IDLE = _animationBase.CrouchIdleHash;
        CROUCH_FORWARD = _animationBase.CrouchForwardHash;
        CROUCH_FORWARD_LEFT = _animationBase.CrouchForwardLeftHash;
        CROUCH_FORWARD_RIGHT = _animationBase.CrouchForwardRightHash;
        CROUCH_BACKWARD = _animationBase.CrouchBackwardHash;
        CROUCH_BACKWARD_LEFT = _animationBase.CrouchBackwardLeftHash;
        CROUCH_BACKWARD_RIGHT = _animationBase.CrouchBackwardRightHash;
        CROUCH_LEFT = _animationBase.CrouchLeftHash;
        CROUCH_RIGHT = _animationBase.CrouchRightHash;
    }


    private void FixedUpdate()
    {
        Vector3 _moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")); 
        Vector3 _moveVelocity = _moveInput.normalized * _moveSpeed;
        _controller.Move(_moveVelocity);
        
        Ray _ray = _viewCamera.ScreenPointToRay(Input.mousePosition);     
        Plane _groundPlane = new Plane(Vector3.up, Vector3.up * _eyes.position.y);
        float _rayDistance;                                             

        if (_groundPlane.Raycast(_ray, out _rayDistance))                            
        {
            //Vector3 _point = _ray.GetPoint(_rayDistance);
            _point = _ray.GetPoint(_rayDistance);
            _controller.LookAt(_point);
            _crosshair.position = _point;
            
            if ((new Vector2(_point.x, _point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
                Aim(_point);
        }


        if (Input.GetMouseButton(0))  
            _currentGun.OnTriggerHold(_point);
        
        
        if (Input.GetMouseButtonUp(0))
            _currentGun.OnTriggerRelease();
        

        if (Input.GetKey(KeyCode.R))
        {
            _currentGun.Reload();
            Debug.Log("R");
        }
            


        if (Input.GetKey(KeyCode.W))
            _state = Player_States.WALK_FORWARD;


        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
            _state = Player_States.WALK_FORWARD_LEFT;


        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
            _state = Player_States.WALK_FORWARD_RIGHT;


        if (Input.GetKey(KeyCode.S))
            _state = Player_States.WALK_BACKWARD;


        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
            _state = Player_States.WALK_BACKWARD_LEFT;


        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
            _state = Player_States.WALK_BACKWARD_RIGHT;


        if (Input.GetKey(KeyCode.A))
            _state = Player_States.WALK_LEFT;


        if (Input.GetKey(KeyCode.D))
            _state = Player_States.WALK_RIGHT;


        if (Input.GetKey(KeyCode.LeftControl))
        {
            _state = Player_States.CROUCH_IDLE;
            _moveSpeed = 2.2f;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            _moveSpeed = 3.4f;
        }



        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.W))
            _state = Player_States.CROUCH_FORWARD;


        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
            _state = Player_States.CROUCH_FORWARD_LEFT;


        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
            _state = Player_States.CROUCH_FORWARD_RIGHT;


        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.S))
            _state = Player_States.CROUCH_BACKWARD;


        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
            _state = Player_States.CROUCH_BACKWARD_LEFT;


        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
            _state = Player_States.CROUCH_BACKWARD_RIGHT;


        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.A))
            _state = Player_States.CROUCH_LEFT;


        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.D))
            _state = Player_States.CROUCH_RIGHT;


        if (!Input.anyKey)
            _state = Player_States.IDLE;


        switch (_state)
        {
            case Player_States.IDLE:
                ChangeState(IDLE);
                break;
            case Player_States.WALK_FORWARD:
                ChangeState(WALK_FORWARD);
                break;
            case Player_States.WALK_FORWARD_LEFT:
                ChangeState(WALK_FORWARD_LEFT);
                break;
            case Player_States.WALK_FORWARD_RIGHT:
                ChangeState(WALK_FORWARD_RIGHT);
                break;
            case Player_States.WALK_BACKWARD:
                ChangeState(WALK_BACKWARD);
                break;
            case Player_States.WALK_BACKWARD_LEFT:
                ChangeState(WALK_BACKWARD_LEFT);
                break;
            case Player_States.WALK_BACKWARD_RIGHT:
                ChangeState(WALK_BACKWARD_RIGHT);
                break;
            case Player_States.WALK_LEFT:
                ChangeState(WALK_LEFT);
                break;
            case Player_States.WALK_RIGHT:
                ChangeState(WALK_RIGHT);
                break;
            case Player_States.CROUCH_IDLE:
                ChangeState(CROUCH_IDLE);
                break;
            case Player_States.CROUCH_FORWARD:
                ChangeState(CROUCH_FORWARD);
                break;
            case Player_States.CROUCH_FORWARD_LEFT:
                ChangeState(CROUCH_FORWARD_LEFT);
                break;
            case Player_States.CROUCH_FORWARD_RIGHT:
                ChangeState(CROUCH_FORWARD_RIGHT);
                break;
            case Player_States.CROUCH_BACKWARD:
                ChangeState(CROUCH_BACKWARD);
                break;
            case Player_States.CROUCH_BACKWARD_LEFT:
                ChangeState(CROUCH_BACKWARD_LEFT);
                break;
            case Player_States.CROUCH_BACKWARD_RIGHT:
                ChangeState(CROUCH_BACKWARD_RIGHT);
                break;
            case Player_States.CROUCH_LEFT:
                ChangeState(CROUCH_LEFT);
                break;
            case Player_States.CROUCH_RIGHT:
                ChangeState(CROUCH_RIGHT);
                break;
            default:
                ChangeState(IDLE);
                break;
        }
    }

    public void Aim(Vector3 _aimPoint)
    {
        _currentGun.Aim(_aimPoint);
    }

    private void ChangeState(int _newState)
    {
        if (_currentState == _newState)
            return;

        _characterAnimator.Play(_newState);

        _currentState = _newState;
    }
}