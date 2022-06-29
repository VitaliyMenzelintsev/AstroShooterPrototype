using UnityEngine;
using System.Collections;

public class NewGunScript : MonoBehaviour
{
    [SerializeField]
    private bool _addBulletSpread = true;
    [SerializeField]
    private Vector3 _bulletSpreadVariance = new Vector3(0.05f, 0.05f, 0.05f);
    [SerializeField]
    private ParticleSystem _shootingParticle;
    [SerializeField]
    private Transform _bulletSpawnPoint;
    [SerializeField]
    private TrailRenderer _bulletTrail;
    [SerializeField]
    private float _shootDelay = 0.05f;  // ???
    //[SerializeField]
    //private LayerMask Mask;
    [SerializeField]
    private float _range = 30f;

    private Animator _animator;
    private float _lastShootTime;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Shoot()
    {
        if(_lastShootTime +_shootDelay < Time.deltaTime)  // если по КД можно стрелять
        {
            // Make shot
            _animator.SetBool("Fire", true);     // trigger? переключаем аниматор в режим стрельбы
            _shootingParticle.Play();            // включаем партикл систем
            Vector3 _direction = GetDirection(); // определяем направление стрельбы

            RaycastHit _hit;

            if(Physics.Raycast(_bulletSpawnPoint.position, _direction, out _hit, _range/*, Mask*/))   // если попали во что-то
            {
                TrailRenderer _trail = Instantiate(_bulletTrail, _bulletSpawnPoint.position, Quaternion.identity);  // делаем след

                StartCoroutine(SpawnTrail(_trail, _hit));

                _lastShootTime = Time.time;
            }
        }
    }

    private Vector3 GetDirection()
    {
        Vector3 _direction = transform.forward;

        if (_addBulletSpread) // если делаем разброс, то он задаётся путём рандомизации координат вектора направления
        {
            _direction += new Vector3(
                Random.Range(-_bulletSpreadVariance.x, _bulletSpreadVariance.x),
                Random.Range(-_bulletSpreadVariance.y, _bulletSpreadVariance.y),
                Random.Range(-_bulletSpreadVariance.z, _bulletSpreadVariance.z));

            _direction.Normalize();
        }
        return _direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer _trail, RaycastHit _hit)
    {
        float time = 0;
        Vector3 _startPosition = _trail.transform.position;

        while(time < 1)
        {
            _trail.transform.position = Vector3.Lerp(_startPosition, _hit.point, time);
            time += Time.deltaTime / _trail.time;

            yield return null;
        }
        _animator.SetBool("Fire", false);
        _trail.transform.position = _hit.point;

        Destroy(_trail.gameObject, _trail.time);
    }
}
