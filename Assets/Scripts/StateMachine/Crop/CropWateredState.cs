using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropWateredState : CropState
{
    const int DAYS_UNTIL_COLLECTABLE = 3;
    public override void EnterState()
    {
        GameManager.Instance.dayChanged += WaitUntilDawn;
    }

    public override void ExitState()
    {
        GameManager.Instance.hourChanged -= ChangeAtDawn;
    }

    private void WaitUntilDawn(object sender, int dayNumber)
    {
        GameManager gameManager = GameManager.Instance;

        gameManager.dayChanged -= WaitUntilDawn;
        gameManager.hourChanged += ChangeAtDawn;
    }

    private void ChangeAtDawn(object sender, int currentHour)
    {
        if (currentHour == 7)
        {
            crop.wateredDays++;

            crop.ChangeState(crop.wateredDays >= DAYS_UNTIL_COLLECTABLE ? crop.grownState : crop.dryState);
        }
    }
}
