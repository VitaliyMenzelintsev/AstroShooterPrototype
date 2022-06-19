using UnityEngine;
using UnityEngine.AI;

// висит на игроке и любом анимированном персонаже
public class AnimatorController : MonoBehaviour
{
    private Animator _characterAnimator;
    private string _currentState;


    // Animation States
    const string IDLE = "";
    const string RUN = "";
    const string SPRINT = "";
    const string SHOOT = "";

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
}