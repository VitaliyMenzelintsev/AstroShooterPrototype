using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _allCharacters = new List<GameObject>();
    // должен составить список из всех персонажей и выдавать цель персонажу при выполнении условий

    private void Awake()
    {
        _allCharacters = new List<GameObject>(GameObject.FindGameObjectsWithTag("Character"));
    }

    public GameObject GetNewTarget(int _myTeamNumber, Transform _myEyesPosition, float _myMaxAttackDistance)
    {
        GameObject _bestTarget = null;

        for (int i = 0; i < _allCharacters.Count; i++)
        {
           

            GameObject _currentCharacter = _allCharacters[i];

            //выбирать текущего персонажа в качестве цели, только если мы не в одной команде и если у него осталось здоровье
            if (_currentCharacter != null &&
                _currentCharacter.GetComponent<Team>().GetTeamNumber() != _myTeamNumber
                && _currentCharacter.GetComponent<Vitals>().IsAlive()
                && Vector3.Distance(_myEyesPosition.position, _currentCharacter.transform.position) <= _myMaxAttackDistance)
            {
                Debug.Log(" Таргет менеджер пытается найти цель");
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
                        if (Vector3.Distance(_currentCharacter.transform.position, _myEyesPosition.position) < Vector3.Distance(_bestTarget.transform.position, _myEyesPosition.position))
                        {
                            _bestTarget = _currentCharacter;
                        }
                    }
                }
            }
        }

        return _bestTarget;
    }


    private bool CanSeeTarget(GameObject _target, Transform _myEyesPosition)
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
