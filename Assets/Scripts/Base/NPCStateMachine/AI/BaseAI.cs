using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI; 

//[RequireComponent(typeof(CapsuleCollider))]
//[RequireComponent(typeof(NavMeshAgent))]
//[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]

public abstract class BaseAI : MonoBehaviour  // наследуют все NPC
{
    [HideInInspector]
    public GameObject[] GoodEntities;    // список добрых
    [HideInInspector]
    public GameObject[] BadEntities;     // список злых
    [HideInInspector]
    public GameObject NearestGoodEntity;   // ближайший добрый
    [HideInInspector]
    public GameObject NearestBadEntity;    // ближайший злой
    [HideInInspector]
    public GameObject Target;              // цель атаки (дл€ компаньонов будет присваиватьс€ цель »грока, иначе - ближайший к ним злой
    public GameObject FollowPoint;
    public GameObject Player;

    protected HashAnimationNames _animationBase = new HashAnimationNames();

    [HideInInspector]
    public Animator CharacterAnimator;

    public float StartingHealth;
    [HideInInspector]
    public float Health;
    public float Speed = 3.0f;
    public float RotationSpeed = 2.0f;
    public float Accuracy = 1.0f;
    public float Damage;
    public bool Dead;

    //public event System.Action OnDeath;


    public virtual void Start()
    {
        Health = StartingHealth;
        CharacterAnimator = GetComponent<Animator>();
    }

    public virtual void Update()
    {

    }

    public GameObject GetPlayer()
    {
        return Player;
    }
    public GameObject GetFollowPoint()
    {
        return FollowPoint;
    }

    public virtual GameObject GetTarget()
    {
        GoodEntities = GameObject.FindGameObjectsWithTag("GoodEntity");
        BadEntities = GameObject.FindGameObjectsWithTag("BadEntity");

        if (gameObject.GetComponent<MeleeEnemyAI>() != null || gameObject.GetComponent<RangeEnemyAI>() != null)
        {
            GameObject _nearestGoodEntity = GoodEntities[0];

            for (int i = 1; i < GoodEntities.Length; i++)
            {
                float _distance = Vector3.Distance(_nearestGoodEntity.transform.position, transform.position);

                float _anotherDistance = Vector3.Distance(GoodEntities[i].transform.position, transform.position);

                if (_anotherDistance < _distance)
                {
                    _nearestGoodEntity = GoodEntities[i];
                }
                else
                {
                    _nearestGoodEntity = GoodEntities[0];
                }
            }

            Target = _nearestGoodEntity;
        }
        else if (gameObject.GetComponent<MeleeCompanionAI>() != null || gameObject.GetComponent<RangeCompanionAI>() != null)
        {
            GameObject _nearestBadEntity = BadEntities[0];

            for (int i = 1; i < BadEntities.Length; i++)
            {
                float _distance = Vector3.Distance(_nearestBadEntity.transform.position, transform.position);

                float _anotherDistance = Vector3.Distance(BadEntities[i].transform.position, transform.position);

                if (_anotherDistance < _distance)
                {
                    _nearestBadEntity = BadEntities[i];
                }
                else
                {
                    _nearestBadEntity = BadEntities[0];
                }
            }

            Target = _nearestBadEntity;
        }
        return Target;
    }

    //private void ChangeState(int _newState)
    //{
    //    if (_currentState == _newState)
    //        return;

    //    CharacterAnimator.Play(_newState);

    //    _currentState = _newState;
    //}


    //public virtual void TakeHit(float _damage)
    //{
    //    _damage = _damage * _armorLevel;     // public float _armorLevel = 1.0f(лЄгка€ брон€) 0.9f(средн€€ брон€) 0.8f (т€жЄла€ брон€)
    //    TakeDamage(_damage);
    //}

    //public virtual void TakeDamage(float _damage)
    //{
    //    Health -= _damage;

    //    if (Health <= 0 && !Dead)
    //    {
    //        Die();
    //    }
    //}

    //protected void Die()
    //{
    //    Dead = true;

    //    if (OnDeath != null)
    //    {
    //        OnDeath();
    //    }

    //    // запуск анимации смерти

    //    Destroy(GetComponent<CapsuleCollider>());

    //    CharacterAnimator.SetBool("dead", true);

    //    GameObject.Destroy(gameObject, 5f);
    //}
}