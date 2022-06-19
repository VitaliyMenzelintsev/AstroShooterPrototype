using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

// Висит на игроке
public class PlayerController : MonoBehaviour
{
    private Vector3 _velocity;
    private Rigidbody _myRigidbody;

    private void Start()
    {
        _myRigidbody = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 _moveVelocity)
    {
        this._velocity = _moveVelocity;
    }

    public void LookAt(Vector3 _lookPoint)       
    {
        Vector3 _heightCorrectedPoint = new Vector3(_lookPoint.x, transform.position.y, _lookPoint.z); 
        transform.LookAt(_heightCorrectedPoint);
    }

    private void FixedUpdate()
    {
        _myRigidbody.MovePosition(_myRigidbody.position + _velocity * Time.deltaTime);
    }
}