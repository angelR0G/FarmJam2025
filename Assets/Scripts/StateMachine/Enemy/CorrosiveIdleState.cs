using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrosiveIdleState : CorrosiveState
{
    private const float CHECK_PLAYER_INTERVAL = 1f;

    private float checkPlayerTimeout;
    public override void EnterState()
    {
        enemy.animator.SetTrigger("StopMoving");
    }

    public override void FixedUpdateState()
    {
        checkPlayerTimeout -= Time.fixedDeltaTime;

        if (checkPlayerTimeout <= 0)
        {
            checkPlayerTimeout = CHECK_PLAYER_INTERVAL;
            CheckPlayerInsideDetectionRange();
        }

    }
}
