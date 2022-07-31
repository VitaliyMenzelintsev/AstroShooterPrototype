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
    private Transform _partToRotate;   
    [SerializeField]
    private Transform _firepoint;     
    private bool _isActivated = false;
    [SerializeField]
    private Collider[] _targets;
    [SerializeField]
    private int _myOwnerTeamNumber;
    [SerializeField]
    private ParticleSystem  _fxExplosion;
    [SerializeField]
    private float _damage = 70f;

    [SerializeField]
    private LayerMask _layerMask;



    public override void Activation(bool _isEButtonSkill, GameObject _target) 
    {
        if (!_isEButtonSkill
            && _isCooldownOver)
        {
            _isCooldownOver = false;

            _myOwnerTeamNumber = GetComponent<Team>().GetTeamNumber();

            Operation();

            Invoke(nameof(StopOperation), _skillDuration);   

            Invoke(nameof(CooldownChanger), _skillCooldown);
        }
    }

    public override void Operation() 
    {
        Vector3 _viewPoint = FindObjectOfType<PlayerController>().GetViewPoint();

        _targets = Physics.OverlapSphere(_viewPoint, 2.5f, _layerMask);

        Instantiate(_fxExplosion, _viewPoint, Quaternion.identity);

        for(int i = 0; i < _targets.Length; i++)
        {
            if (_targets[i].gameObject.TryGetComponent(out ITeamable _targetableObject)
                && _targetableObject.GetTeamNumber() != _myOwnerTeamNumber)

            _targets[i].GetComponent<BaseCharacter>()._speed -= 2f;    
            _targets[i].GetComponent<Vitals>().GetHit(_damage);
            _targets[i].GetComponent<SkillTargetFX>().FXPlay();
        }
    }

    private void StopOperation() 
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



    protected void CooldownChanger() 
    {
        _isCooldownOver = true;
    }

    public float GetCooldown()
    {
        return _skillCooldown;
    }
}

