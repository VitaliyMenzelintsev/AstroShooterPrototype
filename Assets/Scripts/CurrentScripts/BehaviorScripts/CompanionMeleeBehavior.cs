using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]
[RequireComponent(typeof(Animator))]

public class CompanionMeleeBehavior : CompanionBaseBehavior
{
    [SerializeField]
    private Transform _followPoint;
    [SerializeField]
    private Transform _player;
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
        else
        {
            StateDeath();
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
        _characterAnimator.SetBool("Move", false);

        _characterAnimator.SetBool("Dead", true);
    }



    private void StateIdle()
    {
        _characterAnimator.SetBool("HasEnemy", false);

        _characterAnimator.SetBool("Move", false);

        _characterAnimator.SetBool("HasEnemy", false);
    }



    private void StateFollowThePlayer()
    {
        _characterAnimator.SetBool("Move", true);

        _characterAnimator.SetBool("HasEnemy", false);

        transform.LookAt(_player);

        _navMeshAgent.SetDestination(_followPoint.position);            // действие Follow The Player
    }



    private void StateInvestigate()
    {
        _characterAnimator.SetBool("HasEnemy", true);

        _characterAnimator.SetBool("Move", true);

        _navMeshAgent.SetDestination(CurrentTarget.transform.position); // действие Investigate
    }


     
    private void StateCombat()
    {
        _characterAnimator.SetBool("Move", false);

        _characterAnimator.SetTrigger("Fire");

        transform.LookAt(CurrentTarget.transform);

        _currentGun.Shoot(CurrentTarget.GetComponent<AIBaseBehavior>().Eyes.position);
    }



    private bool IsPlayerFar()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) > 3f)
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
        throw new System.NotImplementedException();
    }
}
