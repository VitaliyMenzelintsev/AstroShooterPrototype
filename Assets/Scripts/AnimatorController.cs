using UnityEngine;

// висит на игроке и любом анимированном персонаже. Принимает команды от базовых скриптов "PLayer" или "Enemy"
public class AnimatorController : MonoBehaviour
{
    private Animator _characterAnimator;
    private int _currentState;

    protected HashAnimationNames _animBase = new HashAnimationNames();

    // Старая версия анимаций
    //const string IDLE = "Idle";
    //const string WALK_FORWARD = "Walk_Forward";
    //const string WALK_BACKWARD = "Walk_Backward";
    //const string WALKLEFT = "Walk_Left";
    //const string WALKRIGHT = "Walk_Right";

    // Хэшированные анимации
    public int IDLE;
    public int WALK_FORWARD;
    public int WALK_BACKWARD;
    public int WALKLEFT;
    public int WALKRIGHT;

    // Не реализованные анимации
    //const string IDLE_SHOOT = "Idle_Shot";
    //const string RUN_SHOOT = "Rifle_Run_Shooting";
    //const string SPRINT = "Sprint";
    //const string SPRINT_SHOOT = "Sprint_Shoot";
    //const string LEFTTURN = "Rifle_Left_turn";
    //const string RIGHTTURN = "Rifle_Right_Turn";

    private void Start()
    {
        _characterAnimator = GetComponent<Animator>();
        //_characterAnimator.Play("Idle");

        // инициализация хэшированных анимаций 
        IDLE = _animBase.IdleHash;
        WALK_FORWARD = _animBase.WalkForwardHash;
        WALK_BACKWARD = _animBase.WalkBackwardHash;
        WALKLEFT = _animBase.WalkLeftHash;
        WALKRIGHT = _animBase.WalkRightHash;

        ChangeState(IDLE);
    }

    private void ChangeState(int _newState)
    {
        if (_currentState == _newState)
            return;

        _characterAnimator.Play(_newState);

        _currentState = _newState;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
            ChangeState(WALK_FORWARD);
        
        else if (Input.GetKey(KeyCode.A))
            ChangeState(WALKLEFT);
        
        else if (Input.GetKey(KeyCode.D))
            ChangeState(WALKRIGHT);
        
        else if (Input.GetKey(KeyCode.S))
            ChangeState(WALK_BACKWARD);
       
        //else if(Input.GetKey(KeyCode.W) && Input.GetMouseButton(0))
        //    ChangeState(RUN_SHOOT);
        
        else
            ChangeState(IDLE);

        //if (_currentState == IDLE && Input.GetMouseButton(0))
        //    ChangeState(IDLE_SHOOT);
    }
}