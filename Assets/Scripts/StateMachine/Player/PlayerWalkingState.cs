using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingState : PlayerState
{
    // Movement atributes
    public float acceleration = 4f;
    public float stopAcceleration = 8f;
    public float maxSpeed = 1f;
    private float currentSpeed = 0f;

    public AudioClip walkingSound;

    public override void EnterState()
    {
        player.body.velocity = Vector2.zero;
        currentSpeed = 0f;
        UpdateAnimation(true);
        player.IsInteractionEnabled = true;

        player.audioSource.clip = walkingSound;
    }

    public override void ExitState()
    {
        player.body.velocity = Vector2.zero;

        player.audioSource.loop = false;
    }

    public override void FixedUpdateState()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        InputComponent inputComponent = player.inputComponent;
        Rigidbody2D body = player.body;

        bool isMoving = inputComponent.movementDirection != Vector2.zero;
        Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);
        float previousSpeed = currentSpeed;

        if (isMoving)
        {
            // Accelerating
            player.facingDirection = inputComponent.movementDirection;
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.fixedDeltaTime, maxSpeed);
        }
        else
        {
            // Stoping
            currentSpeed = Mathf.Max(currentSpeed - stopAcceleration * Time.fixedDeltaTime, 0f);
        }

        body.MovePosition(playerPos + player.facingDirection * currentSpeed * Time.fixedDeltaTime);

        UpdateAnimation((currentSpeed == 0 && previousSpeed > 0) || (previousSpeed == 0 && currentSpeed > 0));
    }

    public void UpdateAnimation(bool updateMovingState)
    {
        int direction;

        if (player.facingDirection.x > 0.2)
            direction = 1;
        else if (player.facingDirection.x < -0.2)
            direction = 3;
        else if (player.facingDirection.y > 0.6)
            direction = 0;
        else
            direction = 2;

        player.animator.SetInteger("Direction", direction);
        if (updateMovingState)
        {
            player.animator.SetTrigger(currentSpeed > 0f ? "StartMoving" : "StopMoving");

            if (currentSpeed > 0f)
            {
                player.audioSource.loop = true;
                player.audioSource.Play();
            }
            else
                player.audioSource.loop = false;
        }
    }
}
