using UnityEngine;

public class RoomActivator : MonoBehaviour
{
    public EnemyBaseBehavior[] Enemies;

    private void Start()
    {
        for (int i = 0; i < Enemies.Length; i++)
            Enemies[i].enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < Enemies.Length; i++)
                Enemies[i].enabled = true;
        }

        this.gameObject.SetActive(false);

        Destroy(gameObject, 3f);
    }
}
