using UnityEngine;

public class HashAnimationNames
{
    protected HashAnimationNames _animationBase;

    public int IdleHash = Animator.StringToHash("Idle");
    public int WalkForwardHash = Animator.StringToHash("Walk_Forward");
    public int WalkBackwardHash = Animator.StringToHash("Walk_Backward");
    public int WalkLeftHash = Animator.StringToHash("Walk_Left");
    public int WalkRightHash = Animator.StringToHash("Walk_Right");
    public int CrouchIdleHash = Animator.StringToHash("Crouch_Idle");
    public int CrouchLeftHash = Animator.StringToHash("Crouch_Left");
    public int CrouchRightHash = Animator.StringToHash("Crouch_Right");
    public int CrouchBackwardRightHash = Animator.StringToHash("Crouch_Backward_Right");
    public int CrouchBackwardLeftHash = Animator.StringToHash("Crouch_Backward_Left");
    public int CrouchBackwardHash = Animator.StringToHash("Crouch_Backward");
    public int CrouchForwardRightHash = Animator.StringToHash("Crouch_Forward_Right");
    public int CrouchForwardLeftHash = Animator.StringToHash("Crouch_Forward_Left");
    public int CrouchForwardHash = Animator.StringToHash("Crouch_Forward");
    public int WalkBackwardRightHash = Animator.StringToHash("Walk_Backward_Right");
    public int WalkBackwardLeftHash = Animator.StringToHash("Walk_Backward_Left");
    public int WalkForwardRightHash = Animator.StringToHash("Walk_Forward_Right");
    public int WalkForwardLeftHash = Animator.StringToHash("Walk_Forward_Left");
    public int SprintForwardHash = Animator.StringToHash("Sprint_Forward");
}
