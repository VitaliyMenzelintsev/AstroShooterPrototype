using UnityEngine;

public class RoomActivator : MonoBehaviour
{
    public GameObject[] Enemies;
    public GameObject Player;


    private void OnTriggerEnter(Collider other)
    {

        for (int i = 0; i < Enemies.Length; i++)
            Enemies[i].SetActive(true);

        this.gameObject.SetActive(false);

        Destroy(gameObject, 3f);
    }
}
