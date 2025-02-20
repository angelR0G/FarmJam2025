using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class StalkerAttackState : StalkerState
{
    private const float RUSH_SPEED = 2.5f;
    private const float MAX_RUSH_DISTANCE = 1.5f;

    public float rushDistance = 2f;
    private Vector3 rushDirection;
    private bool collisionDetectionEnabled = false;

    public override void EnterState()
    {
        rushDirection = (enemy.attackTarget.transform.position - transform.position).normalized;
        rushDistance = MAX_RUSH_DISTANCE;
        collisionDetectionEnabled = true;
    }

    public override void ExitState()
    {
        collisionDetectionEnabled = false;
    }

    public override void FixedUpdateState()
    {
        if (rushDistance > 0)
            RushAttack();
        else
            enemy.ChangeState(enemy.recoverState);
    }

    private void RushAttack()
    {
        enemy.body.MovePosition(enemy.transform.position + rushDirection * RUSH_SPEED * Time.fixedDeltaTime);
        rushDistance -= RUSH_SPEED * Time.fixedDeltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collisionDetectionEnabled) return;

        rushDistance = 0;
    }
}
