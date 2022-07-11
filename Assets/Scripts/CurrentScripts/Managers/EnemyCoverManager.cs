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

    public EnemyCoverSpot GetCover(EnemyRangeBehavior _character)
    {
        EnemyCoverSpot _bestCover = null;

        Vector3 _characterPosition = _character.transform.position;

        EnemyCoverSpot[] _possibleCoverSpots = _unOccupiedCoverSpots.ToArray();

        for (int i = 0; i < _possibleCoverSpots.Length; i++)
        {
            EnemyCoverSpot _spot = _possibleCoverSpots[i];

            if (!_spot.IsOccupied() // если спот свободен
                && Vector3.Distance(_characterPosition, _spot.transform.position) <= 5f) // если дистанция до спота менее 5 метров
            {
                if (_bestCover == null)
                {
                    _bestCover = _spot;
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
}