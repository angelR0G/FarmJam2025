using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDiggingState : PlayerBaseState
{
    [SerializeField]
    private GameObject plotPrefab;

    public override void EnterState() 
    { 
        if (HasSpaceToDig())
        {
            GameObject newPlot = Instantiate(plotPrefab);
            newPlot.transform.position = player.transform.position;
        }

        player.ChangeState(player.walkingState);
    }

    private bool HasSpaceToDig()
    {
        return true;
    }
}
