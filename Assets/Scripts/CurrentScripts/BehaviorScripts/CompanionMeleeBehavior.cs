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

        if (_navMeshAgent.speed != 0)
            _navMeshAgent.speed = 0;

        GetComponent<CapsuleCollider>().enabled = false;
    }



    public override void StateIdle()
    {
        _navMeshAgent.speed = 0;

        _characterAnimator.SetBool("HasEnemy", false);
    }



    private void StateFollowThePlayer()
    {
        _navMeshAgent.speed = _speed;

        _characterAnimator.SetBool("HasEnemy", false);

        transform.LookAt(_player);

        _navMeshAgent.stoppingDistance = 0.2f;

        _navMeshAgent.SetDestination(_followPoint.position);            // действие Follow The Player
    }



    private void StateInvestigate()
    {
        _navMeshAgent.speed = _speed;

        _characterAnimator.SetBool("HasEnemy", true);

        float _bestDistance = _maxAttackDistance - _minAttackDistance;

        Vector3 _firePosition = (CurrentTarget.transform.position - transform.position) * _bestDistance;

        _navMeshAgent.stoppingDistance = _maxAttackDistance - _minAttackDistance;

        _navMeshAgent.SetDestination(CurrentTarget.transform.position); // действие Investigate
    }



    private void StateCombat()
    {
        if (CurrentTarget != null)
        {
            _characterAnimator.SetTrigger("Fire");

            _navMeshAgent.speed = 0;

            transform.LookAt(CurrentTarget.transform);

            Vector3 _fixedAimPosition = CurrentTarget.GetComponent<BaseCharacter>().GetEyesPosition().position;

            _fixedAimPosition.y = _fixedAimPosition.y - 0.5f;

            _currentGun.Aim(_fixedAimPosition);

            _currentGun.Shoot(_fixedAimPosition);
        }
    }



    private void SetAnimations()
    {
        _characterAnimator.SetFloat("Speed", _navMeshAgent.velocity.magnitude);
    }


}
