using UnityEngine;

// висит на игроке и любом анимированном персонаже. Принимает команды от базовых скриптов "PLayer" или "Enemy"
public class AnimatorController : MonoBehaviour
{
    private Animator _characterAnimator;
    private string _currentState;


    // Animation States
    const string IDLE = "Idle";
    const string IDLE_SHOOT = "Idle_Shot";
    const string WALK_FORWARD = "Walk_Forward";
    const string WALK_BACKWARD = "Walk_Backward";
    //const string RUN_SHOOT = "Rifle_Run_Shooting";
    //const string SPRINT = "Sprint";
    //const string SPRINT_SHOOT = "Sprint_Shoot";
    //const string LEFTTURN = "Rifle_Left_turn";
    //const string RIGHTTURN = "Rifle_Right_Turn";
    const string WALKLEFT = "Walk_Left";
    const string WALKRIGHT = "Walk_Right";


    private void Start()
    {
        _characterAnimator = GetComponent<Animator>();
        _characterAnimator.Play("Idle");
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
            ChangeState(WALK_FORWARD);
        
        else if (Input.GetKey(KeyCode.A))
            ChangeState(WALKLEFT);
        
        else if (Input.GetKey(KeyCode.D))
            ChangeState(WALKRIGHT);
        
        else if (Input.GetKey(KeyCode.S))
            ChangeState(WALK_BACKWARD);
       
        //else if(Input.GetKey(KeyCode.W) && Input.GetMouseButton(0))
        //    ChangeState(RUN_SHOOT);
        
        else if (Input.GetMouseButton(0))
            ChangeState(IDLE_SHOOT);
        
        else
            ChangeState(IDLE);
        
    }
}