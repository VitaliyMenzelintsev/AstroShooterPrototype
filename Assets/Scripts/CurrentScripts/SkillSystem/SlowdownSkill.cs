using UnityEngine;
using UnityEngine.AI;

public class SlowdownSkill : BaseActivatedSkill
{
    [SerializeField]
    private float _skillDuration = 3f;
    [SerializeField]
    private float _skillCooldown = 8f;
    private bool _isCooldownOver = true;
    [SerializeField]
    private GameObject _myTarget;
    [SerializeField]
    private Transform _partToRotate;   // назначается в инспекторе
    [SerializeField]
    private Transform _firepoint;       // назначается в инспекторе
    [SerializeField]
    private LineRenderer _lineRenderer; // назначается в инспекторе
    private bool _isActivated = false;

    private void Update()
    {
        if(_isActivated)
        {
            LaserRender();

            RotateLaserOrigin();

            _lineRenderer.enabled = true;
        }
        else
        {
            _lineRenderer.enabled = false;
        }
    }

    public override void Activation(bool _isEButtonSkill, GameObject _target) // проверка завершённости кулдауна
    {
        if (_IsIAmEButtonSkill
            && _isEButtonSkill
            && _isCooldownOver)
        {
            _isActivated = true;

            _isCooldownOver = false;

            _myTarget = _target;

            Operation();

            Invoke("StopOperation", _skillDuration);   // время действия способности

            Invoke("CooldownChanger", _skillCooldown); // кулдаун применения
            
        }
    }

    public override void Operation() // действие
    {
        Debug.Log("Способность замедлила" + _myTarget.name);

        _myTarget.GetComponent<NavMeshAgent>().speed -= 2f;
    }

    private void StopOperation() // прекращение действия
    {
        _isActivated = false;

        Debug.Log("Способность завершила действие");

        _myTarget.GetComponent<NavMeshAgent>().speed += 2f;
    }

    protected void CooldownChanger() // переключатель кулдауна
    {
        _isCooldownOver = true;
    }

    private void LaserRender()  // отрисовка лазера
    {
        _lineRenderer.SetPosition(0, _firepoint.position);
        _lineRenderer.SetPosition(1, new Vector3(_myTarget.transform.position.x, _myTarget.transform.position.y + 1.2f, _myTarget.transform.position.z));
    }

    private void RotateLaserOrigin()
    {
        _partToRotate.LookAt(_myTarget.transform.position);
    }
}

