using UnityEngine;

// висит на игроке и любом анимированном персонаже. Принимает команды от базовых скриптов "PLayer" или "Enemy"
public class NPCController : MonoBehaviour
{
    private Animator _characterAnimator;
    private int _currentState;

    protected HashAnimationNames _animationBase = new HashAnimationNames();

    // Хэшированные анимации
    [HideInInspector] public int IDLE;
    [HideInInspector] public int WALK_FORWARD;
    [HideInInspector] public int WALK_BACKWARD;
    [HideInInspector] public int WALKLEFT;
    [HideInInspector] public int WALKRIGHT;

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

        // инициализация хэшированных анимаций 
        IDLE = _animationBase.IdleHash;
        WALK_FORWARD = _animationBase.WalkForwardHash;
        WALK_BACKWARD = _animationBase.WalkBackwardHash;
        WALKLEFT = _animationBase.WalkLeftHash;
        WALKRIGHT = _animationBase.WalkRightHash;

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
   
        else
            ChangeState(IDLE);
    }
}