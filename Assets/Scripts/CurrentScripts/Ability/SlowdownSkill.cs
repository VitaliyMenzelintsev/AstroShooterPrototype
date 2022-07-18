using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlowdownSkill : BaseActivatedSkill
{


    public override void Activation(bool _isESkill, GameObject _target) // проверка возможности запустить способность
    {
        if (_isESkill)
        {
            Operation(_target);
        }
    }

    public override void Operation(GameObject _target) // действие
    {
        _skillManager.SlowdownSkill(_target, _skillOwner);

        
        
        // передать в древо поведения персонажа, на котором висит, поведенческий триггер -> он запустит анимацию 
        // передать команду менеджеру способностей, чтобы он передал команду на замедление уже цели, а так же начал считать кулдаун


    }
}
