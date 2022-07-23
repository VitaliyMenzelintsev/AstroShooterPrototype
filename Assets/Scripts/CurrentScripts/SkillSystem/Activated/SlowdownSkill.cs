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
    private Transform _partToRotate;   // ����������� � ����������
    [SerializeField]
    private Transform _firepoint;       // ����������� � ����������
    [SerializeField]
    private LineRenderer _lineRenderer; // ����������� � ����������
    private bool _isActivated = false;
    private Collider[] _targets;
    [SerializeField]
    private int _myOwnerTeamNumber;

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

    public override void Activation(bool _isEButtonSkill, GameObject _target) // �������� ������������� ��������
    {
        if (!_isEButtonSkill
            && _isCooldownOver)
        {
            _isActivated = true;

            _isCooldownOver = false;

            _myOwnerTeamNumber = GetComponent<Team>().GetTeamNumber();

            //_myTarget = _target;

            Operation();

            Invoke("StopOperation", _skillDuration);   // ����� �������� �����������

            Invoke("CooldownChanger", _skillCooldown); // ������� ����������
            
        }
    }

    public override void Operation() // ��������
    {
        //Debug.Log("����������� ���������" + _myTarget.name);
        //RaycastHit[] _targets;

        Vector3 _viewPoint = GameObject.FindObjectOfType<PlayerController>().GetViewPoint();

        _targets = Physics.OverlapSphere(_viewPoint, 2.5f);

        for(int i = 0; i < _targets.Length; i++)
        {
            if(_targets[i].GetComponent<NavMeshAgent>()
                && _targets[i].gameObject.TryGetComponent(out ITeamable _targetableObject)
                && _targetableObject.GetTeamNumber() != _myOwnerTeamNumber)
            _targets[i].GetComponent<NavMeshAgent>().speed -= 2f;
        }
    }

    private void StopOperation() // ����������� ��������
    {
        _isActivated = false;

        for (int i = 0; i < _targets.Length; i++)
        {
            _targets[i].GetComponent<NavMeshAgent>().speed += 2f;
            _lineRenderer.SetPosition(1, _firepoint.position);
        }

        Debug.Log("����������� ��������� ��������");

        //_myTarget.GetComponent<NavMeshAgent>().speed += 2f;
    }

    protected void CooldownChanger() // ������������� ��������
    {
        _isCooldownOver = true;
    }

    private void LaserRender()  // ��������� ������
    {
        for (int i = 0; i < _targets.Length; i++)
        {
            _lineRenderer.SetPosition(0, _firepoint.position);
            _lineRenderer.SetPosition(1, new Vector3(_targets[i].transform.position.x, _targets[i].transform.position.y + 1.2f, _targets[i].transform.position.z));
        }
    }

    private void RotateLaserOrigin()
    {
        //_partToRotate.LookAt(_myTarget.transform.position);
    }
}

