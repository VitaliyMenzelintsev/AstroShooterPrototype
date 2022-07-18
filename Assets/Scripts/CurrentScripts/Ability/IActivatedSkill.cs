using UnityEngine;

public interface IActivatedSkill
{
    void Awake();

    void Activation(bool _isESkill, GameObject _target);

    void Operation(GameObject _target); 
}
