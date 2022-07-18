using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpot 
{
    public void SetOccupier(Transform _occupier);
    public Transform GetOccupier();
    public bool IsOccupied();

}
