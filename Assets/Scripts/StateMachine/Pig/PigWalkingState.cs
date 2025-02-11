using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigWalkingState : PigState
{
    private const float WALKING_SPEED = 0.5f;
    private const float TARGET_REACHED_TRESHOLD = 0.2f;
    private const float STAND_STILL_MIN_TIME = 2f;
    private const float STAND_STILL_MAX_TIME = 10f;
    private const float MAX_TIME_TO_REACH_TARGET = 30f;

    private Vector3 targetPosition;
    private float nextTargetCountdown;
    private bool hasReachedTarget;

    public override void EnterState()
    {
        RequestNewTargetPosition();
        nextTargetCountdown = MAX_TIME_TO_REACH_TARGET;
    }

    public override void FixedUpdateState()
    {
        UpdateMovementTimeout();
        UpdateMovement();
    }

    private void UpdateMovementTimeout()
    {
        nextTargetCountdown -= Time.fixedDeltaTime;

        if (nextTargetCountdown <= 0f)
        {
            RequestNewTargetPosition();
        }
    }

    private void UpdateMovement()
    {
        Vector3 targetVector = targetPosition - transform.position;

        // Check if it is already in the target position
        UpdateTargetReached(targetVector.magnitude < TARGET_REACHED_TRESHOLD);
        if (hasReachedTarget)
            return;

        Vector3 movementVector = targetVector.normalized * Mathf.Min(targetVector.magnitude, WALKING_SPEED);

        pig.body.velocity = movementVector;
    }

    private void UpdateTargetReached(bool isInTargetPosition)
    {
        if (hasReachedTarget == isInTargetPosition) return;
        hasReachedTarget = isInTargetPosition;

        nextTargetCountdown = hasReachedTarget ? Random.Range(STAND_STILL_MIN_TIME, STAND_STILL_MAX_TIME) : MAX_TIME_TO_REACH_TARGET;
    }

    private void RequestNewTargetPosition()
    {
        targetPosition = new Vector3(Random.Range(-6, -4), Random.Range(-2, 2));
    }
}
