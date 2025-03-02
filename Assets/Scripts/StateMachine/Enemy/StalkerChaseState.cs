using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerChaseState : StalkerState
{
    private const float ATTACK_DISTANCE = 0.8f;
    private const float CHASE_MAX_DISTANCE = 3f;
    private const float MAX_SPEED = 1.5f;
    private const float AVOIDING_OBSTACLE_MAX_SPEED = 0.8f;
    private const float ACCELERATION = 0.75f;
    private const float CHASE_TIME_BEFORE_ATTACK = 1f;

    private float speed;
    private float attackCooldown;

    public override void EnterState()
    {
        if (enemy.enemyTarget == null || enemy.enemyTarget.IsSafe())
        {
            enemy.ChangeState(enemy.wanderState);
            return;
        }

        speed = 0.0f;
        attackCooldown = CHASE_TIME_BEFORE_ATTACK;

        enemy.animator.SetTrigger("Chase");
        enemy.enemyTarget.onEnterSafeArea.AddListener(OnTargetEnterSafeArea);
    }

    public override void ExitState()
    {
        enemy.enemyTarget.onEnterSafeArea.RemoveListener(OnTargetEnterSafeArea);
    }

    public override void FixedUpdateState()
    {
        Vector3 vectorToTarget = enemy.enemyTarget.transform.position - transform.position;
        float distanceToTarget = enemy.GetDistanceToTarget();

        // Depending on the distance to the target, change its behaviour
        if (attackCooldown <= 0 && distanceToTarget < ATTACK_DISTANCE && CanAmbushPlayer())
        {
            enemy.ChangeState(enemy.attackState);
        }
        else if (distanceToTarget < CHASE_MAX_DISTANCE)
        {
            ChaseTarget();
        }
        else
        {
            enemy.ChangeState(enemy.wanderState);
        }

        // If it has no line of sight with target, resets attack cooldown
        if (attackCooldown > 0 && !CanAmbushPlayer(true))
        {
            attackCooldown = CHASE_TIME_BEFORE_ATTACK;
        }
    }

    public override void UpdateState()
    {
        if (attackCooldown > 0) attackCooldown -= Time.deltaTime;
    }

    private void ChaseTarget()
    {
        Vector3 targetDirection = (enemy.enemyTarget.transform.position - transform.position).normalized;

        // Obstacle detection
        Vector3 avoidVector = enemy.GetAvoidanceDirection(targetDirection, speed);
        targetDirection = (targetDirection + avoidVector).normalized;

        float targetSpeed = avoidVector.sqrMagnitude <= 0 ? MAX_SPEED : AVOIDING_OBSTACLE_MAX_SPEED;

        if (speed < targetSpeed)
            speed = Mathf.Min(speed + ACCELERATION * Time.fixedDeltaTime, targetSpeed);

        else if (speed > targetSpeed)
            speed = Mathf.Max(speed - ACCELERATION * Time.fixedDeltaTime, targetSpeed);

        enemy.body.MovePosition(enemy.transform.position + targetDirection * speed * Time.fixedDeltaTime);

        if (targetDirection.x > 0) enemy.FlipSprite(true);
        else if (targetDirection.x < 0) enemy.FlipSprite(false);
    }

    private bool CanAmbushPlayer(bool useSimplifiedRaycast = false)
    {
        Vector2 attackDirection = (enemy.enemyTarget.transform.position - transform.position).normalized;
        RaycastHit2D[] hits;

        if (useSimplifiedRaycast)
            hits = Physics2D.RaycastAll(transform.position, attackDirection, attackDirection.magnitude);
        else
            hits = Physics2D.CircleCastAll(transform.position, enemy.enemyCollider.radius, attackDirection, attackDirection.magnitude);

        foreach(RaycastHit2D hit in hits)
        {
            if (hit.collider.isTrigger || hit.collider.gameObject == enemy.gameObject) continue;

            if (hit.collider.gameObject != enemy.enemyTarget.gameObject)
                return false;
        }

        return true;
    }

    private void OnTargetEnterSafeArea()
    {
        enemy.ChangeState(enemy.wanderState);
    }
}
