using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrosiveReturningState : CorrosiveState
{
    private const float RETURNING_SPEED = 0.3f;
    public override void EnterState()
    {
        enemy.animator.SetTrigger("StartMoving");
    }

    public override void FixedUpdateState()
    {
        if (Vector3.Distance(enemy.originPosition, transform.position) < RETURNING_SPEED * Time.fixedDeltaTime)
        {
            enemy.ChangeState(enemy.idleState);
        }
        else
        {
            MoveTo(enemy.originPosition, RETURNING_SPEED);
            CheckPlayerInsideDetectionRange();
        }
    }
}
