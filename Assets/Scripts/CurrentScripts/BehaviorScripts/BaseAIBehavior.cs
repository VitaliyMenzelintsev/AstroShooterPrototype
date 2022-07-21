using UnityEngine;

public abstract class BaseAIBehavior : BaseCharacter
{
    public abstract void StateSkill(bool _isESkill, GameObject _target);
}
