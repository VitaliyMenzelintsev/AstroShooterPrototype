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


    public override void Activation(bool _isEButtonSkill, GameObject _target) // проверка завершённости кулдауна
    {
        if (_isEButtonSkill
            && _isCooldownOver)
        {
            _isCooldownOver = false;

            Operation();

            Invoke("CooldownChanger", _skillCooldown); // кулдаун применения
        }
    }



    public override void Operation() // действие
    {
        Debug.Log("Щит установлен");

        _spawnPosition = GameObject.FindObjectOfType<PlayerController>().GetViewPoint();

        GameObject _myShield = Instantiate(_shieldPrefab, _spawnPosition, Quaternion.identity);

        _myShield.GetComponent<Shield>().EndTimer(_skillDuration);
        _myShield.GetComponent<Shield>()._myOwnerTeamNumber = _myOwnerTeamNumber;
    }



    protected void CooldownChanger() // переключатель кулдауна
    {
        _isCooldownOver = true;
    }
}
