using UnityEngine;
using UnityEngine.AI;


public class CompanionMeleeBehavior : CompanionBaseBehavior
{
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

        _navMeshAgent.SetDestination(_followPoint.position);            // �������� Follow The Player
    }



    private void StateInvestigate()
    {
        _characterAnimator.SetBool("HasEnemy", true);

        _characterAnimator.SetBool("Move", true);

        _navMeshAgent.SetDestination(CurrentTarget.transform.position); // �������� Investigate
    }



    private void StateCombat()
    {
        _characterAnimator.SetBool("Move", false);

        _characterAnimator.SetTrigger("Fire");

        transform.LookAt(CurrentTarget.transform);

        _currentGun.Aim(CurrentTarget.transform.position); /* ������� � Shoot � ����� ������*/


        _currentGun.Shoot(CurrentTarget.GetComponent<BaseCharacter>().GetEyesPosition().position);
    }




    //public override void StateSkill(bool _isESkill, GameObject _target)
    //{
    //    if (MyVitals.IsAlive())
    //    {

    //        Debug.Log("sDGdsfg");
    //        MyActivatedSkill.Activation(_isESkill, _target);
    //    }
    //}
}
