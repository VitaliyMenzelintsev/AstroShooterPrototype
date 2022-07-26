using System.Collections.Generic;
using UnityEngine;

public class DamageDecreaseLaser : MonoBehaviour
{
    public int _myTeamNumber;
    public GameObject[] _alliesArray;
    private TargetManager _targetManager;
    private Transform _laserPosition;
    private float _skillDistance = 12f;
    [SerializeField]
    private float _damageDecrease = 0.3f;

    [HideInInspector]
    public Vitals MyVitals;


    List<GameObject> _activatedAllies = new List<GameObject>();
    List<GameObject> _disactivatedAllies = new List<GameObject>();


    public LineRenderer[] _laserRender = new LineRenderer[10];


    private void Start()
    {
        _targetManager = GameObject.FindObjectOfType<TargetManager>();

        _myTeamNumber = GetComponent<Team>().GetTeamNumber();

        _laserPosition = this.gameObject.GetComponent<EnemyBaseBehavior>().GetBuffPoint();

        _alliesArray = _targetManager.GetNearestAllies(_myTeamNumber, _skillDistance, _laserPosition, this.gameObject);

        MyVitals = GetComponent<Vitals>();

        StartSkill();
    }

    private void FixedUpdate()  // на старте раскидать по активейтд и дизактивейтед, а в апдейте пробегаться по обоим спискам и менять отрисовку вкл-выкл
    {
        if (MyVitals.IsAlive()
            && _alliesArray != null)
        {
            for (int i = 0; i < _alliesArray.Length; i++)
            {
                GameObject _currentCharacter = _alliesArray[i].gameObject;

                if (_currentCharacter != null
                   && _targetManager.IsTargetAlive(_currentCharacter)
                   && _targetManager.IsTargetReachable(_laserPosition, _currentCharacter, _skillDistance)
                   && _targetManager.CanSeeTarget(_currentCharacter, _laserPosition))
                {
                    GameObject _buffPoint = _currentCharacter.GetComponent<EnemyBaseBehavior>().GetBuffPoint().gameObject;

                    if (_buffPoint != null) // таким незамысловатым образом происходит проверка иерархиии противников, могут ли они принимать луч усиления
                    {
                        AddToActivated(_currentCharacter);
                        //_laserRender[i].enabled = true;
                        _laserRender[i].SetPosition(0, _laserPosition.position);
                        _laserRender[i].SetPosition(1, _buffPoint.transform.position);
                    }
                    else
                    {
                        AddToDisactivated(_currentCharacter);
                    }
                }
                else
                {
                    AddToDisactivated(_currentCharacter);
                }

            }
        }
        else
        {
            EndSkill();
        }
    }


    private void StartSkill()
    {
        for (int i = 0; i < _alliesArray.Length; i++)
        {
            GameObject _currentCharacter = _alliesArray[i].gameObject;

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
