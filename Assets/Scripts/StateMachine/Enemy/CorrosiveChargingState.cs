using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrosiveChargingState : CorrosiveState
{
    private const float CHARGE_TIME_TO_EXPLODE = 1f;
    private const float DETECTION_RANGE = 0.7f;

    public float chargeTime = 0;

    public override void EnterState()
    {
        chargeTime = 0;
        enemy.animator.SetTrigger("ChargeExplosion");
    }

    public override void FixedUpdateState()
    {
        if (GetTargetDistance() > DETECTION_RANGE) 
        {
            enemy.ChangeState(enemy.followState);
        }
        else
        {
            chargeTime += Time.fixedDeltaTime;
            if (chargeTime > CHARGE_TIME_TO_EXPLODE)
            {
                enemy.ChangeState(enemy.explodingState);
            }
        }
    }
}
