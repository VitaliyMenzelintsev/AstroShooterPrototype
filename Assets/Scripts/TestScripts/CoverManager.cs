using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverManager : MonoBehaviour
{
    List<CoverSpot> unOccupiedCoverSpots = new List<CoverSpot>();
    List<CoverSpot> occupiedCoverSpots = new List<CoverSpot>();

    List<Character> allSoldiers = new List<Character>();

    private void Awake()
    {
        unOccupiedCoverSpots = new List<CoverSpot>(GameObject.FindObjectsOfType<CoverSpot>());

        allSoldiers = new List<Character>(GameObject.FindObjectsOfType<Character>());
    }

    void AddToOccupied(CoverSpot spot)
    {
        if (unOccupiedCoverSpots.Contains(spot))
        {
            unOccupiedCoverSpots.Remove(spot);
        }
        if (!occupiedCoverSpots.Contains(spot))
        {
            occupiedCoverSpots.Add(spot);
        }
    }
    void AddToUnoccupied(CoverSpot spot)
    {
        if (occupiedCoverSpots.Contains(spot))
        {
            occupiedCoverSpots.Remove(spot);
        }
        if (!unOccupiedCoverSpots.Contains(spot))
        {
            unOccupiedCoverSpots.Add(spot);
        }
    }

    public CoverSpot GetCoverTowardsTarget(Character soldier, Vector3 targetPosition, float maxAttackDistance, float minAttackDistance, CoverSpot prevCoverSpot)
    {
        CoverSpot bestCover = null;
        Vector3 soldierPosition = soldier.transform.position;

        CoverSpot[] possibleCoverSpots = unOccupiedCoverSpots.ToArray();

        for (int i = 0; i < possibleCoverSpots.Length; i++)
        {
            CoverSpot spot = possibleCoverSpots[i];

            if (!spot.IsOccupied() && spot.AmICoveredFrom(targetPosition) && Vector3.Distance(spot.transform.position, targetPosition) >= minAttackDistance && !CoverIsPastEnemyLine(soldier, spot))
            {
                if (bestCover == null)
                {
                    bestCover = spot;
                }
                else if (prevCoverSpot != spot && Vector3.Distance(bestCover.transform.position, soldierPosition) > Vector3.Distance(spot.transform.position, soldierPosition) && Vector3.Distance(spot.transform.position, targetPosition) < Vector3.Distance(soldierPosition, targetPosition))
                {
                    if (Vector3.Distance(spot.transform.position, soldierPosition) < Vector3.Distance(targetPosition, soldierPosition))
                    {
                        bestCover = spot;
                    }
                }
            }
        }

        if (bestCover != null)
        {
            bestCover.SetOccupier(soldier.transform);
            AddToOccupied(bestCover);
        }

        return bestCover;
    }

    public void ExitCover(CoverSpot spot)
    {
        if (spot != null)
        {
            spot.SetOccupier(null);

            AddToUnoccupied(spot);
        }
    }

    bool CoverIsPastEnemyLine(Character soldier, CoverSpot spot)
    {
        bool isPastEnemyLine = false;

        foreach (Character unit in allSoldiers)
        {
            if (soldier.MyTeam.getTeamNumber() != unit.MyTeam.getTeamNumber() && unit.MyVitals.getCurHealth() > 0)
            {
                if (spot.AmIBehindTargetPosition(soldier.transform.position, unit.transform.position))
                {
                    isPastEnemyLine = true;
                    break;
                }
            }
        }

        return isPastEnemyLine;
    }
}