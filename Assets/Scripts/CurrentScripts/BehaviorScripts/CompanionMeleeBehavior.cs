using UnityEngine;
using UnityEngine.AI;


public class CompanionMeleeBehavior : CompanionBaseBehavior
{
    [SerializeField]
    private Transform _lookPoint;


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


        if (_isDead)
            _isDead = true;


        for (int i = 0; i < _myColliders.Length; i++)
        {
            _myColliders[i].enabled = false;
        }
    }



    public override void StateIdle()
    {
        _navMeshAgent.speed = _speed;

        _characterAnimator.SetBool("HasEnemy", false);
    }



    private void StateFollowThePlayer()
    {
        _navMeshAgent.speed = _speed;

        _characterAnimator.SetBool("HasEnemy", false);

        transform.LookAt(_player);

        _navMeshAgent.stoppingDistance = 0.2f;

        _navMeshAgent.SetDestination(_followPoint.position);
    }



    private void StateInvestigate()
    {
        _navMeshAgent.stoppingDistance = (_maxAttackDistance - _minAttackDistance) / 2;

        if (Vector3.Distance(_navMeshAgent.transform.position, CurrentTarget.transform.position) <= (_maxAttackDistance - _minAttackDistance) / 2)
        {
            _navMeshAgent.speed = 0;
        }
        else
        {
            _navMeshAgent.speed = _speed;
        }


        _characterAnimator.SetBool("HasEnemy", true);




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
}
