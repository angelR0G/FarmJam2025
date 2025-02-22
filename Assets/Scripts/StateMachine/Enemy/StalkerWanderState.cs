using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerWanderState : StalkerState
{
    private const float WALKING_SPEED = 0.5f;
    private const float TARGET_REACHED_TRESHOLD = 0.1f;
    private const float STAND_STILL_TIME = 5f;
    private const float MAX_TIME_TO_REACH_TARGET = 20f;
    private const float DETECTION_DISTANCE = 1.5f;

    public Vector3 targetPosition;
    private float nextTargetCountdown;
    private bool hasReachedTarget;
    private float playerDetectionCountdown;

    public override void EnterState()
    {
        nextTargetCountdown = STAND_STILL_TIME;
        UpdateTargetReached(true);
    }

    public override void FixedUpdateState()
    {
        UpdateMovementTimeout();
        MoveToTarget();

        if (playerDetectionCountdown > 0)
            playerDetectionCountdown -= Time.fixedDeltaTime;
        else
        {
            playerDetectionCountdown = 0.2f;
            CheckPlayerInSight();
        }
    }

    private void UpdateTargetPosition()
    {
        targetPosition = MapComponent.Instance.GetWalkablePositionAroundPoint(transform.position, 0.5f, 2f);
        nextTargetCountdown = MAX_TIME_TO_REACH_TARGET;
        UpdateTargetReached(false);
    }

    private void UpdateMovementTimeout()
    {
        nextTargetCountdown -= Time.fixedDeltaTime;

        if (nextTargetCountdown <= 0f)
        {
            UpdateTargetPosition();
        }
    }

    private void MoveToTarget()
    {
        if (hasReachedTarget || enemy.healthComp.IsStunned())
            return;

        // Check if it is in the target position
        Vector3 targetVector = targetPosition - transform.position;
        if (targetVector.magnitude < TARGET_REACHED_TRESHOLD)
        {
            UpdateTargetReached(true);
        }
        else
        {
            Vector3 movementVector = targetVector.normalized * Mathf.Min(targetVector.magnitude, WALKING_SPEED);
            enemy.body.velocity = movementVector;

            if (movementVector.x > 0) enemy.FlipSprite(true);
            else if (movementVector.x < 0) enemy.FlipSprite(false);
        }
    }

    private void UpdateTargetReached(bool isInTargetPosition)
    {
        if (hasReachedTarget == isInTargetPosition) return;

        hasReachedTarget = isInTargetPosition;

        if (hasReachedTarget)
            nextTargetCountdown = STAND_STILL_TIME;

        enemy.animator.SetTrigger(hasReachedTarget ? "StopMoving" : "StartMoving");
    }

    private void CheckPlayerInSight()
    {
        // Check if the player is in its detection range
        Collider2D player = Physics2D.OverlapCircle(transform.position, DETECTION_DISTANCE, LayerMask.GetMask("Player"));

        if (player == null || player.GetComponent<PlayerComponent>() == null) return;

        if (!enemy.IsGameObjectInSight(player.gameObject)) return;

        enemy.attackTarget = player.gameObject;
        enemy.ChangeState(enemy.stalkState);
    }
}
