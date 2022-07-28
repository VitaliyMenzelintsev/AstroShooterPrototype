using UnityEngine;
using UnityEngine.AI;

public class EnemyRangeBehavior : EnemyBaseBehavior
{
    private NavMeshAgent _navMeshAgent;
    private Animator _characterAnimator;
    private CoverManager _coverManager;
    private EnemyCoverSpot _currentCover = null;


    public override void Start()
    {
        base.Start();

        _navMeshAgent = GetComponent<NavMeshAgent>();

        _characterAnimator = GetComponent<Animator>();

        _coverManager = GameObject.FindObjectOfType<CoverManager>();
    }


    private void FixedUpdate()
    {
        if (!MyVitals.IsAlive())
        {
            StateDeath();
            return;
        }
        if (IsTargetAlive())
        {
            if (!IsCoverExist())
                _currentCover = _coverManager.GetCover(this, CurrentTarget);

            if (IsCoverExist())
            {
                if (IsNotInCover())
                {
                    StateMoveToCover();
                }
                else
                {
                    StateCombat();
                }
            }
            else
            {
                StateCombat();
            }
        }
        else
        {
            GetNewTarget();

            StateIdle();
        }
    }


    public override void StateSkill(bool _isESkill, GameObject _target) { }


    private void StateCombat()
    {
        if (IsRangeDistance())
        {
            StateRangeCombat();
        }
        else if (IsMeleeDistance())
        {
            StateMeleeCombat();
        }
    }


    private void StateDeath()
    {
        if (_navMeshAgent != null
        && _navMeshAgent.speed != 0)
            _navMeshAgent.speed = 0;

        _characterAnimator.SetBool("Move", false);

        _characterAnimator.SetBool("Dead", true);

        ExitCover();

        Destroy(GetComponent<CapsuleCollider>());

        Destroy(gameObject, 10f);
    }


    private void StateIdle()
    {
        _characterAnimator.SetBool("Move", false);

        _characterAnimator.SetBool("HasEnemy", false);
    }


    private void StateMoveToCover()
    {
        _characterAnimator.SetBool("Move", true);

        _characterAnimator.SetBool("HasEnemy", true);

        _navMeshAgent.SetDestination(_currentCover.transform.position);
    }


    private void StateRangeCombat()
    {
        transform.LookAt(CurrentTarget.transform);

        _characterAnimator.SetBool("Move", false);

        _characterAnimator.SetTrigger("Fire");

        _currentGun.Aim(CurrentTarget.GetComponent<BaseCharacter>().GetHeadTransform().position);

        _currentGun.Shoot(CurrentTarget.GetComponent<BaseCharacter>().GetHeadTransform().position);

    }


    private void StateMeleeCombat()
    {
        ExitCover();

        _characterAnimator.SetBool("Move", false);

        _characterAnimator.SetTrigger("Punch");

        _currentGun.Punch();
    }


    private bool IsMeleeDistance()
    {
        if (Vector3.Distance(transform.position, CurrentTarget.transform.position) < _minAttackDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private bool IsRangeDistance()
    {
        if (Vector3.Distance(transform.position, CurrentTarget.transform.position) <= _maxAttackDistance
                && Vector3.Distance(transform.position, CurrentTarget.transform.position) >= _minAttackDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private void ExitCover()
    {
        if (_currentCover != null)
            _coverManager.ExitCover(ref _currentCover);
    }


    private bool IsNotInCover()
    {
        if (Vector3.Distance(this.gameObject.transform.position, _currentCover.transform.position) > 0.3F)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private bool IsCoverExist()
    {
        if (_currentCover != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
