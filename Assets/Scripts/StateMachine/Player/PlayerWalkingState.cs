using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingState : MonoBehaviour, IState
{
    private PlayerComponent player;

    // Movement atributes
    const float MOV_ACCELERATION = 4f;
    const float STOP_ACCELERATION = 8f;
    const float MAX_SPEED = 1f;
    private float currentSpeed = 0f;

    public void Awake()
    {
        player = GetComponent<PlayerComponent>();
    }

    public void EnterState()
    {
        currentSpeed = 0f;
    }

    public void ExitState() { }

    public void UpdateState() { }

    public void FixedUpdateState()
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
            currentSpeed = Mathf.Min(currentSpeed + MOV_ACCELERATION * Time.fixedDeltaTime, MAX_SPEED);
            body.MovePosition(playerPos + inputComponent.movementDirection * currentSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Stoping
            currentSpeed = Mathf.Max(currentSpeed - STOP_ACCELERATION * Time.fixedDeltaTime, 0f);
            body.MovePosition(playerPos + body.velocity.normalized * currentSpeed * Time.fixedDeltaTime);
        }
    }
}
