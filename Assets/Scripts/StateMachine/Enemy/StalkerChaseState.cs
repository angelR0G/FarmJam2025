using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerChaseState : MonoBehaviour
{

    private void UpdateMovement()
    {/*
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

        UpdateAnimation((currentSpeed == 0 && previousSpeed > 0) || (previousSpeed == 0 && currentSpeed > 0));*/
    }
}
