using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4f;
    [SerializeField]
    private float _damage = 10f;
    [SerializeField]
    private float _speedDecrease = 1f;
    [SerializeField]
    private float _slowdownDuration = 1f;
    private SpeedManager _speedManager;
    public Vector3 _aimPoint;


    private void Start()
    {
        _speedManager = GameObject.FindObjectOfType<SpeedManager>();
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
        SphereCast();
    }

    private void SphereCast()
    {
        RaycastHit _hit;

        if (Physics.SphereCast(transform.position, 0.2f, Vector3.forward, out _hit, 0.01f))
        {
            if (_hit.collider != null
                      && _hit.collider.TryGetComponent(out IDamageable _damageableObject))
            {
                _damageableObject.GetHit(_damage);
                Destroy(this.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
}
