using UnityEngine;

public class GrenadeLauncher : MonoBehaviour
{
    [SerializeField]
    private GameObject _grenadePrefab;
    [SerializeField]
    private Transform _spawnPoint;

    public void Launch()
    {
        Instantiate(_grenadePrefab, _spawnPoint.position, _spawnPoint.rotation);
    }
}
