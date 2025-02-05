using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingState : PlayerBaseState
{
    // Movement atributes
    public float acceleration = 4f;
    public float stopAcceleration = 8f;
    public float maxSpeed = 1f;
    private float currentSpeed = 0f;

    public override void EnterState()
    {
        currentSpeed = 0f;
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

        if (isMoving)
        {
            // Accelerating
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.fixedDeltaTime, maxSpeed);
            body.MovePosition(playerPos + inputComponent.movementDirection * currentSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Stoping
            currentSpeed = Mathf.Max(currentSpeed - stopAcceleration * Time.fixedDeltaTime, 0f);
            body.MovePosition(playerPos + body.velocity.normalized * currentSpeed * Time.fixedDeltaTime);
        }
    }
}
