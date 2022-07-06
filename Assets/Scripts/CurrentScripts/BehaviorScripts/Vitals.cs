using UnityEngine;

public class Vitals : MonoBehaviour
{
    [SerializeField]
    private float _health = 100;
    [SerializeField]
    private float _currentHealth = 100;
    [SerializeField]
    private float _ressurectHealth = 60;

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

    public void GetHeal(float _heal)
    {
        _currentHealth += _heal;
    }

    public void GetRessurect()
    {
        _currentHealth = _ressurectHealth;
    }
}