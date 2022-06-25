using UnityEngine;

// висит на пуле
public class Projectile : MonoBehaviour
{
    public LayerMask CollisionMask;

    private float _speed = 30;
    private float _damage = 1;
    private float _lifeTime = 3;
    private float _skinWidth = 0.1f;

    private void Start()
    {
        Destroy(gameObject, _lifeTime);

        Collider[] _initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, CollisionMask);

        if (_initialCollisions.Length > 0)
            OnHitObject(_initialCollisions[0], transform.position, transform.forward);
    }

    public void SetSpeed(float _newSpeed)
    {
        _speed = _newSpeed;
    }

    private void Update()
    {
        float _moveDistance = _speed * Time.deltaTime;
        CheckCollisions(_moveDistance);
        transform.Translate(Vector3.forward * _moveDistance);
    }

    private void CheckCollisions(float _moveDistance)
    {
        Ray _ray = new Ray(transform.position, transform.forward);
        RaycastHit _hit;

        if (Physics.Raycast(_ray, out _hit, _moveDistance + _skinWidth, CollisionMask, QueryTriggerInteraction.Collide))
            OnHitObject(_hit.collider, _hit.point, transform.forward);
    }

    private void OnHitObject(Collider _collider, Vector3 _hitPoint, Vector3 _hitDirection)
    {
        //IDamageable _damageableObject = _collider.GetComponent<IDamageable>();

        //if (_damageableObject != null)
        //    _damageableObject.TakeHit(_damage, _hitPoint, transform.forward);
        
        GameObject.Destroy(gameObject);
    }
}