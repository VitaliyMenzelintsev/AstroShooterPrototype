using UnityEngine;

public abstract class BaseActivatedSkill : MonoBehaviour
{
    [SerializeField]
    protected bool _IsIAmEButtonSkill;   // ���� true, �� ���� ������������ �� E, ���� false, �� �� Q

    public abstract void Activation(bool _isEButtonSkill, GameObject _target);

    public abstract void Operation();
}
