using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseActivatedSkill : MonoBehaviour, IActivatedSkill
{
    public GameObject _skillOwner; // назначаетс€ в инспекторе
    // надо подключить —киллћенеджера, чтобы к нему обращатсь€ и передавать команду 
    public SkillManager _skillManager;

    public virtual void Awake()
    {
        _skillManager = GameObject.FindObjectOfType<SkillManager>();
    }

    public abstract void Activation(bool _isESkill, GameObject _target);

    public abstract void Operation(GameObject _target);
}
