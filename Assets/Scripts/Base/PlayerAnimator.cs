using UnityEngine;

// висит на игроке
public class PlayerAnimator : MonoBehaviour
{
    private Animator _characterAnimator;
    private int _currentState;

    protected HashAnimationNames _animationBase = new HashAnimationNames();

    // Хэшированные анимации
    [HideInInspector] public int IDLE;
    [HideInInspector] public int WALK_FORWARD;
    [HideInInspector] public int WALK_FORWARD_LEFT;
    [HideInInspector] public int WALK_FORWARD_RIGHT;
    [HideInInspector] public int WALK_BACKWARD;
    [HideInInspector] public int WALK_BACKWARD_LEFT;
    [HideInInspector] public int WALK_BACKWARD_RIGHT;
    [HideInInspector] public int WALK_LEFT;
    [HideInInspector] public int WALK_RIGHT;
    [HideInInspector] public int CROUCH_IDLE;
    [HideInInspector] public int CROUCH_FORWARD;
    [HideInInspector] public int CROUCH_FORWARD_LEFT;
    [HideInInspector] public int CROUCH_FORWARD_RIGHT;
    [HideInInspector] public int CROUCH_BACKWARD;
    [HideInInspector] public int CROUCH_BACKWARD_LEFT;
    [HideInInspector] public int CROUCH_BACKWARD_RIGHT;
    [HideInInspector] public int CROUCH_LEFT;
    [HideInInspector] public int CROUCH_RIGHT;


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

    private Player_States _state;

    private void Start()
    {
        _characterAnimator = GetComponent<Animator>();

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

        //ChangeState(IDLE);
    }

    private void Update()
    {
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
            _state = Player_States.CROUCH_IDLE;

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
            case Player_States.CROUCH_LEFT:
                ChangeState(CROUCH_LEFT);
                break;
            default:
                ChangeState(IDLE);
                break;
        }
    }

    private void ChangeState(int _newState)
    {
        if (_currentState == _newState)
            return;

        _characterAnimator.Play(_newState);

        _currentState = _newState;
    }


    //private void FixedUpdate()
    //{
    //    if (Input.GetKey(KeyCode.W))
    //        ChangeState(WALK_FORWARD);

    //     if (Input.GetKey(KeyCode.A))
    //        ChangeState(WALKLEFT);

    //     if (Input.GetKey(KeyCode.D))
    //        ChangeState(WALKRIGHT);

    //     if (Input.GetKey(KeyCode.S))
    //        ChangeState(WALK_BACKWARD);

    //     if (Input.GetKey(KeyCode.LeftControl))
    //        ChangeState(KNEEL);

    //     //if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.A))
    //     //   ChangeState(CROUCH_LEFT);

    //     if(!Input.anyKey)
    //     ChangeState(IDLE);
    //}
}