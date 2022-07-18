using UnityEngine;

public abstract class BaseActivatedSkill : MonoBehaviour
{
    [SerializeField]
    protected bool _IsIAmEButtonSkill;   // ≈сли true, то скил активируетс€ на E, если false, то на Q

    public abstract void Activation(bool _isEButtonSkill, GameObject _target);

    public abstract void Operation();
}
