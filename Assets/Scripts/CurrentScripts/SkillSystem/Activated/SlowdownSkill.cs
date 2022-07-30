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
    private bool _isActivated = false;
    [SerializeField]
    private Collider[] _targets;
    [SerializeField]
    private int _myOwnerTeamNumber;
    [SerializeField]
    private ParticleSystem  _fxExplosion;
    [SerializeField]
    private float _damage = 70f;

    public LayerMask _layerMask;



    public override void Activation(bool _isEButtonSkill, GameObject _target) // проверка завершённости кулдауна
    {
        if (!_isEButtonSkill
            && _isCooldownOver)
        {
            _isCooldownOver = false;

            _myOwnerTeamNumber = GetComponent<Team>().GetTeamNumber();

            Operation();

            Invoke(nameof(StopOperation), _skillDuration);   // время действия способности

            Invoke(nameof(CooldownChanger), _skillCooldown); // кулдаун применения
            
        }
    }

    public override void Operation() // действие
    {
        Vector3 _viewPoint = FindObjectOfType<PlayerController>().GetViewPoint();

        _targets = Physics.OverlapSphere(_viewPoint, 2.5f, _layerMask);

        Instantiate(_fxExplosion, _viewPoint, Quaternion.identity);

        for(int i = 0; i < _targets.Length; i++)
        {
            if (_targets[i].gameObject.TryGetComponent(out ITeamable _targetableObject)
                && _targetableObject.GetTeamNumber() != _myOwnerTeamNumber)

            _targets[i].GetComponent<BaseCharacter>()._speed -= 2f;      // перестало работать, потому что каждое состояние жёстко задаёт скорость
            _targets[i].GetComponent<Vitals>().GetHit(_damage);
            _targets[i].GetComponent<SkillTargetFX>().FXPlay();
        }
    }

    private void StopOperation() // прекращение действия
    {
        _isActivated = false;

        for (int i = 0; i < _targets.Length; i++)
        {
            if(_targets[i] != null)
            {
                _targets[i].GetComponent<BaseCharacter>()._speed += 2f;
                _targets[i].GetComponent<SkillTargetFX>().StopFX();
            }
        }
    }



    protected void CooldownChanger() // переключатель кулдауна
    {
        _isCooldownOver = true;
    }
}

