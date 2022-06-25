using UnityEngine;

public class Patrol : NPCBaseFSM
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
        Speed = 3.0f;
        _currentWP = 0;
    }

    
    override public void OnStateUpdate(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        if (WayPoints.Length == 0) return;
        if(Vector3.Distance(WayPoints[_currentWP].transform.position, 
            NPC.transform.position) < Accuracy)
        {
            _currentWP++;

            if(_currentWP >= WayPoints.Length)
            {
                _currentWP = 0;
            }
        }

        var _direction = WayPoints[_currentWP].transform.position - NPC.transform.position;

        NPC.transform.rotation = Quaternion.Slerp(NPC.transform.rotation, Quaternion.LookRotation(_direction), RotationSpeed * Time.deltaTime);

        NPC.transform.Translate(0, 0, Time.deltaTime * Speed); 
    }

    override public void OnStateExit(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {

    }
}
