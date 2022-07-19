using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _allCharactersList = new List<GameObject>();
    private GameObject[] _allCharactersArray;


    private void Awake()
    {
        _allCharactersList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Character"));
    }

    private void Start()
    {
       _allCharactersArray = _allCharactersList.ToArray();
    }


    public GameObject GetNewTarget(int _myTeamNumber, Transform _myEyesPosition, bool _isFindEnemy)
    {
        float _viewDistance = 15f;

        GameObject _bestTarget = null;

        if (_isFindEnemy) // если ищу врагов
        {
            for (int i = 0; i < _allCharactersArray.Length; i++)
            {
                GameObject _currentCharacter = _allCharactersList[i];

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
            for (int i = 0; i < _allCharactersList.Count; i++)
            {
                GameObject _currentCharacter = _allCharactersList[i];

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


    public GameObject[] GetNearestAllies(int _myTeamNumber, float _distanceToLockate, Transform _viewPoint)
    {
        List<GameObject> _myAlliesList = new List<GameObject>();

        for(int i = 0; i < _allCharactersArray.Length; i++)
        {
            GameObject _currentCharacter = _allCharactersList[i];

            if (!IsItMyEnemy(_currentCharacter, _myTeamNumber)
                && IsTargetAlive(_currentCharacter)
                && IsTargetReachable(_viewPoint, _currentCharacter, _distanceToLockate))
            {
                _myAlliesList.Add(_allCharactersArray[i]);
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


    public bool IsTargetReachable(Transform _myEyesPosition, GameObject _enemyPosition, float _viewDistance)
    {
        if (Vector3.Distance(_myEyesPosition.position, _enemyPosition.transform.position) <= _viewDistance)
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

        //направить луч на текущего врага
        if (Physics.Raycast(_myEyesPosition.position, _directionTowardsEnemy, out _hit, _rayDistance))
        {
            //если рейкаст попал в цель, то мы знаем, что можем его увидеть
            if (_hit.transform == _target.transform)
            {
                _canSeeIt = true;
            }
        }

        return _canSeeIt;
    }
}
