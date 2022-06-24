using System.Collections.Generic;
using UnityEngine;

public class EnemyCoverManager : MonoBehaviour
{
    List<EnemyCoverSpot> _unOccupiedCoverSpots = new List<EnemyCoverSpot>();
    List<EnemyCoverSpot> _occupiedCoverSpots = new List<EnemyCoverSpot>();
    List<EnemyRangeBehavior> _allCharacters = new List<EnemyRangeBehavior>();

    private void Awake()
    {
        _unOccupiedCoverSpots = new List<EnemyCoverSpot>(GameObject.FindObjectsOfType<EnemyCoverSpot>());

        _allCharacters = new List<EnemyRangeBehavior>(GameObject.FindObjectsOfType<EnemyRangeBehavior>());
    }

    private void AddToOccupied(EnemyCoverSpot _spot)
    {
        if (_unOccupiedCoverSpots.Contains(_spot))
        {
            _unOccupiedCoverSpots.Remove(_spot);
        }
        if (!_occupiedCoverSpots.Contains(_spot))
        {
            _occupiedCoverSpots.Add(_spot);
        }
    }
    private void AddToUnoccupied(EnemyCoverSpot _spot)
    {
        if (_occupiedCoverSpots.Contains(_spot))
        {
            _occupiedCoverSpots.Remove(_spot);
        }
        if (!_unOccupiedCoverSpots.Contains(_spot))
        {
            _unOccupiedCoverSpots.Add(_spot);
        }
    }

    public EnemyCoverSpot GetCoverTowardsTarget(EnemyRangeBehavior _character, Vector3 _targetPosition, float _maxAttackDistance, float _minAttackDistance, EnemyCoverSpot _prevCoverSpot)
    {
        EnemyCoverSpot _bestCover = null;
        Vector3 _soldierPosition = _character.transform.position;

        EnemyCoverSpot[] _possibleCoverSpots = _unOccupiedCoverSpots.ToArray();

        for (int i = 0; i < _possibleCoverSpots.Length; i++)
        {
            EnemyCoverSpot _spot = _possibleCoverSpots[i];

            if (!_spot.IsOccupied() && _spot.AmICoveredFrom(_targetPosition) 
                && Vector3.Distance(_spot.transform.position, _targetPosition) >= _minAttackDistance 
                && !CoverIsPastEnemyLine(_character, _spot))
            {
                if (_bestCover == null)
                {
                    _bestCover = _spot;
                }
                else if (_prevCoverSpot != _spot 
                    && Vector3.Distance(_bestCover.transform.position, _soldierPosition) > Vector3.Distance(_spot.transform.position, _soldierPosition) 
                    && Vector3.Distance(_spot.transform.position, _targetPosition) < Vector3.Distance(_soldierPosition, _targetPosition))
                {
                    if (Vector3.Distance(_spot.transform.position, _soldierPosition) < Vector3.Distance(_targetPosition, _soldierPosition))
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

    public void ExitCover(EnemyCoverSpot _spot)
    {
        if (_spot != null)
        {
            _spot.SetOccupier(null);

            AddToUnoccupied(_spot);
        }
    }

    private bool CoverIsPastEnemyLine(EnemyRangeBehavior _character, EnemyCoverSpot _spot)
    {
        bool _isPastEnemyLine = false;

        foreach (EnemyRangeBehavior _unit in _allCharacters)
        {
            if (_character.MyTeam.GetTeamNumber() != _unit.MyTeam.GetTeamNumber() && _unit.MyVitals.GetCurrentHealth() > 0)
            {
                if (_spot.AmIBehindTargetPosition(_character.transform.position, _unit.transform.position))
                {
                    _isPastEnemyLine = true;
                    break;
                }
            }
        }
        return _isPastEnemyLine;
    }
}