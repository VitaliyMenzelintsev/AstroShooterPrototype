using System.Collections.Generic;
using UnityEngine;

public class DamageDecreaseLaser : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _alliesArray;

    [SerializeField]
    private float _skillDistance = 12f;
    [SerializeField]
    private float _damageDecrease = 0.3f;
    private int _myTeamNumber;

    private TargetManager _targetManager;
    private Transform _laserPosition;
    private Vitals _myVitals;

    private List<GameObject> _activatedAllies = new();
    private List<GameObject> _disactivatedAllies = new();
    [SerializeField]
    private LineRenderer[] _laserRender = new LineRenderer[10];


    private void Start()
    {
        _targetManager = FindObjectOfType<TargetManager>();

        _myTeamNumber = GetComponent<Team>().GetTeamNumber();

        _laserPosition = gameObject.GetComponent<EnemyBaseBehavior>().GetBuffPoint();

        _alliesArray = _targetManager.GetNearestAllies(_myTeamNumber, _skillDistance, _laserPosition, gameObject);

        _myVitals = GetComponent<Vitals>();

        StartSkill();
    }


    private void FixedUpdate() 
    {
        _myVitals.IsAlive();  // ??

        if (_myVitals.IsAlive()
            && _alliesArray != null)
        {
            for (int i = 0; i < _alliesArray.Length; i++)
            {
                GameObject _currentCharacter = _alliesArray[i];

                if (_currentCharacter != null
                   && _targetManager.IsTargetAlive(_currentCharacter)
                   && _targetManager.IsDistanceCorrect(_currentCharacter, _laserPosition, _skillDistance)
                   /*&& _targetManager.CanSeeTarget(_currentCharacter, _laserPosition)*/)
                {
                    Transform _buffPoint = _currentCharacter.GetComponent<EnemyBaseBehavior>().GetBuffPoint();

                    if (_buffPoint != null) 
                    {
                        AddToActivated(_currentCharacter);
                        _laserRender[i].SetPosition(0, _laserPosition.position);
                        _laserRender[i].SetPosition(1, _buffPoint.position);
                    }
                    else
                    {
                        _laserRender[i].enabled = false;
                        AddToDisactivated(_currentCharacter);
                    }
                }
                else
                {
                    _laserRender[i].enabled = false;
                    AddToDisactivated(_currentCharacter);
                }
            }
        }
        else
        {
            EndSkill();
        }
    }


    private void LaserRenderer()
    {
        for (int i = 0; i < _activatedAllies.Count; i++)
        {


            AddToDisactivated(_activatedAllies[i]);
        }
    }


    private void LaserSwitcher()
    {

    }



    private void StartSkill()
    {
        for (int i = 0; i < _alliesArray.Length; i++)
        {
            GameObject _currentCharacter = _alliesArray[i];

            Transform _myLaserTarget = _currentCharacter.GetComponent<EnemyBaseBehavior>().GetBuffPoint();

            if (_myLaserTarget != null)
            {
                _activatedAllies.Add(_currentCharacter);

                _currentCharacter.GetComponent<Vitals>()._damageMultiplier -= _damageDecrease;
            }
        }
    }


    private void EndSkill()
    {
        for (int i = 0; i < _activatedAllies.Count; i++)
        {
            AddToDisactivated(_activatedAllies[i]);
        }

        for (int i = 0; i < _laserRender.Length; i++)
        {
            _laserRender[i].enabled = false;
        }
    }


    private void AddToDisactivated(GameObject _currentCharacter)
    {
        if (_activatedAllies.Contains(_currentCharacter)
            && !_disactivatedAllies.Contains(_currentCharacter))
        {
            _activatedAllies.Remove(_currentCharacter);
            _disactivatedAllies.Add(_currentCharacter);

            _currentCharacter.GetComponent<Vitals>()._damageMultiplier += _damageDecrease;
        }
    }


    private void AddToActivated(GameObject _currentCharacter)
    {
        if (_disactivatedAllies.Contains(_currentCharacter)
            && !_activatedAllies.Contains(_currentCharacter))
        {
            _disactivatedAllies.Remove(_currentCharacter);
            _activatedAllies.Add(_currentCharacter);

            _currentCharacter.GetComponent<Vitals>()._damageMultiplier -= _damageDecrease;
        }
    }
}
