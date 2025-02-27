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
    }

    public override void UpdateState()
    {
        Vector3 targetVector = enemy.attackTarget.transform.position - transform.position;
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
            else if (IsTargetInSight())
                attackTimer += Time.deltaTime;
            else
                attackTimer = attackReadyTime;

            if (attackTimer >= ATTACK_TIME)
                enemy.ChangeState(enemy.attackingState);
        }
    }

    public bool IsTargetInSight()
    {
        if (enemy.attackTarget == null) return false;

        Vector2 vectorToObject = enemy.attackTarget.transform.position - transform.position;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, vectorToObject.normalized, vectorToObject.magnitude);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.isTrigger || hit.collider.gameObject == enemy.gameObject || hit.collider.gameObject == enemy.attackTarget) continue;

            // Another object's collision is in the line of sight
            return false;
        }

        return true;
    }
}
