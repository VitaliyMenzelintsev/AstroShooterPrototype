using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{

    //public List<GameObject> _allCharactersList = new List<GameObject>();

    public GameObject[] _allCharactersArray;


    private void Awake()
    {
        //_allCharactersList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Character"));
        _allCharactersArray = GameObject.FindGameObjectsWithTag("Character");
    }


    private void Start()
    {
       
    }


    public GameObject GetNewTarget(int _myTeamNumber, Transform _myEyesPosition, bool _isFindEnemy)
    {
        float _viewDistance = 15f;

        GameObject _bestTarget = null;

        if (_isFindEnemy) // если ищу врагов
        {
            for (int i = 0; i < _allCharactersArray.Length; i++)   // 111
            {
                GameObject _currentCharacter = _allCharactersArray[i];

                //выбирать текущего персонажа в качестве цели, только если мы не в одной команде и если у него осталось здоровье
                if (_currentCharacter != null
                    && IsItMyEnemy(_currentCharacter, _myTeamNumber)
                    && IsTargetAlive(_currentCharacter)
                    && IsTargetReachable(_myEyesPosition, _currentCharacter, _viewDistance))
                {

                    //если цель видно
                    if (CanSeeTarget(_currentCharacter, _myEyesPosition))
                    {
                        if (_bestTarget == null)
                        {
                            _bestTarget = _currentCharacter;
                        }
                        else
                        {
                            //если текущая цель ближе, чем лучшая цель, то выбрать текущую цель 
                            if (IsAnotherTargetCloser(_currentCharacter, _myEyesPosition, _bestTarget))
                            {
                                _bestTarget = _currentCharacter;
                            }
                        }
                    }
                }
            }
        }
        else    // если ищу друзей
        {
            for (int i = 0; i < _allCharactersArray.Length; i++)
            {
                GameObject _currentCharacter = _allCharactersArray[i];

                if (_currentCharacter != null
                    && !IsItMyEnemy(_currentCharacter, _myTeamNumber)
                    && IsTargetAlive(_currentCharacter)
                    && IsTargetReachable(_myEyesPosition, _currentCharacter, _viewDistance))
                {
                    if (CanSeeTarget(_currentCharacter, _myEyesPosition))
                    {
                        _bestTarget = _currentCharacter;
                    }
                }
            }
        }

        return _bestTarget;
    }


    public GameObject[] GetNearestAllies(int _myTeamNumber, float _distanceToLockate, Transform _viewPoint, GameObject _me)
    {
        List<GameObject> _myAlliesList = new List<GameObject>();

        for (int i = 0; i < _allCharactersArray.Length; i++)  // 111
        {
            GameObject _currentCharacter = _allCharactersArray[i];

            if (!IsItMyEnemy(_currentCharacter, _myTeamNumber)
                && IsTargetAlive(_currentCharacter)
                && IsTargetReachable(_viewPoint, _currentCharacter, _distanceToLockate)
                && _currentCharacter != _me)
            {
                _myAlliesList.Add(_currentCharacter);
            }
        }

        GameObject[] _myAlliesArray = _myAlliesList.ToArray();

        return _myAlliesArray;
    }


    public bool IsAnotherTargetCloser(GameObject _currentCharacter, Transform _myEyesPosition, GameObject _bestTarget)
    {
        if (Vector3.Distance(_currentCharacter.transform.position, _myEyesPosition.position) < Vector3.Distance(_bestTarget.transform.position, _myEyesPosition.position))
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    public bool IsItMyEnemy(GameObject _target, int _myTeamNumber)
    {
        if (_target.GetComponent<Team>().GetTeamNumber() != _myTeamNumber)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    public bool IsTargetAlive(GameObject _target)
    {
        if (_target.GetComponent<Vitals>().IsAlive())
        {
            return true;
        }
        else
        {
            return false;
        }

    }


    public bool IsTargetReachable(Transform _myPosition, GameObject _targetPosition, float _viewDistance)
    {
        if (Vector3.Distance(_myPosition.position, _targetPosition.transform.position) <= _viewDistance)
        {
            return true;
        }
        else
        {
            return false;
        }

    }


    public bool CanSeeTarget(GameObject _target, Transform _myEyesPosition)
    {
        bool _canSeeIt = false;

        Vector3 _enemyPosition = new Vector3(_target.transform.position.x, _target.transform.position.y + 1.2f, _target.transform.position.z);

        Vector3 _directionTowardsEnemy = _enemyPosition - _myEyesPosition.position;

        RaycastHit _hit;

        float _rayDistance = 45f;

        if (Physics.Raycast(_myEyesPosition.position, _directionTowardsEnemy, out _hit, _rayDistance))
        {
            
            if (_hit.transform == _target.transform)
            {
                _canSeeIt = true;
            }
        }

        return _canSeeIt;
    }
}
