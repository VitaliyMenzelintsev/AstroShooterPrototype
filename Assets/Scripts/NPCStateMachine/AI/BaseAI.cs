using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public abstract class BaseAI : MonoBehaviour  // ��������� ��� NPC
{
    [HideInInspector]
    public Animator _characterAnimator;
    [HideInInspector]
    public List<GameObject> GoodEntity;    // ������ ������
    [HideInInspector]
    public List<GameObject> BadEntity;     // ������ ����
    [HideInInspector]
    public GameObject NearestGoodEntity;   // ��������� ������
    [HideInInspector]
    public GameObject NearestBadEntity;    // ��������� ����
    [HideInInspector]
    public GameObject Target;              // ���� ����� (��� ����������� ����� ������������� ���� ������, ����� - ��������� � ��� ����

    public float StartingHealth;
    public float Health;
    public float Speed = 3.0f;
    public float RotationSpeed = 2.0f;
    public float Accuracy = 1.0f;
    public float Damage;
    public bool Dead;

    public event System.Action OnDeath;
    



    public virtual GameObject FindNearestGoodEntity()
    {
        GoodEntity = new List<GameObject>(GameObject.FindGameObjectsWithTag("GoodEntity"));
       

        GameObject[] _goodEntities = GoodEntity.ToArray();

        NearestGoodEntity = _goodEntities[0];

        for (int i = 1; i < _goodEntities.Length; i++)
        {
            var _distance = Vector3.Distance(NearestGoodEntity.transform.position, transform.position);

            var _anotherDistance = Vector3.Distance(_goodEntities[i].transform.position, transform.position);

            if (_anotherDistance < _distance)
            {
                NearestGoodEntity = _goodEntities[i];
            }
            else
            {
                NearestGoodEntity = _goodEntities[0];
            }
        }

        return NearestGoodEntity;
    }

    public virtual GameObject FindNearestBadEntity()
    {
        BadEntity = new List<GameObject>(GameObject.FindGameObjectsWithTag("BadEntity"));

        GameObject[] _badEntities = GoodEntity.ToArray();
        GameObject _nearestBadEntity = _badEntities[0];

        for (int i = 1; i < _badEntities.Length; i++)
        {
            var _distance = Vector3.Distance(_nearestBadEntity.transform.position, transform.position);

            var _anotherDistance = Vector3.Distance(_badEntities[i].transform.position, transform.position);

            if (_anotherDistance < _distance)
            {
                _nearestBadEntity = _badEntities[i];
            }
            else
            {
                _nearestBadEntity = _badEntities[0];
            }
        }

        return _nearestBadEntity;
    }


    public virtual void Start()
    {
        Health = StartingHealth;
        _characterAnimator = GetComponent<Animator>();
    }

    //public virtual void TakeHit(float _damage)
    //{
    //    _damage = _damage * _armorLevel;     // public float _armorLevel = 1.0f(����� �����) 0.9f(������� �����) 0.8f (������ �����)
    //    TakeDamage(_damage);
    //}

    public virtual void TakeDamage(float _damage) // ��� ����������� �������: ������������� ���� ��� �������
    {
        Health -= _damage;

        if (Health <= 0 && !Dead)
        {
            Die();
        }
    }

    protected void Die()
    {
        Dead = true;

        if (OnDeath != null)
        {
            OnDeath();
        }

        // ������ �������� ������

        Destroy(GetComponent<CapsuleCollider>());

        _characterAnimator.SetBool("dead", true);

        GameObject.Destroy(gameObject, 5f);
    }
}