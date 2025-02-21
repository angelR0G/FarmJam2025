using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerRecoverState : StalkerState
{
    private float recoveringTime;

    public override void EnterState()
    {
        recoveringTime = 2.5f;
    }

    public override void UpdateState()
    {
        recoveringTime -= Time.deltaTime;

        if (recoveringTime <= 0f)
            enemy.ChangeState(enemy.chaseState);
    }
}
