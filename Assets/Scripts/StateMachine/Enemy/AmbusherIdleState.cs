using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbusherIdleState : AmbusherState
{
    private const float ATTACK_TIME = 1.7f;
    private const float MIN_TIME_IN_SIGHT_TO_ATTACK = 0.45f;
    private const float HIDE_DISTANCE = 2f;

    private float attackTimer = 0f;

    public AudioClip appearSound;

    public override void EnterState()
    {
        if (enemy.enemyTarget == null || enemy.enemyTarget.IsSafe())
        {
            enemy.ChangeState(enemy.hidingState);
            return;
        }


        if (enemy.hidden)
        {
            enemy.hidden = false;
            enemy.sprite.enabled = true;
            enemy.animator.SetTrigger("Appear");

            enemy.audioSource.clip = appearSound;
            enemy.audioSource.Play();
        }
        else
        {
            enemy.animator.SetTrigger("Idle");
        }

        attackTimer = 0;
        enemy.enemyTarget.onEnterSafeArea.AddListener(OnTargetEnterSafeArea);
    }

    public override void ExitState()
    {
        enemy.enemyTarget.onEnterSafeArea.RemoveListener(OnTargetEnterSafeArea);
    }

    public override void UpdateState()
    {
        Vector3 targetVector = enemy.enemyTarget.transform.position - transform.position;
        if (targetVector.x > 0) enemy.FlipSprite(true);
        else if (targetVector.x < 0) enemy.FlipSprite(false);

        float distanceToTarget = targetVector.magnitude;
        if (distanceToTarget > HIDE_DISTANCE)
        {
            enemy.ChangeState(enemy.hidingState);
        }
        else
        {
            float attackReadyTime = ATTACK_TIME - MIN_TIME_IN_SIGHT_TO_ATTACK;

            // When attack has charged enough, player must be exposed for a short amount of time before attacking
            if (attackTimer < attackReadyTime)
                attackTimer = Mathf.Min(attackTimer + Time.deltaTime, attackReadyTime);
            else if (enemy.IsGameObjectInSight(enemy.enemyTarget.gameObject))
                attackTimer += Time.deltaTime;
            else
                attackTimer = attackReadyTime;

            if (attackTimer >= ATTACK_TIME)
                enemy.ChangeState(enemy.attackingState);
        }
    }

    private void OnTargetEnterSafeArea()
    {
        enemy.ChangeState(enemy.hidingState);
    }
}
