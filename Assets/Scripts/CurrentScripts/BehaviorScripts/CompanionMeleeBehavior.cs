using UnityEngine;
using UnityEngine.AI;


public class CompanionMeleeBehavior : CompanionBaseBehavior
{
    public override void Start()
    {
        base.Start();

        _navMeshAgent = GetComponent<NavMeshAgent>();

        _characterAnimator = GetComponent<Animator>();

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

        if (IsTargetAlive())
        {
            if (IsDistanceCorrect())
            {
                StateCombat();
            }
            else
            {
                StateInvestigate();
            }

        }
        else
        {
            GetNewTarget();

            if (IsTargetAlive())
            {
                StateIdle();
            }
            else
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
        }
    }


    private void StateDeath()
    {
        _characterAnimator.SetBool("Dead", true);

        //if (_navMeshAgent.speed != 0)
        //    _navMeshAgent.speed = 0;


        if (_isDead)
            _isDead = true;

        if (_navMeshAgent.isStopped != true)
            _navMeshAgent.isStopped = true;


        for (int i = 0; i < _myColliders.Length; i++)
        {
            _myColliders[i].enabled = false;
        }
    }



    public override void StateIdle()
    {
        //_navMeshAgent.speed = 0;

        _characterAnimator.SetBool("HasEnemy", false);
    }



    private void StateFollowThePlayer()
    {
        _navMeshAgent.speed = _speed;

        _characterAnimator.SetBool("HasEnemy", false);

        transform.LookAt(_player);

        _navMeshAgent.isStopped = false;

        _navMeshAgent.stoppingDistance = 0.2f;

        _navMeshAgent.SetDestination(_followPoint.position);
    }



    private void StateInvestigate()
    {
        if (Vector3.Distance(_navMeshAgent.transform.position, CurrentTarget.transform.position) <= (_maxAttackDistance - _minAttackDistance) / 2)
        {
            _navMeshAgent.isStopped = true;
        }

        _navMeshAgent.speed = _speed;

        _characterAnimator.SetBool("HasEnemy", true);

        _navMeshAgent.stoppingDistance = (_maxAttackDistance - _minAttackDistance) / 2;

        _navMeshAgent.SetDestination(CurrentTarget.transform.position);
    }



    private void StateCombat()
    {
        _characterAnimator.SetTrigger("Fire");

        _navMeshAgent.speed = 0;

        transform.LookAt(CurrentTarget.transform);

        Vector3 _fixedAimPosition = CurrentTarget.GetComponent<BaseCharacter>().GetHeadTransform().position;

        _fixedAimPosition.y -= 0.3f;

        _currentGun.Aim(_fixedAimPosition);

        _currentGun.Shoot(_fixedAimPosition);

    }



    private void SetAnimations()
    {
        _characterAnimator.SetFloat("Speed", _navMeshAgent.velocity.magnitude);
    }
}
