using UnityEngine;
using UnityEngine.AI;

public class CompanionRangeBehavior : CompanionBaseBehavior
{
    public override void Start()
    {
        base.Start();

        _navMeshAgent = GetComponent<NavMeshAgent>();

        _characterAnimator = GetComponent<Animator>();

        _coverManager = FindObjectOfType<CoverManager>();

        _navMeshAgent.stoppingDistance = 0.2f;

        _navMeshAgent.speed = _speed;
    }


    private void FixedUpdate()
    {
        if (!MyVitals.IsAlive())
        {
            StateDeath();
            return;
        }

        SetAnimations();

        if (!_isDead)
        {
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
                    StateInvestigate();
                }
            }
            else
            {
                GetNewTarget();

                if (IsFollowPointFar())
                {
                    if (IsPlayerFar())
                    {
                        StateFollowThePlayer();
                    }
                    else
                    {
                        StateIdle();
                    }
                }
                else
                {
                    StateIdle();
                }
            }
        }

        CheckStoppingDistance();
    }


    private void StateDeath()
    {
        _characterAnimator.SetBool("Dead", true);


        ExitCover();


        if (!_isDead)
            _isDead = true;


        if (_navMeshAgent.speed != 0)
            _navMeshAgent.speed = 0;


        for (int i = 0; i < _myColliders.Length; i++)
        {
            _myColliders[i].enabled = false;
        }
    }



    public override void StateIdle()
    {
        _navMeshAgent.speed = _speed;

        _currentGun.Aim(_lookPoint.position);

        _characterAnimator.SetBool("HasEnemy", false);
    }



    private void StateFollowThePlayer()
    {
        ExitCover();

        _navMeshAgent.speed = _speed;

        _characterAnimator.SetBool("HasEnemy", false);

        transform.LookAt(_player);

        _currentGun.Aim(_lookPoint.position);

        _navMeshAgent.SetDestination(_followPoint.position);
    }



    private void StateMoveToCover()
    {
        _navMeshAgent.speed = _speed;

        _characterAnimator.SetBool("HasEnemy", true);

        _currentGun.Aim(_lookPoint.position);

        _navMeshAgent.SetDestination(_currentCover.transform.position);
    }



    private void StateRangeCombat()
    {
        _navMeshAgent.speed = 0;

        transform.LookAt(CurrentTarget.transform);

        _characterAnimator.SetTrigger("Fire");

        _currentGun.Aim(CurrentTarget.GetComponent<BaseCharacter>().GetHeadTransform().position);

        _currentGun.Shoot(CurrentTarget.GetComponent<BaseCharacter>().GetHeadTransform().position);
    }



    private void StateMeleeCombat()
    {
        _navMeshAgent.speed = 0;

        ExitCover();

        _characterAnimator.SetTrigger("Punch");

        transform.LookAt(CurrentTarget.transform);

        _currentGun.Punch();
    }



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


    private void StateInvestigate()
    {
        if (Vector3.Distance(_navMeshAgent.transform.position, CurrentTarget.transform.position) <= (_maxAttackDistance - _minAttackDistance) / 2)
        {
            _navMeshAgent.speed = 0;
        }
        else
        {
            _navMeshAgent.speed = _speed;
        }

        _characterAnimator.SetBool("HasEnemy", true);

        _currentGun.Aim(_lookPoint.position);

        _navMeshAgent.SetDestination(CurrentTarget.transform.position);
    }



    private void CheckStoppingDistance()
    {
        if (_navMeshAgent.stoppingDistance != 0.2f)
        {
            _navMeshAgent.stoppingDistance = 0.2f;
        }
    }



    private bool IsFollowPointFar()
    {
        if (Vector3.Distance(transform.position, _followPoint.transform.position) > 0.3f)
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



    private bool IsMeleeDistance()
    {
        if (Vector3.Distance(transform.position, CurrentTarget.transform.position) <= _minAttackDistance)
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
        if (Vector3.Distance(transform.position, _currentCover.transform.position) > 0.2F)
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
