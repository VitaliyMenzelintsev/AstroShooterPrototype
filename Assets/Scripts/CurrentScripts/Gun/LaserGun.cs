using UnityEngine;

public class LaserGun : BaseGun
{
    [Header("Gun Settings")]
    private float _punchDamage;
    [SerializeField]
    private LineRenderer _lineRenderer;
    private float _maxDistance;
    private Vector3 _myTarget;
    public bool _myOwnerIsAlive;

    public override void Start()
    {
        base.Start();

        _lineRenderer.enabled = false;

        _punchDamage = _damage / 2;

        _maxDistance = gameObject.GetComponentInParent<BaseCharacter>().GetMaxAttackDistance();
    }



    public override void LateUpdate()
    {
        base.LateUpdate();

        if (Vector3.Distance(gameObject.transform.position, _myTarget) > (_maxDistance - 0.5f))
        {
            _lineRenderer.enabled = false;
        }

        if (_myTarget == null)
        {
            _lineRenderer.enabled = false;
        }


        _myOwnerIsAlive = GetComponentInParent<Vitals>().IsAlive();

        if (!_myOwnerIsAlive)
        {
            _lineRenderer.enabled = false;
        }


    }

    public override void Shoot(Vector3 _aimPoint)
    {
        if (IsGunReady())
        {
            _myTarget = _aimPoint;

            _bulletsInMagazine--;

            _nextShotTime = Time.time + _msBetweenShots / 1000;

            _shootingParticle.Play();

            Vector3 _direction = GetDirection(); 

            Ray _ray = new Ray(_barrelOrigin.position, _direction);

            RaycastHit _hit;

            _lineRenderer.SetPosition(0, _barrelOrigin.position);

            if (Physics.Raycast(_ray, out _hit, _maxDistance))   
            {
                _lastShootTime = Time.time;

                if (_hit.collider != null
                    && _hit.collider.GetComponentInParent<Vitals>()
                    && _hit.collider.GetComponentInParent<Team>().GetTeamNumber() != _myOwnerTeamNumber)
                {
                    _lineRenderer.enabled = true;

                    _hit.collider.GetComponentInParent<Vitals>().GetHit(_damage);

                    _lineRenderer.SetPosition(1, _hit.point);

                }
                else
                {
                    //_lineRenderer.SetPosition(1, _barrelOrigin.position);
                    _lineRenderer.enabled = false;
                }

            }
            else
            {
                _lineRenderer.enabled = false;
            }
        }
        else 
        {
            _lineRenderer.enabled = false;
        }
    }



    public override void Punch()
    {
        if (Time.time > _nextShotTime)
        {
            _nextShotTime = Time.time + _msBetweenShots / 1000;

            if (_lastShootTime + _shootDelay < Time.time)
            {
                Vector3 _direction = GetDirection(); 

                if (Physics.Raycast(_barrelOrigin.position, _direction, out RaycastHit _hit, float.MaxValue))   
                {
                    _lastShootTime = Time.time;

                    _hit.collider.gameObject.GetComponent<Vitals>().GetHit(_punchDamage);
                }
                else
                {
                    _lastShootTime = Time.time;
                }
            }
        }
    }


    public override void ShootRender(Vector3 _aimPoint) { }
}
