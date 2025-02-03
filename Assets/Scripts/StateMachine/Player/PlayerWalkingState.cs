using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingState : IState
{
    private PlayerComponent player;

    // Movement atributes
    const float MOV_ACCELERATION = 20f;
    const float STOP_ACCELERATION = 40f;
    const float MAX_SPEED = 5f;
    private float currentSpeed = 0f;

    public PlayerWalkingState(PlayerComponent p)
    {
        player = p;
    }

    public void EnterState()
    {
        currentSpeed = 0f;
    }

    public void ExitState() { }

    public void UpdateState()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        InputComponent inputComponent = player.inputComponent;
        Rigidbody2D body = player.body;

        bool isMoving = inputComponent.movementDirection != Vector2.zero;

        if (isMoving)
        {
            // Accelerating
            currentSpeed = Mathf.Min(currentSpeed + MOV_ACCELERATION * Time.deltaTime, MAX_SPEED);
            body.velocity = inputComponent.movementDirection * currentSpeed;
        }
        else
        {
            // Stoping
            currentSpeed = Mathf.Max(currentSpeed - STOP_ACCELERATION * Time.deltaTime, 0f);
            body.velocity = body.velocity.normalized * currentSpeed;
        }
    }
}
