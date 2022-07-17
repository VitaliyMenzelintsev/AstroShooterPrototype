using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Team))]
[RequireComponent(typeof(Vitals))]
[RequireComponent(typeof(Animator))]

public class EnemyRangeBehavior : EnemyBaseBehavior
{
    private NavMeshAgent _navMeshAgent;
    private Animator _characterAnimator;
    private EnemyCoverManager _coverManager;
    [SerializeField]
    private EnemyCoverSpot _currentCover = null;


    public AI_States _state = AI_States.idle;


    private void Start()
    {
        MyTeam = GetComponent<Team>();

        MyVitals = GetComponent<Vitals>();

        _allCharacters = GameObject.FindObjectsOfType<Team>();

        _navMeshAgent = GetComponent<NavMeshAgent>();

        _characterAnimator = GetComponent<Animator>();

        _coverManager = GameObject.FindObjectOfType<EnemyCoverManager>();
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
                _currentCover = _coverManager.GetCover(this, _currentTarget);

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
            _currentTarget = GetNewTarget();

            StateIdle();
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

    private void StateDeath()
    {
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
        transform.LookAt(_currentTarget.transform);

        _characterAnimator.SetBool("Move", false);

        _characterAnimator.SetTrigger("Fire");

        _currentGun.Aim(_currentTarget.Eyes.position);

        _currentGun.Shoot(_currentTarget.Eyes.position); //  действие range combat
    }


    private void StateMeleeCombat()
    {
        ExitCover();

        _characterAnimator.SetBool("Move", false);

        _characterAnimator.SetTrigger("Punch");  // действие melee combat

        _currentGun.Punch();
    }


    private bool IsMeleeDistance()
    {
        if (Vector3.Distance(transform.position, _currentTarget.transform.position) < _minAttackDistance)
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
        if (Vector3.Distance(transform.position, _currentTarget.transform.position) <= _maxAttackDistance
                && Vector3.Distance(transform.position, _currentTarget.transform.position) >= _minAttackDistance)
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
