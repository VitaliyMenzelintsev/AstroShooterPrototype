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

    public bool AmICoveredFrom(Vector3 _targetPosition)
    {
        Vector3 _targetDirection = _targetPosition - transform.position;
        Vector3 _coverDirection = _cover.position - transform.position;

        if (Vector3.Dot(_coverDirection, _targetDirection) > 0.9F)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool AmIBehindTargetPosition(Vector3 _characterPosition, Vector3 _targetPosition)
    {
        Vector3 _characterToTargetDirection = _targetPosition - _characterPosition;
        Vector3 _characterToCoverDirection = transform.position - _characterPosition;

        float _characterToTargetDistance = Vector3.Distance(_characterPosition, _targetPosition);
        float _characterToCoverDistance = Vector3.Distance(_characterPosition, transform.position);

        if ((_characterToCoverDistance + 1) < _characterToTargetDistance)
        {
            return false;
        }

        if (Vector3.Dot(_characterToTargetDirection, _characterToCoverDirection) < 0.7F)
        {
            return false;
        }
        return true;
    }
}