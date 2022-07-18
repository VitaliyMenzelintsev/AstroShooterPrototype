using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseActivatedSkill : MonoBehaviour, IActivatedSkill
{
    public GameObject _skillOwner; // ����������� � ����������
    // ���� ���������� ��������������, ����� � ���� ���������� � ���������� ������� 
    public SkillManager _skillManager;

    public virtual void Awake()
    {
        _skillManager = GameObject.FindObjectOfType<SkillManager>();
    }

    public abstract void Activation(bool _isESkill, GameObject _target);

    public abstract void Operation(GameObject _target);
}
