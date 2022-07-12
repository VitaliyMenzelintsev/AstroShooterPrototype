using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSpot : MonoBehaviour, ISpot
{
    private bool _occupied = false;
    private Transform _occupier;

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
    internal virtual Color GetColor() => Color.cyan;
    public virtual bool IsEnemySpot() => false;
    private void OnDrawGizmos()
    {
        if (IsOccupied())
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = GetColor();
        }

        Gizmos.DrawWireSphere(transform.position, 0.5F);
    }
    
}
