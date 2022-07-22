using UnityEngine;

public class ShieldSkill : BaseActivatedSkill
{
    [SerializeField]
    private float _skillDuration = 8f;
    [SerializeField]
    private float _skillCooldown = 20f;
    private bool _isCooldownOver = true;
    [SerializeField]
    private GameObject _shieldPrefab;
    private Vector3 _spawnPosition;
    public int _myOwnerTeamNumber;


    private void Start()
    {
        _myOwnerTeamNumber = GetComponent<Team>().GetTeamNumber();
    }


    public override void Activation(bool _isEButtonSkill, GameObject _target) // �������� ������������� ��������
    {
        if (_isEButtonSkill
            && _isCooldownOver)
        {
            _isCooldownOver = false;

            Operation();

            Invoke("CooldownChanger", _skillCooldown); // ������� ����������
        }
    }



    public override void Operation() // ��������
    {
        Debug.Log("��� ����������");

        _spawnPosition = GameObject.FindObjectOfType<PlayerController>().GetViewPoint();

        GameObject _myShield = Instantiate(_shieldPrefab, _spawnPosition, Quaternion.identity);

        _myShield.GetComponent<Shield>().EndTimer(_skillDuration);
        _myShield.GetComponent<Shield>()._myOwnerTeamNumber = _myOwnerTeamNumber;
    }



    protected void CooldownChanger() // ������������� ��������
    {
        _isCooldownOver = true;
    }
}
