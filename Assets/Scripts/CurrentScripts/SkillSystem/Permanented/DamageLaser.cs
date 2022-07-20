using UnityEngine;

public class DamageLaser : MonoBehaviour
{
    [SerializeField]
    private Transform[] _laserPoints;
    [SerializeField]
    private LineRenderer[] _laserRender;
    private BaseCharacter _myOwner;
    private GameObject _currentTarget;
    private float _skillDistance = 10f;
    [SerializeField]
    private float _damage = 0.2f; // 0.2f * 50 * 4 = 40 dps с 4х лазеров


    private void Start()
    {
        _myOwner = this.gameObject.GetComponent<BaseCharacter>();
        _currentTarget = _myOwner.GetMyTarget();
    }


    private void FixedUpdate()
    {
        if(_currentTarget != null
            && Vector3.Distance(transform.position, _currentTarget.transform.position) <= _skillDistance
            && _currentTarget.GetComponent<Vitals>().IsAlive())
        {
            LaserActon();
        }
    }

    private void LaserActon()
    {
        for(int i = 0; i < _laserRender.Length; i++)
        {
            _laserRender[i].enabled = true;
            _laserRender[i].SetPosition(0, _laserPoints[i].position);
            _laserRender[i].SetPosition(1, _currentTarget.GetComponent<BaseCharacter>().GetEyesPosition().position);
            _currentTarget.GetComponent<Vitals>().GetHit(_damage);
        }
    }
}
