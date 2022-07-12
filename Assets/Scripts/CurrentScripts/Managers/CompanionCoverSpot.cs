using UnityEngine;

public class CompanionCoverSpot : MonoBehaviour
{
    private bool _occupied = false;
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
            Gizmos.color = Color.cyan;
        }

        Gizmos.DrawWireSphere(transform.position, 0.5F);
    }
}