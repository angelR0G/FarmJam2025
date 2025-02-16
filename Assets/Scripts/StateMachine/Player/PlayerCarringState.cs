using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarringState : PlayerState
{
    private const float DROP_TIME = 0.5f;
    private const float MOVEMENT_TRESHOLD = 0.1f;
    private const float DETACH_DISTANCE = 0.3f;

    // Movement atributes
    public float speed = 1f;
    public CarriableComponent carriedObject;
    private float dropCooldown = 0;
    private float animatorBaseSpeed = 0f;

    public override void EnterState()
    {
        player.IsInteractionEnabled = false;
        dropCooldown = DROP_TIME;

        animatorBaseSpeed = player.animator.GetFloat("AnimSpeed");
        player.animator.SetTrigger("Carry");
        player.facingDirection = -(carriedObject.transform.position - transform.position).normalized;
        UpdateAnimation(false);
    }

    public override void ExitState()
    {
        player.IsInteractionEnabled = true;

        player.facingDirection = -player.facingDirection;
        player.animator.SetFloat("AnimSpeed", animatorBaseSpeed);
    }

    public override void FixedUpdateState()
    {
        MovePlayer();
        MoveCarriedObject();
    }

    public override void UpdateState()
    {
        if (dropCooldown > 0)
            dropCooldown -= Time.deltaTime;
        else if (player.inputComponent.interact)
        {
            player.ChangeState(player.walkingState);
        }
    }

    private void MovePlayer()
    {
        InputComponent inputComponent = player.inputComponent;
        Rigidbody2D body = player.body;

        bool isMoving = inputComponent.movementDirection != Vector2.zero;
        UpdateAnimation(isMoving);

        if (!isMoving)
            return;

        player.facingDirection = inputComponent.movementDirection;

        Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);
        body.MovePosition(playerPos + player.facingDirection * speed * Time.fixedDeltaTime);
    }

    private void MoveCarriedObject()
    {
        Vector3 carriedObjectDirection = transform.position - carriedObject.transform.position;
        float distanceToObject = carriedObjectDirection.magnitude - MOVEMENT_TRESHOLD;

        if (distanceToObject > DETACH_DISTANCE)
        {
            player.ChangeState(player.walkingState);
        }
        else if (distanceToObject > 0)
        {
            carriedObject.body.MovePosition(carriedObject.transform.position + carriedObjectDirection * distanceToObject);
        }
    }

    public void UpdateAnimation(bool isPlayerMoving)
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
        player.animator.SetFloat("AnimSpeed", isPlayerMoving ? animatorBaseSpeed : 0f);
    }
}
