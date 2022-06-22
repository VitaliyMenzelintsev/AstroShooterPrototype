using UnityEngine;

public class MeleeRobotAI : MonoBehaviour
{
    private Animator _animator;
    public GameObject Player;

    public GameObject GetPlayer()
    {
        return Player;
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _animator.SetFloat("_distanceToPlayer", Vector3.Distance(transform.position, Player.transform.position));
    }

    private void Hit()
    {
        Debug.Log("Melee Robot hits player");
    }

    public void StartHitting()
    {
        InvokeRepeating("Hit", 1.5f, 1.5f);
    }

    public void StopHitting()
    {
        CancelInvoke("Hit");
    }
}
