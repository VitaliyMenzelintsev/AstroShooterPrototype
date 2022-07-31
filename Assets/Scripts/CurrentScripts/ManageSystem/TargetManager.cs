using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    private GameObject[] _allCharactersArray;
   

    private void Awake()
    {
        _allCharactersArray = GameObject.FindGameObjectsWithTag("Character");
    }


    public GameObject GetNewTarget(int _myTeamNumber, Transform _myEyesPosition, bool _isFindEnemy)
    {
        float _viewDistance = 15f;

        GameObject _bestTarget = null;

        if (_isFindEnemy) 
        {
            for (int i = 0; i < _allCharactersArray.Length; i++) 
            {
                GameObject _currentCharacter = _allCharactersArray[i];

                if (_currentCharacter != null
                    && IsItMyEnemy(_currentCharacter, _myTeamNumber)
                    && IsTargetAlive(_currentCharacter)
                    && IsDistanceCorrect(_currentCharacter, _myEyesPosition, _viewDistance))
                {
                    if (CanSeeTarget(_currentCharacter, _myEyesPosition))
                    {
                        if (_bestTarget == null)
                        {
                            _bestTarget = _currentCharacter;
                        }
                        else
                        {
                            if (IsAnotherTargetCloser(_currentCharacter, _myEyesPosition, _bestTarget))
                            {
                                _bestTarget = _currentCharacter;
                            }
                        }
                    }
                }
            }
        }
        else    
        {
            for (int i = 0; i < _allCharactersArray.Length; i++)
            {
                GameObject _currentCharacter = _allCharactersArray[i];

                if (_currentCharacter != null
                    && !IsItMyEnemy(_currentCharacter, _myTeamNumber)
                    && IsTargetAlive(_currentCharacter)
                    && IsDistanceCorrect(_currentCharacter, _myEyesPosition, _viewDistance))
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
        List<GameObject> _myAlliesList = new();

        for (int i = 0; i < _allCharactersArray.Length; i++) 
        {
            GameObject _currentCharacter = _allCharactersArray[i];

            if (!IsItMyEnemy(_currentCharacter, _myTeamNumber)
                && IsTargetAlive(_currentCharacter)
                && IsDistanceCorrect(_currentCharacter, _viewPoint, _distanceToLockate)
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


    public bool IsDistanceCorrect(GameObject _target, Transform _myTransform, float _viewDistance)
    {
        if (Vector3.Distance(_myTransform.position, _target.transform.position) <= _viewDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public bool CanSeeTarget(GameObject _target, Transform _myEyes)
    {
        bool _canSeeIt = false;

        Transform _visibleTarget = _target.GetComponent<IVisible>().GetHeadTransform();

        Vector3 _directionTowardsTarget = _visibleTarget.position - _myEyes.position;

        float _rayDistance = 25f;

        if (Physics.Raycast(_myEyes.position, _directionTowardsTarget, out RaycastHit _hit, _rayDistance))
        {
            IVisible _visible = _hit.collider.GetComponentInParent<IVisible>();

            if (_visible != null
                && _visibleTarget == _visible.GetHeadTransform())
            {
                _canSeeIt = true;
            }
        }
        return _canSeeIt;
    }
}
