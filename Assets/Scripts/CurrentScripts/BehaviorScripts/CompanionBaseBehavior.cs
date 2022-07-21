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
}
