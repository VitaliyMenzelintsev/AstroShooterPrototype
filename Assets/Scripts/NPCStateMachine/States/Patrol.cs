using UnityEngine;

public class Patrol : NPCBaseStateMachine
{
    GameObject[] WayPoints;
    int _currentWP;

    private void Awake()
    {
        WayPoints = GameObject.FindGameObjectsWithTag("WayPoint"); 
    }

    override public void OnStateEnter(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        base.OnStateEnter(_animator, _stateInfo, _layerIndex);
        _currentWP = 0;
    }

    
    override public void OnStateUpdate(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        if (WayPoints.Length == 0) return;
        if(Vector3.Distance(WayPoints[_currentWP].transform.position,   // может лучше перемещать через НавМешАгент Сет Дестинейшн
            NPC.transform.position) < Accuracy)
        {
            _currentWP++;
            if(_currentWP >= WayPoints.Length)
            {
                _currentWP = 0;
            }
        }

        // rotate towards target
        var _direction = WayPoints[_currentWP].transform.position - NPC.transform.position;
        NPC.transform.rotation = Quaternion.Slerp(NPC.transform.rotation,    // кватернион слёрп плавно поворачивает в сторону нужной точки (исопльзовать для поворота игрока)
            Quaternion.LookRotation(_direction),
            RotationSpeed * Time.deltaTime);   
        NPC.transform.Translate(0, 0, Time.deltaTime * Speed);                // может лучше перемещать через НавМешАгент Сет Дестинейшн
    }

    override public void OnStateExit(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {

    }
}
