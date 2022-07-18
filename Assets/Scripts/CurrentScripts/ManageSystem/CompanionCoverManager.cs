using System.Collections.Generic;
using UnityEngine;

public class CompanionCoverManager : MonoBehaviour
{
    List<CompanionCoverSpot> _unOccupiedCoverSpots = new List<CompanionCoverSpot>();
    List<CompanionCoverSpot> _occupiedCoverSpots = new List<CompanionCoverSpot>();

    private void Awake()
    {
        _unOccupiedCoverSpots = new List<CompanionCoverSpot>(GameObject.FindObjectsOfType<CompanionCoverSpot>());
    }

    private void AddToOccupied(CompanionCoverSpot _spot)
    {
        if (_unOccupiedCoverSpots.Contains(_spot) && !_occupiedCoverSpots.Contains(_spot))
        {
            _unOccupiedCoverSpots.Remove(_spot);
            _occupiedCoverSpots.Add(_spot);
        }
    }
    private void AddToUnoccupied(CompanionCoverSpot _spot)
    {
        if (_occupiedCoverSpots.Contains(_spot) && !_unOccupiedCoverSpots.Contains(_spot))
        {
            _occupiedCoverSpots.Remove(_spot);
            _unOccupiedCoverSpots.Add(_spot);
        }
    }
     

    public CompanionCoverSpot GetCover(CompanionRangeBehavior _character, GameObject _target)
    {
        CompanionCoverSpot _bestCover = null;

        Vector3 _characterPosition = _character.transform.position;

        CompanionCoverSpot[] _possibleCoverSpots = _unOccupiedCoverSpots.ToArray();

        for (int i = 0; i < _possibleCoverSpots.Length; i++)
        {
            CompanionCoverSpot _spot = _possibleCoverSpots[i];

            if (!_spot.IsOccupied() // ���� ���� ��������
                && Vector3.Distance(_characterPosition, _spot.transform.position) <= 10f  // ���� ��������� �� ����� ����� 10 ������
                /*&& CanSeeEnemyFromSpot(_target, _spot)*/)  // ���� �� ����� ����� �����
            {
                if (_bestCover == null)
                {
                    _bestCover = _spot;
                }
                else
                {   
                    // ����� ���������� �����
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


    private bool CanSeeEnemyFromSpot(Team _target, CompanionCoverSpot _spot)
    {
        bool _canSeeIt = false;

        Vector3 _enemyPosition = _target.Eyes.position;

        float _averageEyesPosition = 1.8f;

        Vector3 _possibleSpotPosition = new Vector3(_spot.transform.position.x, _spot.transform.position.y + _averageEyesPosition, _spot.transform.position.z); 

        RaycastHit _hit;

        if (Physics.Raycast(_possibleSpotPosition, _enemyPosition, out _hit, Mathf.Infinity))
        {
            //���� ������� ����� � ����, �� �� �����, ��� ����� ��� �������
            if (_hit.transform == _target.Eyes.transform)  //??
            {
                _canSeeIt = true;
            }
        }
        return _canSeeIt;
    }


    public void ExitCover(ref CompanionCoverSpot _spot)
    {
        if (_spot != null)
        {
            _spot.SetOccupier(null);

            AddToUnoccupied(_spot);
            _spot = null;
        }
    }
}
