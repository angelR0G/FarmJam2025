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
        speed = 0.0f;
        attackCooldown = CHASE_TIME_BEFORE_ATTACK;
    }

    public override void FixedUpdateState()
    {
        Vector3 vectorToTarget = enemy.attackTarget.transform.position - transform.position;
        float distanceToTarget = vectorToTarget.magnitude;

        // Depending on the distance to the target, change its behaviour
        if (attackCooldown <= 0 && distanceToTarget < ATTACK_DISTANCE && CanAmbushPlayer())
        {
            enemy.ChangeState(enemy.attackState);
        }
        else if (distanceToTarget < CHASE_MAX_DISTANCE)
        {
            MoveTo(vectorToTarget.normalized);
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

    private void MoveTo(Vector3 direction)
    {
        // Obstacle detection
        Vector3 avoidVector = enemy.GetAvoidanceDirection(direction, speed, new List<GameObject> { enemy.gameObject, enemy.attackTarget });
        direction = (direction + avoidVector).normalized;

        float targetSpeed = avoidVector.sqrMagnitude <= 0 ? MAX_SPEED : AVOIDING_OBSTACLE_MAX_SPEED;

        if (speed < targetSpeed)
            speed = Mathf.Min(speed + ACCELERATION * Time.fixedDeltaTime, targetSpeed);

        else if (speed > targetSpeed)
            speed = Mathf.Max(speed - ACCELERATION * Time.fixedDeltaTime, targetSpeed);

        enemy.body.MovePosition(enemy.transform.position + direction * speed * Time.fixedDeltaTime);
    }

    private bool CanAmbushPlayer(bool useSimplifiedRaycast = false)
    {
        Vector2 attackDirection = enemy.attackTarget.transform.position - transform.position;
        RaycastHit2D[] hits;

        if (useSimplifiedRaycast)
            hits = Physics2D.RaycastAll(transform.position, attackDirection, attackDirection.magnitude);
        else
            hits = Physics2D.CircleCastAll(transform.position, enemy.enemyCollider.radius, attackDirection, attackDirection.magnitude);

        foreach(RaycastHit2D hit in hits)
        {
            if (hit.collider.isTrigger || hit.collider.gameObject == enemy.gameObject) continue;

            if (hit.collider.gameObject != enemy.attackTarget)
                return false;
        }

        return true;
    }
}
