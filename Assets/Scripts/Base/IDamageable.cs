using UnityEngine;

public interface IDamageable
{
    void TakeHit(float _damage, Vector3 _hitPoint, Vector3 _hitDirection);  

    void TakeDamage(float _damage);
}