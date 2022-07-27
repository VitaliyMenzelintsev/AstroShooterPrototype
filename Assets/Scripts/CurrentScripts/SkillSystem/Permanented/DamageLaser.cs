using UnityEngine;

public class DamageLaser : MonoBehaviour
{
    [SerializeField]
    private Transform[] _laserPoints;
    [SerializeField]
    private Transform[] _partToRotate;
    [SerializeField]
    private LineRenderer[] _laserRender;
    private BaseCharacter _myOwner;
    [SerializeField]
    private GameObject _currentTarget;
    [SerializeField]
    private float _skillDistance = 6f;
    [SerializeField]
    private float _damage = 0.02f; 


    private void Start()
    {
        _myOwner = this.gameObject.GetComponent<BaseCharacter>();
    }


    private void FixedUpdate()
    {
        if (this.gameObject.GetComponent<Vitals>().IsAlive())
        {
            _currentTarget = _myOwner.GetMyTarget();

            if (_currentTarget != null
                && Vector3.Distance(this.gameObject.transform.position, _currentTarget.transform.position) <= _skillDistance)
            {
                LaserActon(true);
                RotateLaserOrigins();
            }
            else
            {
                LaserActon(false);
            }
        }
        else
        {
            for (int i = 0; i < _laserRender.Length; i++)
            {
                _laserRender[i].enabled = false;
            }
        }
    }


    private void LaserActon(bool _isActive)
    {
        if (_isActive)
        {
            for (int i = 0; i < _laserRender.Length; i++)
            {
                _laserRender[i].enabled = true;
                _laserRender[i].SetPosition(0, _laserPoints[i].position);
                _laserRender[i].SetPosition(1, _currentTarget.transform.position);
                _currentTarget.GetComponent<Vitals>().GetHit(_damage);
            }
        }
        else
        {
            for (int i = 0; i < _laserRender.Length; i++)
            {
                _laserRender[i].enabled = false;
            }
        }
    }


    private void RotateLaserOrigins()
    {
        for (int i = 0; i < _partToRotate.Length; i++)
        {
            _partToRotate[i].transform.LookAt(_currentTarget.transform.position);
        }
    }
}
