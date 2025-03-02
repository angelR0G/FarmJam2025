using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightmareAvoidState : NightmareState
{
    private const float VULNERABLE_TIME_TO_ATTACK = 3f;
    private const float DISAPPEAR_DISTANCE = 3f;
    private const float AVOID_SPEED = 0.9f;
    private const float AVOID_SPEED_IN_LIGHT = 0.6f;

    private float targetTimeOutsideLight = 0f;
    public override void EnterState()
    {
        if (enemy.enemyTarget == null)
        {
            enemy.Disappear();
        }
        else
        {
            enemy.animator.SetTrigger("Fly");
            enemy.targetTimeInLight = 0;
            targetTimeOutsideLight = 0;
        }
    }

    public override void UpdateState()
    {
        // Check if the player is inside a light
        if (IsTargetInLight())
            targetTimeOutsideLight = Mathf.Max(targetTimeOutsideLight - Time.deltaTime, 0);
        else
            targetTimeOutsideLight += Time.deltaTime;

        if (targetTimeOutsideLight >= VULNERABLE_TIME_TO_ATTACK)
        {
            // If target returns to darkness, attacks again
            enemy.ChangeState(enemy.flyingState);
        }
        else
        {
            // Escapes from target until it can disappear
            if (enemy.GetDistanceToTarget() > DISAPPEAR_DISTANCE)
            {
                enemy.Disappear();
            }
            else
            {
                Vector3 avoidVector = transform.position - enemy.enemyTarget.transform.position;
                enemy.MoveTo(transform.position + avoidVector, enemy.lightDetector.IsInsideLight() ? AVOID_SPEED_IN_LIGHT : AVOID_SPEED, true);
            }
        }
    }
}
