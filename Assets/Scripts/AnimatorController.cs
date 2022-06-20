using UnityEngine;

// висит на игроке и любом анимированном персонаже. Принимает команды от базовых скриптов "PLayer" или "Enemy"
public class AnimatorController : MonoBehaviour
{
    private Animator _characterAnimator;
    private string _currentState;


    // Animation States
    const string IDLE = "Rifle_Idle";
    const string IDLE_SHOOT = "Rifle_Idle_SimpleShoot";
    const string RUN_FORWARD = "Rifle_Run_Forward";
    const string RUN_BACKWARD = "Rifle_Run_Backward";
    const string RUN_SHOOT = "Rifle_Run_Shooting";
    //const string SPRINT = "Sprint";
    //const string SPRINT_SHOOT = "Sprint_Shoot";
    const string LEFTTURN = "Rifle_Left_turn";
    const string RIGHTTURN = "Rifle_Right_Turn";
    const string LEFTSTRAFE = "Rifle_Strafe_Left";
    const string RIGHTSTRAFE = "Rifle_Strafe_Right";


    private void Start()
    {
        _characterAnimator = GetComponent<Animator>();
        _characterAnimator.Play("Rifle_Idle");
    }

    private void ChangeState(string _newState)
    {
        if (_currentState == _newState)
            return;

        _characterAnimator.Play(_newState);

        _currentState = _newState;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
            ChangeState(RUN_FORWARD);
        
        else if (Input.GetKey(KeyCode.A))
            ChangeState(LEFTSTRAFE);
        
        else if (Input.GetKey(KeyCode.D))
            ChangeState(RIGHTSTRAFE);
        
        else if (Input.GetKey(KeyCode.S))
            ChangeState(RUN_BACKWARD);
       
        else if(Input.GetKey(KeyCode.W) && Input.GetMouseButton(0))
            ChangeState(RUN_SHOOT);
        
        else if (Input.GetMouseButton(0))
            ChangeState(IDLE_SHOOT);
        
        else
            ChangeState(IDLE);
        
    }
}