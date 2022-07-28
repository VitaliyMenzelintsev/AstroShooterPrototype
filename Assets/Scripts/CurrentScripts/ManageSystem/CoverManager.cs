using System.Collections.Generic;
using UnityEngine;

public class CoverManager : MonoBehaviour
{
    private List<EnemyCoverSpot> _freeEnemyCoverSpots = new List<EnemyCoverSpot>();
    private List<EnemyCoverSpot> _lockEnemyCoverSpots = new List<EnemyCoverSpot>();
    private List<CompanionCoverSpot> _freeCompanionCoverSpots = new List<CompanionCoverSpot>();
    private List<CompanionCoverSpot> _lockCompanionCoverSpots = new List<CompanionCoverSpot>();
    private PlayerController _player;

    
    private void Awake()
    {
        _freeEnemyCoverSpots = new List<EnemyCoverSpot>(GameObject.FindObjectsOfType<EnemyCoverSpot>());
        _freeCompanionCoverSpots = new List<CompanionCoverSpot>(GameObject.FindObjectsOfType<CompanionCoverSpot>());
        _player = GameObject.FindObjectOfType<PlayerController>();
    }

    private void AddToOccupied(EnemyCoverSpot _spot)
    {
        if (_freeEnemyCoverSpots.Contains(_spot) && !_lockEnemyCoverSpots.Contains(_spot))
        {
            _freeEnemyCoverSpots.Remove(_spot);
            _lockEnemyCoverSpots.Add(_spot);
        }
    }
    private void AddToUnoccupied(EnemyCoverSpot _spot)
    {
        if (_lockEnemyCoverSpots.Contains(_spot) && !_freeEnemyCoverSpots.Contains(_spot))
        {
            _lockEnemyCoverSpots.Remove(_spot);
            _freeEnemyCoverSpots.Add(_spot);
        }
    }
    public EnemyCoverSpot GetCover(EnemyRangeBehavior _character, GameObject _target)
    {
        EnemyCoverSpot _bestCover = null;

        Vector3 _characterPosition = _character.transform.position;

        EnemyCoverSpot[] _possibleCoverSpots = _freeEnemyCoverSpots.ToArray();
        for (int i = 0; i < _possibleCoverSpots.Length; i++)
        {
            EnemyCoverSpot _spot = _possibleCoverSpots[i];

            if (!_spot.IsOccupied() // если спот свободен
                && Vector3.Distance(_characterPosition, _spot.transform.position) <= 10f  // если дистанция до спота менее 10 метров
               /* && CanSeeEnemyFromSpot(_target, _spot)*/)  // если со спота видно врага
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


    private bool CanSeeEnemyFromSpot(GameObject _target, EnemyCoverSpot _spot)
    {
        bool _canSeeIt = false;

        Vector3 _enemyPosition = _target.GetComponent<BaseCharacter>().GetHeadTransform().position;

        float _averageEyesPosition = 1.4f;

        Vector3 _possibleSpotPosition = new Vector3(_spot.transform.position.x, _spot.transform.position.y + _averageEyesPosition, _spot.transform.position.z);

        RaycastHit _hit;

        if (Physics.Raycast(_possibleSpotPosition, _enemyPosition, out _hit, Mathf.Infinity))
        {
            //если рейкаст попал в цель, то мы знаем, что можем его увидеть
            if (_hit.transform == _target.GetComponent<BaseCharacter>().GetHeadTransform())  
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

    private void AddToOccupied(CompanionCoverSpot _spot)
    {
        if (_freeCompanionCoverSpots.Contains(_spot) && !_lockCompanionCoverSpots.Contains(_spot))
        {
            _freeCompanionCoverSpots.Remove(_spot);
            _lockCompanionCoverSpots.Add(_spot);
        }
    }
    private void AddToUnoccupied(CompanionCoverSpot _spot)
    {
        if (_lockCompanionCoverSpots.Contains(_spot) && !_freeCompanionCoverSpots.Contains(_spot))
        {
            _lockCompanionCoverSpots.Remove(_spot);
            _freeCompanionCoverSpots.Add(_spot);
        }
    }


    public void AddTemporaryCoverSpots(CompanionCoverSpot _spot)
    {
        _freeCompanionCoverSpots.Add(_spot);
    }

    public void RemoveTemporaryCoverSpots(CompanionCoverSpot _spot)
    {
        if(_freeCompanionCoverSpots.Contains(_spot))
        _freeCompanionCoverSpots.Remove(_spot);

        if (_lockCompanionCoverSpots.Contains(_spot))
            _lockCompanionCoverSpots.Remove(_spot);
    }


    public CompanionCoverSpot GetCover(CompanionRangeBehavior _character, GameObject _target)
    {
        CompanionCoverSpot _bestCover = null;

        Vector3 _characterPosition = _character.transform.position;

        CompanionCoverSpot[] _possibleCoverSpots = _freeCompanionCoverSpots.ToArray();

        for (int i = 0; i < _possibleCoverSpots.Length; i++)
        {
            CompanionCoverSpot _spot = _possibleCoverSpots[i];

            if (!_spot.IsOccupied() // если спот свободен
                && Vector3.Distance(_characterPosition, _spot.transform.position) <= 5f
                && Vector3.Distance(_player.transform.position, _spot.transform.position) <= 4f)
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


    private bool CanSeeEnemyFromSpot(GameObject _target, CompanionCoverSpot _spot)
    {
        bool _canSeeIt = false;

        Vector3 _enemyPosition = _target.GetComponent<BaseCharacter>().GetHeadTransform().position;

        float _averageEyesPosition = 1.4f;

        Vector3 _possibleSpotPosition = new Vector3(_spot.transform.position.x, _spot.transform.position.y + _averageEyesPosition, _spot.transform.position.z);

        RaycastHit _hit;

        if (Physics.Raycast(_possibleSpotPosition, _enemyPosition, out _hit, Mathf.Infinity))
        {
            //если рейкаст попал в цель, то мы знаем, что можем его увидеть
            if (_hit.transform == _target.GetComponent<BaseCharacter>().GetHeadTransform())
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