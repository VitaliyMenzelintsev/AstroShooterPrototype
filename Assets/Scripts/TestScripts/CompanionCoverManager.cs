using System.Collections.Generic;
using UnityEngine;

public class CompanionCoverManager : MonoBehaviour
{
    List<CompanionCoverSpot> _unOccupiedCoverSpots = new List<CompanionCoverSpot>();
    List<CompanionCoverSpot> _occupiedCoverSpots = new List<CompanionCoverSpot>();
    List<CompanionRangeBehavior> _allCharacters = new List<CompanionRangeBehavior>();

    private void Awake()
    {
        _unOccupiedCoverSpots = new List<CompanionCoverSpot>(GameObject.FindObjectsOfType<CompanionCoverSpot>());

        _allCharacters = new List<CompanionRangeBehavior>(GameObject.FindObjectsOfType<CompanionRangeBehavior>());
    }

    private void AddToOccupied(CompanionCoverSpot _spot)
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
    private void AddToUnoccupied(CompanionCoverSpot _spot)
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

    public CompanionCoverSpot GetCoverTowardsTarget(CompanionRangeBehavior _character, Vector3 _targetPosition, float _maxAttackDistance, float _minAttackDistance, CompanionCoverSpot _prevCoverSpot)
    {
        CompanionCoverSpot _bestCover = null;
        Vector3 _characterPosition = _character.transform.position;

        CompanionCoverSpot[] _possibleCoverSpots = _unOccupiedCoverSpots.ToArray();

        for (int i = 0; i < _possibleCoverSpots.Length; i++)
        {
            CompanionCoverSpot _spot = _possibleCoverSpots[i];

            if (!_spot.IsOccupied() && _spot.AmICoveredFrom(_targetPosition)
                && Vector3.Distance(_spot.transform.position, _targetPosition) >= _minAttackDistance
                && !CoverIsPastEnemyLine(_character, _spot))
            {
                if (_bestCover == null)
                {
                    _bestCover = _spot;
                }
                else if (_prevCoverSpot != _spot
                    && Vector3.Distance(_bestCover.transform.position, _characterPosition) > Vector3.Distance(_spot.transform.position, _characterPosition)
                    && Vector3.Distance(_spot.transform.position, _targetPosition) < Vector3.Distance(_characterPosition, _targetPosition))
                {
                    if (Vector3.Distance(_spot.transform.position, _characterPosition) < Vector3.Distance(_targetPosition, _characterPosition))
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

    public void ExitCover(CompanionCoverSpot _spot)
    {
        if (_spot != null)
        {
            _spot.SetOccupier(null);

            AddToUnoccupied(_spot);
        }
    }

    private bool CoverIsPastEnemyLine(CompanionRangeBehavior _character, CompanionCoverSpot _spot)
    {
        bool _isPastEnemyLine = false;

        foreach (CompanionRangeBehavior _unit in _allCharacters)
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
