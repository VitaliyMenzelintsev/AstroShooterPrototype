using UnityEngine;

public class Bullet : MonoBehaviour
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

        if(Physics.SphereCast(transform.position, 0.1f, Vector3.forward, out _hit, 0.1f))
        {
            if (_hit.collider.gameObject.GetComponent<Vitals>())
            {
                _hit.collider.gameObject.GetComponent<Vitals>().GetHit(_damage);

                if (_hit.collider.gameObject.GetComponent<BaseAIBehavior>())
                {
                    _speedManager.TemporarilyAIChange(_hit.collider.gameObject, true, _speedDecrease, _slowdownDuration);
                }
                else if (_hit.collider.gameObject.GetComponent<PlayerController>())
                {
                    _speedManager.TemporarilyPlayerChange(_hit.collider.gameObject, true, _speedDecrease, _slowdownDuration);
                }
            }
            Destroy(this.gameObject, 0.1f);
        }
    }
}
