using UnityEngine;
using UnityEngine.AI;

public class EnemyMeleeBehavior : EnemyBaseBehavior
{
    private NavMeshAgent _navMeshAgent;
    private Animator _characterAnimator;


    public override void Start()
    {
        base.Start();

        _navMeshAgent = GetComponent<NavMeshAgent>();

        _characterAnimator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (MyVitals.IsAlive())
        {
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

                StateIdle();
            }
        }
        else
        {
            StateDeath();
        }
    }


    public override void StateSkill(bool _isESkill, GameObject _target) { }


    private void StateDeath()
    {
        if(_navMeshAgent != null
           && _navMeshAgent.speed != 0)
        _navMeshAgent.speed = 0;

        _characterAnimator.SetBool("Move", false);

        _characterAnimator.SetBool("Dead", true);

        Destroy(GetComponent<CapsuleCollider>());

        Destroy(gameObject, 10f);
    }


    private void StateIdle()
    {
        _navMeshAgent.speed = _speed;

        _characterAnimator.SetBool("Move", false);

        _characterAnimator.SetBool("HasEnemy", false);
    }


    private void StateCombat()
    {
        _characterAnimator.SetBool("Move", false);

        _characterAnimator.SetTrigger("Fire");

        _navMeshAgent.speed = 0;

        transform.LookAt(CurrentTarget.transform);

        _currentGun.Shoot(CurrentTarget.GetComponent<BaseCharacter>().GetHeadTransform().position);
    }


    private void StateInvestigate()
    {
        _navMeshAgent.speed = _speed;

        _characterAnimator.SetBool("HasEnemy", true);

        _characterAnimator.SetBool("Move", true);

        _navMeshAgent.SetDestination(CurrentTarget.transform.position); // действие Investigate
    }
}
