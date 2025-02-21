using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class StalkerAttackState : StalkerState
{
    private const float RUSH_SPEED = 3f;
    private const float MAX_RUSH_DISTANCE = 1.5f;

    public float rushDistance = 2f;
    private Vector3 rushDirection;

    public override void EnterState()
    {
        rushDirection = (enemy.attackTarget.transform.position - transform.position).normalized;
        rushDistance = MAX_RUSH_DISTANCE;

        enemy.attackComp.UpdateDamageArea(0.1f, enemy.enemyCollider.radius * 2, Vector2.SignedAngle(Vector2.right, rushDirection));
        enemy.attackComp.SetDamageAreaActive(true);
    }

    public override void ExitState()
    {
        enemy.attackComp.SetDamageAreaActive(false);
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

    public void FinishAttack()
    {
        rushDistance = 0;
    }
}
