using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigSleepingState : PigState
{
    public override void EnterState()
    {
        GameManager.Instance.hourChanged += CheckWakeUpTime;
        pig.animator.SetTrigger("Sleeping");
    }

    public override void ExitState()
    {
        GameManager.Instance.hourChanged -= CheckWakeUpTime;
    }

    public void CheckWakeUpTime(object sender, int hour)
    {
        if (hour > 5 && hour < 19)
            pig.ChangeState(pig.walkingState);
    }
}
