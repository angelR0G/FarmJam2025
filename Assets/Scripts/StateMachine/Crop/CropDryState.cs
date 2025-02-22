using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropDryState : CropState
{
    public int requiredLiquidToGrow = 1000;
    public bool isWateredWithBlood = false;

    private int currentWater = 0;

    public override void EnterState()
    {
        crop.cropCollider.enabled = true;
        crop.sprite.sprite = crop.cropSprites[crop.wateredDays];
        currentWater = 0;
    }

    public override void ExitState()
    {
        crop.cropCollider.enabled = false;
    }

    public void Water(int amount)
    {
        currentWater += amount;

        if (currentWater >= requiredLiquidToGrow)
        {
            crop.ChangeState(crop.wateredState);
        }
    }

    public int GetRemainingWaterForGrowth()
    {
        return requiredLiquidToGrow - currentWater;
    }
}
