using UnityEngine;

public class RoomActivator : MonoBehaviour
{
    public EnemyBaseBehavior[] Enemies;

    private void Start()
    {
        for (int i = 0; i < Enemies.Length; i++)
            Enemies[i].GetComponent<BaseAIBehavior>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            for (int i = 0; i < Enemies.Length; i++)
                Enemies[i].GetComponent<BaseAIBehavior>().enabled = true;
        }

        this.gameObject.SetActive(false);

        Destroy(gameObject, 3f);
    }
}
