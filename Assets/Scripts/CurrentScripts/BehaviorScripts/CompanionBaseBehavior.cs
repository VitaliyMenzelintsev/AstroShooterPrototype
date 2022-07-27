using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]

public abstract class CompanionBaseBehavior : BaseAIBehavior
{
    [SerializeField]
    protected Transform _followPoint;
    [SerializeField]
    protected Transform _player;
    protected NavMeshAgent _navMeshAgent;
    protected Animator _characterAnimator;
    protected CoverManager _coverManager;
    protected CompanionCoverSpot _currentCover = null;
    protected float _speed = 3.8f;
    protected float _resAnimationTime = 3.2f;
    public BaseActivatedSkill MyActivatedSkill; // в это поле в инспекторе кладём нужный скилл

    protected bool IsPlayerFar()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) > 2.5f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void StateSkill(bool _isESkill, GameObject _target)
    {
        if (GetComponent<Vitals>().IsAlive())
        {
            MyActivatedSkill.Activation(_isESkill, _target);
        }
    }

    public void GetRessurect()
    {
        if (MyVitals.IsAlive())
        {
            GetComponent<CapsuleCollider>().enabled = true;

            _characterAnimator.SetBool("Dead", false);

            _characterAnimator.SetBool("HasEnemy", false);

            Invoke("SetSpeed", _resAnimationTime);

            StateIdle();
        }
    }

    protected void SetSpeed()
    {
        _navMeshAgent.speed = _speed;
    }

    public abstract void StateIdle();
}
