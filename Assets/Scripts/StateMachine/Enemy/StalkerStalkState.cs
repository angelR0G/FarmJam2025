using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerStalkState : StalkerState
{
    private const float STALK_TIME_TO_ATTACK = 1.2f;
    private const float STALK_MAX_DISTANCE = 1.8f;

    private float stalkTime;
    private float inSightCheckTimeout;

    public AudioClip stalkSound;

    public override void EnterState()
    {
        stalkTime = STALK_TIME_TO_ATTACK;
        inSightCheckTimeout = 0.15f;

        enemy.animator.SetTrigger("Stalk");

        enemy.audioSource.clip = stalkSound;
        enemy.audioSource.Play();
    }

    public override void ExitState()
    {
        stalkTime = STALK_TIME_TO_ATTACK;
    }

    public override void UpdateState()
    {
        if (inSightCheckTimeout > 0f)
            inSightCheckTimeout -= Time.deltaTime;
        else
        {
            UpdateOrientation();

            inSightCheckTimeout += 0.15f;
            if (!enemy.IsGameObjectInSight(enemy.enemyTarget.gameObject, STALK_MAX_DISTANCE))
            {
                enemy.ChangeState(enemy.wanderState);
                return;
            }
        }

        // After stalking the player for enough time, start chasing them
        stalkTime -= Time.deltaTime;
        if (stalkTime <= 0)
        {
            enemy.ChangeState(enemy.chaseState);
        }
    }

    public void UpdateOrientation()
    {
        Vector3 targetDirection = enemy.enemyTarget.transform.position - transform.position;

        if (targetDirection.x > 0) enemy.FlipSprite(true);
        else if (targetDirection.x < 0) enemy.FlipSprite(false);
    }
}
