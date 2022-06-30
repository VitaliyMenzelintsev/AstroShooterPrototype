using UnityEngine;

public class GunScript : MonoBehaviour
{
    public float _damageDealt = 50f;
    public float _range = 15f;
    public Transform Eyes;


    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        RaycastHit _hit;
        if(Physics.Raycast(Eyes.position, Eyes.forward, out _hit, _range))
        {
            Debug.Log(_hit.transform.name);

            Vitals _vitals = _hit.transform.GetComponent<Vitals>();

            if(_vitals != null)
            {
                _vitals.GetHit(_damageDealt);
            }
        }
    }
}
