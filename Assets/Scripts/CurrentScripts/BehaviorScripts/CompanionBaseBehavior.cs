using UnityEngine;
using UnityEngine.AI;


public abstract class CompanionBaseBehavior : BaseAIBehavior
{
    public BaseActivatedSkill MyActivatedSkill;
    [SerializeField]
    protected Transform _followPoint;
    [SerializeField]
    protected Transform _player;
    protected NavMeshAgent _navMeshAgent;
    protected Animator _characterAnimator;
    protected CoverManager _coverManager;
    protected CompanionCoverSpot _currentCover = null;
    protected float _resAnimationTime = 3.2f;

    protected Collider[] _myColliders;


    public override void Start()
    {
        base.Start();

        _myColliders = GetComponentsInChildren<Collider>();
    }


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
            for (int i = 0; i < _myColliders.Length; i++)
            {
                _myColliders[i].enabled = true;
            }

            _characterAnimator.SetBool("Dead", false);

            _characterAnimator.SetBool("HasEnemy", false);

            Invoke(nameof(SetSpeed), _resAnimationTime);

            StateIdle();
        }
    }



    protected void SetSpeed()
    {
        _navMeshAgent.speed = _speed;
    }



    public abstract void StateIdle();
}
