using UnityEngine;

public class HashAnimationNames 
{
    protected HashAnimationNames _animationBase;

    public int IdleHash = Animator.StringToHash("Idle");
    public int WalkForwardHash = Animator.StringToHash("Walk_Forward");
    public int WalkBackwardHash = Animator.StringToHash("Walk_Backward");
    public int WalkLeftHash = Animator.StringToHash("Walk_Left");
    public int WalkRightHash = Animator.StringToHash("Walk_Right");
    public int KneelHash = Animator.StringToHash("Kneel");
}
