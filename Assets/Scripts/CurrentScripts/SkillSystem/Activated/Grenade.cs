using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField]
    private float _time;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _angle;
    private float _gravity = 9.81f;


    [SerializeField]
    private float _explosionRadius;
    [SerializeField]
    private LayerMask _collicionLayerMask;
    [SerializeField]
    private LayerMask _damageLayerMask;

    [SerializeField]
    private float _damage;

    [SerializeField]
    private ParticleSystem _trailFX;
    [SerializeField]
    private ParticleSystem _explosionFXPrefab;

    private List<GameObject> _currentHitObjects = new List<GameObject>();



    private void Start()
    {
        transform.localEulerAngles = new Vector3(transform.parent.eulerAngles.x, 0, transform.parent.eulerAngles.z);
        transform.parent.eulerAngles = new Vector3(0, transform.parent.eulerAngles.y, 0);

        _angle = -transform.localEulerAngles.x;
        _angle = _angle * Mathf.Deg2Rad;
    }




    private void FixedUpdate()
    {
        BallisticTranslate();

        CollisionDetect();
    }

    private void BallisticTranslate()
    {
        _time += Time.fixedDeltaTime;
        float _vz = _speed * _time * Mathf.Cos(_angle) * Time.fixedDeltaTime;
        float _vy = _speed * _time * Mathf.Sin(_angle) * Time.fixedDeltaTime - _gravity * _time * _time / 2 * Time.fixedDeltaTime;
        Vector3 _futurePosition = new Vector3(transform.localPosition.x, transform.localPosition.y + _vy, transform.localPosition.z + _vz);
        transform.rotation = Quaternion.LookRotation(transform.localPosition - _futurePosition);
        transform.eulerAngles += new Vector3(0, transform.parent.eulerAngles.y, 0);
        transform.localPosition = _futurePosition;
    }

    private void CollisionDetect()
    {
        if(Physics.Raycast(transform.position, transform.forward, 0.1f, _collicionLayerMask))
        {
            Explosion();
        }
    }

    private void Explosion()
    {
        //_currentHitObjects.Clear();

        //RaycastHit[] _hits;
        //_hits = Physics.SphereCastAll(transform.position, _explosionRadius, transform.forward, 0.1f, _damageLayerMask, QueryTriggerInteraction.UseGlobal);
        //foreach(RaycastHit _hit in _hits)
        //{
        //    _currentHitObjects.Add(_hit.transform.gameObject);
        //}

        _trailFX.Stop();

        Instantiate(_explosionFXPrefab, new Vector3(transform.position.x, transform.position.y + 0.7f, transform.position.z) , Quaternion.identity);
        //_explosionFX.Play();

        RaycastHit[] _hits = new RaycastHit[10];

        int _numberOfHits = Physics.SphereCastNonAlloc(transform.position, _explosionRadius, transform.forward, _hits, 0.1f, _damageLayerMask, QueryTriggerInteraction.UseGlobal);

        for(int i = 0; i < _numberOfHits; i++)
        {
            _currentHitObjects.Add(_hits[i].transform.gameObject);
        }

        DamageDeal();
    }

    private void DamageDeal()
    {
        for (int i = 0; i < _currentHitObjects.Count; i++)
        {
            if (_currentHitObjects[i].GetComponentInParent<Vitals>())
            {
                _currentHitObjects[i].GetComponentInParent<Vitals>().GetHit(_damage);
            }
        }
        Destroy(gameObject);
    }
}
