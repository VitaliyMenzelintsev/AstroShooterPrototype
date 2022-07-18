using System.Collections.Generic;
using UnityEngine;

public class EnemyCoverManager : MonoBehaviour
{
    List<EnemyCoverSpot> _unOccupiedCoverSpots = new List<EnemyCoverSpot>();
    List<EnemyCoverSpot> _occupiedCoverSpots = new List<EnemyCoverSpot>();

    private void Awake()
    {
        _unOccupiedCoverSpots = new List<EnemyCoverSpot>(GameObject.FindObjectsOfType<EnemyCoverSpot>());
    }

    private void AddToOccupied(EnemyCoverSpot _spot)
    {
        if (_unOccupiedCoverSpots.Contains(_spot) && !_occupiedCoverSpots.Contains(_spot))
        {
            _unOccupiedCoverSpots.Remove(_spot);
            _occupiedCoverSpots.Add(_spot);
        }
    }
    private void AddToUnoccupied(EnemyCoverSpot _spot)
    {
        if (_occupiedCoverSpots.Contains(_spot) && !_unOccupiedCoverSpots.Contains(_spot))
        {
            _occupiedCoverSpots.Remove(_spot);
            _unOccupiedCoverSpots.Add(_spot);
        }
    }
    public EnemyCoverSpot GetCover(EnemyRangeBehavior _character, Team _target)
    {
        EnemyCoverSpot _bestCover = null;

        Vector3 _characterPosition = _character.transform.position;

        EnemyCoverSpot[] _possibleCoverSpots = _unOccupiedCoverSpots.ToArray();
        for (int i = 0; i < _possibleCoverSpots.Length; i++)
        {
            EnemyCoverSpot _spot = _possibleCoverSpots[i];

            if (!_spot.IsOccupied() // если спот свободен
                && Vector3.Distance(_characterPosition, _spot.transform.position) <= 10f  // если дистанция до спота менее 10 метров
                && CanSeeEnemyFromSpot(_target, _spot))  // если со спота видно врага
            {
                if (_bestCover == null)
                {
                    _bestCover = _spot;
                }
                else
                {
                    // поиск ближайшего спота
                    if (Vector3.Distance(_spot.transform.position, _characterPosition) < Vector3.Distance(_bestCover.transform.position, _characterPosition))
                    {
                        _bestCover = _spot;
                    }
                }

            }
        }
        if (_bestCover != null)
        {
            _bestCover.SetOccupier(_character.transform);
            AddToOccupied(_bestCover);
        }

        return _bestCover;
    }


    private bool CanSeeEnemyFromSpot(Team _target, EnemyCoverSpot _spot)
    {
        bool _canSeeIt = false;

        Vector3 _enemyPosition = _target.Eyes.position;

        float _averageEyesPosition = 1.8f;

        Vector3 _possibleSpotPosition = new Vector3(_spot.transform.position.x, _spot.transform.position.y + _averageEyesPosition, _spot.transform.position.z);

        RaycastHit _hit;

        if (Physics.Raycast(_possibleSpotPosition, _enemyPosition, out _hit, Mathf.Infinity))
        {
            //если рейкаст попал в цель, то мы знаем, что можем его увидеть
            if (_hit.transform == _target.Eyes.transform)  //??
            {
                _canSeeIt = true;
            }
        }
        return _canSeeIt;
    }


    public void ExitCover(ref EnemyCoverSpot _spot)
    {
        if (_spot != null)
        {
            _spot.SetOccupier(null);

            AddToUnoccupied(_spot);
            _spot = null;
        }
    }
}