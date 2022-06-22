using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverManager : MonoBehaviour
{

    List<CoverSpot> _unOccupiedCoverSpots = new List<CoverSpot>();
    List<CoverSpot> _occupiedCoverSpots = new List<CoverSpot>();

    private void Awake()
    {
        _unOccupiedCoverSpots = new List<CoverSpot>(GameObject.FindObjectsOfType<CoverSpot>());
    }

    private void AddToOccupied(CoverSpot _spot)
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

    private void AddToUnOccupied(CoverSpot _spot)
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

    public CoverSpot GetCoverTowardsTarget(Character _character, Vector3 _targetPosition, float _maxAttackDistance, float _minAttackDistance, CoverSpot _prevCoverSpot)
    {
        CoverSpot _bestCover = null;
        Vector3 _characterPosition = _character.transform.position;

        CoverSpot[] _possibleCoverSpots = _unOccupiedCoverSpots.ToArray();

        for(int i = 0; i < _possibleCoverSpots.Length; i++)
        {
            CoverSpot _spot = _possibleCoverSpots[i];
            if(_spot.IsOccupied() && _spot.AmICoveredFrom(_targetPosition) && Vector3.Distance(_spot.transform.position, _targetPosition) >= _minAttackDistance)
            {
                if(_bestCover == null)
                {
                    _bestCover = _spot;
                }
                else if(_prevCoverSpot != _spot 
                    && Vector3.Distance(_bestCover.transform.position, _characterPosition) > Vector3.Distance(_spot.transform.position, _characterPosition) 
                    && Vector3.Distance(_spot.transform.position, _targetPosition) < Vector3.Distance(_characterPosition, _targetPosition)) 
                {
                    if(Vector3.Distance(_spot.transform.position, _characterPosition) < Vector3.Distance(_targetPosition, _characterPosition))
                    {
                        _bestCover = _spot;
                    }
                }
            } 
        }
        if(_bestCover != null)
        {
            _bestCover.SetOccupier(_character.transform);
            AddToOccupied(_bestCover);
        }
        return _bestCover;
    }

    public void ExitCover(CoverSpot _spot)
    {
        if(_spot != null)
        {
            _spot.SetOccupier(null);

            AddToUnOccupied(_spot);
        }
    }
}
