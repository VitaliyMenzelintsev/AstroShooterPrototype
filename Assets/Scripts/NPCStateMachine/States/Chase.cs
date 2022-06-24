using UnityEngine;

public class Chase : NPCBaseStateMachine
{
    override public void OnStateEnter(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        base.OnStateEnter(_animator, _stateInfo, _layerIndex);
    }

    override public void OnStateUpdate(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        var _direction = Opponent.transform.position - NPC.transform.position;
        NPC.transform.rotation = Quaternion.Slerp(NPC.transform.rotation,    // кватернион слёрп плавно поворачивает в сторону нужной точки (исопльзовать для поворота игрока)
            Quaternion.LookRotation(_direction),
            RotationSpeed * Time.deltaTime);      
        NPC.transform.Translate(0, 0, Time.deltaTime * Speed);
    }

    override public void OnStateExit(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {

    }
}
