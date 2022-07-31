using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : EnemyMeleeBehavior
{
    [SerializeField]
    private GrenadeLauncher[] _launchers;
    [SerializeField]
    private GameObject _shield;
    private bool _isHealingPossible = true;
    private bool _isHealingDone = false;
    [SerializeField]
    private float _secondPhaseSpeed = 4.6f;
    private float _standingAnimationTime = 1f;



    private void FixedUpdate()
    {
        if (MyVitals.IsAlive())
        {
            SetAnimationData();

            if ((MyVitals.GetCurrentHealth() <= MyVitals.GetMaxHealth() / 2) 
                && _isHealingPossible)
            {
                if (!_isHealingDone)
                {
                    _isHealingPossible = false;
                    StartHealState();
                }
            }
            else
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
        }
        else
        {
            StateDeath();
        }
    }



    private void StartHealState()
    {
        // включаем щит, запускаем мины, включаем анимацию, регеним хп на +200 
        _speed = 0;

        if (_navMeshAgent.speed != 0)
            _navMeshAgent.speed = 0;

        _characterAnimator.SetTrigger("StartTransition");

        _shield.SetActive(true);

        StartCoroutine(Heal());

        StartCoroutine(RocketStart());
    
        Invoke(nameof(StopHealState), 4f);
    }


    private IEnumerator RocketStart()
    {
        int _numbersOfLaunches = 8;

        for(int i = 0; i < _numbersOfLaunches; i++)
        {
            yield return new WaitForSeconds(0.3f);
            RocketLaunch();
        }

        StopCoroutine(RocketStart());
    }


    private IEnumerator Heal()
    {
        int _healTicks = 8;

        for (int i = 0; i < _healTicks; i++)
        {
            yield return new WaitForSeconds(0.5f);
            MyVitals.GetHeal(50);
        }

        StopCoroutine(Heal());
    }



    private void RocketLaunch() // запускается в переходной фазе и в фазе комбат
    {
        for (int i = 0; i < _launchers.Length; i++)
        {
            _launchers[i].Launch();
        }
    }


    private void StopHealState()
    {
        _characterAnimator.SetBool("TransitionEnded", true);

        _shield.SetActive(false);

        _isHealingDone = true;

        Invoke(nameof(SetSecondPhaseSpeed), _standingAnimationTime);
    }


    private void SetSecondPhaseSpeed()
    {
        _speed = _secondPhaseSpeed;
        _navMeshAgent.speed = _speed;
    }


    private void SetAnimationData()
    {
        _characterAnimator.SetFloat("Speed", _navMeshAgent.velocity.magnitude);
    }


    private void StateDeath()
    {
        if (_navMeshAgent.speed != 0)
            _navMeshAgent.speed = 0;

        _characterAnimator.SetBool("Dead", true);

        for (int i = 0; i < _myColliders.Length; i++)
        {
            Destroy(_myColliders[i]);
        }

        Destroy(gameObject, 10f);
    }


    private void StateIdle()
    {
        _navMeshAgent.speed = _speed;

        _characterAnimator.SetBool("HasEnemy", false);
    }


    private void StateCombat()
    {
        _characterAnimator.SetTrigger("Fire");

        _navMeshAgent.speed = 0;

        transform.LookAt(CurrentTarget.transform);

        _currentGun.Shoot(CurrentTarget.GetComponent<BaseCharacter>().GetHeadTransform().position);
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

        _navMeshAgent.SetDestination(CurrentTarget.transform.position);
    }
}
