using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _damage = 10f;
    [SerializeField]
    private float _speedDecrease = 1f;
    [SerializeField]
    private float _slowdownDuration = 1f;
    private SpeedManager _speedManager;
    public Vector3 _aimPoint;
    public LayerMask layerMask;

    private void Start()
    {
        _speedManager = FindObjectOfType<SpeedManager>();
        //Destroy(gameObject, 3f);
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * _speed * Time.fixedDeltaTime);
        SphereCast();
    }

    private void SphereCast()
    {
        RaycastHit _hit;

        if (Physics.SphereCast(transform.position, 1f, transform.forward, out _hit, 0.1f, layerMask, QueryTriggerInteraction.UseGlobal))
        {
            if (_hit.transform.gameObject.GetComponentInParent<Vitals>())
            {
                _hit.transform.gameObject.GetComponentInParent<Vitals>().GetHit(_damage);
                Destroy(gameObject, 0.2f);
            }

            Destroy(gameObject, 0.2f);
        }
    }
}
