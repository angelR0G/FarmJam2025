using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrosiveFollowState : CorrosiveState
{
    private const float FOLLOW_SPEED = 0.35f;
    private const float UNFOLLOW_DISTANCE = 1.35f;
    private const float START_EXPLOSION_DISTANCE = 0.4f;

    public override void EnterState()
    {
        enemy.animator.SetTrigger("StartMoving");
    }

    public override void FixedUpdateState()
    {
        float distanceToTarget = GetTargetDistance();

        if (distanceToTarget < START_EXPLOSION_DISTANCE)
        {
            enemy.ChangeState(enemy.chargingState);
        }
        else if (distanceToTarget > UNFOLLOW_DISTANCE)
        {
            enemy.ChangeState(enemy.returningState);
        }
        else
        {
            MoveTo(enemy.followTarget.transform.position, FOLLOW_SPEED);
        }
    }
}
