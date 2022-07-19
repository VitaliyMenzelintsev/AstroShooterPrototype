using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDecrease : MonoBehaviour
{
    [SerializeField]
    private int _myTeamNumber;
    [SerializeField]
    private GameObject[] _alliesArray;
    private TargetManager _targetManager;
    [SerializeField]
    private Transform _laserPosition;
    [SerializeField]
    private LineRenderer _lineRenderer;
    private float _skillDistance = 15f;
    [SerializeField]
    private float _damageDecrease = 0.3f;
    private bool _isActivated;


    private Transform _myEyesPosition;
    [HideInInspector]
    public Vitals MyVitals;


    private void Start()
    {
        _targetManager = GameObject.FindObjectOfType<TargetManager>();

        _myTeamNumber = GetComponent<Team>().GetTeamNumber();

        _alliesArray = _targetManager.GetNearestAllies(_myTeamNumber, _skillDistance, _laserPosition);

        MyVitals = GetComponent<Vitals>();

        _isActivated = true;
    }

    private void FixedUpdate()
    {
        if (MyVitals.IsAlive()
            && _alliesArray != null)
        {
            _lineRenderer.enabled = true;

            for (int i = 0; i < _alliesArray.Length; i++)
            {
                GameObject _currentCharacter = _alliesArray[i].gameObject;

                if (_currentCharacter != null
                    && _targetManager.IsTargetAlive(_currentCharacter)
                    && _targetManager.IsTargetReachable(_laserPosition, _currentCharacter, _skillDistance)
                    && _targetManager.CanSeeTarget(_currentCharacter, _laserPosition))
                {
                    Transform _myLaserTarget = _currentCharacter.GetComponent<EnemyBaseBehavior>().GetAntenna();

                    if (_myLaserTarget != null) // таким незамысловатым образом происходит проверка иерархиии противников, могут ли они принимать луч усиления
                    {
                        Operation(_currentCharacter);

                        LaserRender(_myLaserTarget);
                    }
                }
                else
                {

                }
            }
        }
        else
        {


            _lineRenderer.enabled = false;
        }
    }

    public void Operation(GameObject _currentCharacter) 
    {
        _currentCharacter.GetComponent<Vitals>()._damageMultiplier -= _damageDecrease;
    }

    private void StopOperation() 
    {

        _isActivated = false;
    }


    private void LaserRender(Transform _myTarget)  // отрисовка лазера
    {
        _lineRenderer.SetPosition(0, _laserPosition.position);
        _lineRenderer.SetPosition(1, _myTarget.transform.position);
    }
}
