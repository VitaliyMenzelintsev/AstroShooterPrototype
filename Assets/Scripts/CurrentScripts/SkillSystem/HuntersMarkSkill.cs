using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntersMarkSkill : BaseActivatedSkill
{
    [SerializeField]
    private float _skillDuration = 5f;
    [SerializeField]
    private float _skillCooldown = 8f;
    private bool _isCooldownOver = true;
    [SerializeField]
    private GameObject _myTarget;
    [SerializeField]
    private float _damageIncrease = 0.4f;
    [SerializeField]
    private Transform _partToRotate;   // ����������� � ����������
    [SerializeField]
    private Transform _firepoint;       // ����������� � ����������
    [SerializeField]
    private LineRenderer _lineRenderer; // ����������� � ����������
    private bool _isActivated = false;

    private void Update()
    {
        if (_isActivated)
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

    public override void Activation(bool _isEButtonSkill, GameObject _target) // �������� ������������� ��������
    {
        if (!_IsIAmEButtonSkill
            && !_isEButtonSkill
            && _isCooldownOver)
        {
            _isActivated = true;

            _isCooldownOver = false;

            _myTarget = _target;

            Operation();

            Invoke("StopOperation", _skillDuration);   // ����� �������� �����������

            Invoke("CooldownChanger", _skillCooldown); // ������� ����������

        }
    }

    public override void Operation() // ��������
    {
        _myTarget.GetComponent<Vitals>()._damageMultiplier += _damageIncrease;
        _myTarget.GetComponentInChildren<ParticleSystem>().Play();
    }

    private void StopOperation() // ����������� ��������
    {
        _myTarget.GetComponent<Vitals>()._damageMultiplier -= _damageIncrease;
        _myTarget.GetComponentInChildren<ParticleSystem>().Stop();
        _isActivated = false;

    }

    protected void CooldownChanger() // ������������� ��������
    {
        _isCooldownOver = true;
    }

    private void LaserRender()  // ��������� ������
    {
        _lineRenderer.SetPosition(0, _firepoint.position);
        _lineRenderer.SetPosition(1, new Vector3(_myTarget.transform.position.x, _myTarget.transform.position.y + 1.2f, _myTarget.transform.position.z));
    }

    private void RotateLaserOrigin()
    {
        _partToRotate.LookAt(_myTarget.transform.position);
    }
}
