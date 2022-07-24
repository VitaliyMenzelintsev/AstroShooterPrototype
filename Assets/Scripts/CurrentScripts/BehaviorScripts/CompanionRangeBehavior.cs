﻿using UnityEngine;
using UnityEngine.AI;

public class CompanionRangeBehavior : CompanionBaseBehavior
{
    public override void Start()
    {
        base.Start();

        _navMeshAgent = GetComponent<NavMeshAgent>();

        _characterAnimator = GetComponent<Animator>();

        _coverManager = GameObject.FindObjectOfType<CoverManager>();

        _navMeshAgent.stoppingDistance = 0.2f;

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

        SetAnimations();

        CheckStoppingDistance();

    }


    private void SetAnimations()
    {
        _characterAnimator.SetFloat("Speed", _navMeshAgent.velocity.magnitude);
    }



    private void CheckStoppingDistance()
    {
        if(_navMeshAgent.stoppingDistance != 0.2f)
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




    private void OnTriggerEnter(Collider other)
    {
        GetNewTarget();

        if (CurrentTarget == null)
        {
            MyVitals.GetRessurect();

            StateIdle();

            _characterAnimator.SetBool("Dead", false);

            _characterAnimator.SetBool("HasEnemy", false);
        }
    }



    private void StateDeath()
    {
        //_characterAnimator.SetBool("Move", false);

        _characterAnimator.SetBool("Dead", true);

        ExitCover();
    }



    private void StateIdle()
    {
        //_characterAnimator.SetBool("Move", false);

        _characterAnimator.SetBool("HasEnemy", false);
    }



    private void StateFollowThePlayer()
    {
        ExitCover();

        //_characterAnimator.SetBool("Move", true);

        _characterAnimator.SetBool("HasEnemy", false);

        _navMeshAgent.SetDestination(_followPoint.position);      // действие follow the player

    }



    private void StateMoveToCover()
    {
        //_characterAnimator.SetBool("Move", true);

        _characterAnimator.SetBool("HasEnemy", true);

        _navMeshAgent.SetDestination(_currentCover.transform.position);            // действие move to cover
    }



    private void StateRangeCombat()
    {
        transform.LookAt(CurrentTarget.transform);

        //_characterAnimator.SetBool("Move", false);

        _characterAnimator.SetTrigger("Fire");

        _currentGun.Aim(CurrentTarget.GetComponent<BaseCharacter>().GetEyesPosition().position);

        _currentGun.Shoot(CurrentTarget.GetComponent<BaseCharacter>().GetEyesPosition().position);  //  действие range combat
    }



    private void StateMeleeCombat()
    {
        ExitCover();

        //_characterAnimator.SetBool("Move", false);

        _characterAnimator.SetTrigger("Punch");  // действие melee combat

        _currentGun.Punch();
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
