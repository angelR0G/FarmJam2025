using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerChaseState : StalkerState
{
    private const float ATTACK_DISTANCE = 0.8f;
    private const float CHASE_MAX_DISTANCE = 3f;
    private const float MAX_SPEED = 1.5f;
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
    }

    public override void UpdateState()
    {
        if (attackCooldown > 0) attackCooldown -= Time.deltaTime;
    }

    private void MoveTo(Vector3 direction)
    {
        if (speed < MAX_SPEED)
        {
            speed = Mathf.Min(speed + ACCELERATION * Time.fixedDeltaTime, MAX_SPEED);
        }

        // Obstacle detection
        Vector3 avoidVector = enemy.GetAvoidanceDirection(direction, speed, new List<GameObject> { enemy.gameObject, enemy.attackTarget });
        direction = (direction + avoidVector).normalized;

        enemy.body.MovePosition(enemy.transform.position + direction * speed * Time.fixedDeltaTime);
    }

    private bool CanAmbushPlayer()
    {
        Vector2 attackDirection = enemy.attackTarget.transform.position - transform.position;
        Vector2 boxSize = new Vector2(enemy.enemyCollider.radius * 2, enemy.enemyCollider.radius * 2);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, boxSize, Vector2.SignedAngle(Vector2.right, attackDirection), attackDirection);

        foreach(RaycastHit2D hit in hits)
        {
            if (hit.collider.isTrigger || hit.collider.gameObject == enemy.gameObject) continue;

            return hit.collider.gameObject == enemy.attackTarget;
        }

        return true;
    }
}
