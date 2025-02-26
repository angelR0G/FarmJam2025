using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigPanicState : PigState
{
    private const float PANIC_TIME = 20f;
    private const float RUNNING_SPEED = 2.5f;
    private const float TARGET_REACHED_TRESHOLD = 0.2f;

    private Vector3 targetPosition;
    private float panicTimeout;

    private float defaultAnimSpeed = 0;

    public AudioClip pigPanicSound;

    public override void EnterState()
    {
        RequestNewTargetPosition();
        panicTimeout = PANIC_TIME;

        pig.animator.SetTrigger("StartMoving");
        defaultAnimSpeed = pig.animator.GetFloat("AnimSpeed");
        pig.animator.SetFloat("AnimSpeed", 0.75f);

        pig.audioSource.PlayOneShot(pigPanicSound);
    }

    public override void ExitState()
    {
        pig.animator.SetFloat("AnimSpeed", defaultAnimSpeed);
    }

    public override void FixedUpdateState()
    {
        if (panicTimeout <= 0)
        {
            pig.ChangeState(pig.walkingState);
        }
        else
        {
            UpdateTarget();
            UpdateMovement();

            panicTimeout -= Time.fixedDeltaTime;
        }
    }

    private void UpdateMovement()
    {
        if (pig.healthComp.IsStunned())
            return;

        
        Vector3 targetVector = targetPosition - transform.position;
        Vector3 movementVector = targetVector.normalized * RUNNING_SPEED;

        pig.body.velocity = movementVector;
        pig.facingDirection = targetVector.normalized;

        if (movementVector.x > 0) pig.FlipSprite(true);
        else if (movementVector.x < 0) pig.FlipSprite(false);
    }

    private void UpdateTarget()
    {
        // Check if it is already in the target position
        if ((targetPosition - transform.position).magnitude < TARGET_REACHED_TRESHOLD)
            RequestNewTargetPosition();
    }

    private void RequestNewTargetPosition()
    {
        targetPosition = pig.farmyard.GetRandomPositionInFarmyard();
    }

    public void RestartPanicTime()
    {
        panicTimeout = PANIC_TIME;
    }
}
