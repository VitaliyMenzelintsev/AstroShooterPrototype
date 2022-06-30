using UnityEngine;

public class Vitals : MonoBehaviour
{
    [SerializeField]
    private float _health = 100;
    private float _currentHealth = 100;


    private void Start()
    {
        _currentHealth = _health;
    }

    public float GetCurrentHealth()
    {
        return _currentHealth;
    }

    public void GetHit(float _damage)
    {
        _currentHealth -= _damage;
    }
}