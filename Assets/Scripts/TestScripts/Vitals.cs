using UnityEngine;

public class Vitals : MonoBehaviour
{
    [SerializeField]
    private float _health = 100;
    private float _currentHealth = 100;

    // Start is called before the first frame update
    private void Start()
    {
        _currentHealth = _health;
    }

    public float GetCurHealth()
    {
        return _currentHealth;
    }

    public void GetHit(float _damage)
    {
        _currentHealth -= _damage;
    }
}