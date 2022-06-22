using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverSpot : MonoBehaviour
{
    private bool _occupied = false; // занята или нет точка
    private Transform _occupier;
    private Transform _cover;

    private void Start()
    {
        _cover = transform.parent;
    }

    public void SetOccupier(Transform _occupier)
    {
        this._occupier = _occupier;
        if (this._occupier == null)
        {
            _occupied = false;
        }
        else
        {
            _occupied = true;
        }
    }

    public Transform GetOccupier()
    {
        return _occupier;
    }

    public bool IsOccupied()
    {
        return _occupied;
    }

    private void OnDrawGizmos()
    {
        if (IsOccupied())
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

    public bool AmICoveredFrom(Vector3 _targetPosition)
    {
        Vector3 _targetDirection = _targetPosition - transform.position;
        Vector3 _coverDirection = _cover.position - transform.position;

        if(Vector3.Dot(_coverDirection, _targetPosition) > 0.9f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

