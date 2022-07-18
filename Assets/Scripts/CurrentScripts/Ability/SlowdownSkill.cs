using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlowdownSkill : BaseActivatedSkill
{


    public override void Activation(bool _isESkill, GameObject _target) // �������� ����������� ��������� �����������
    {
        if (_isESkill)
        {
            Operation(_target);
        }
    }

    public override void Operation(GameObject _target) // ��������
    {
        _skillManager.SlowdownSkill(_target, _skillOwner);

        
        
        // �������� � ����� ��������� ���������, �� ������� �����, ������������� ������� -> �� �������� �������� 
        // �������� ������� ��������� ������������, ����� �� ������� ������� �� ���������� ��� ����, � ��� �� ����� ������� �������


    }
}
