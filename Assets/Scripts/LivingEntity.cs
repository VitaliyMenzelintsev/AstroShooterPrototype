using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    protected float _startingHealth;
    protected float _health;
    protected bool _dead;

    public event System.Action OnDeath;

    protected virtual void Start()
    {
        _health = _startingHealth;
    }
    public virtual void TakeHit(float _damage, Vector3 _hitPoint, Vector3 _hitDirection)
    {
        TakeDamage(_damage);
    }

    public virtual void TakeDamage(float _damage)
    {
        _health -= _damage;
        if (_health <= 0 && !_dead)
        {
            Die();
        }
    }

    protected void Die()
    {
        _dead = true;
        if (OnDeath != null)
        {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }
}