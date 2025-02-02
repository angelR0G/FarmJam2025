using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputComponent))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerComponent : MonoBehaviour
{
    // Components
    private InputComponent inputComponent;
    private Rigidbody2D body;

    // Movement atributes
    const float MOV_ACCELERATION = 20f;
    const float STOP_ACCELERATION = 40f;
    const float MAX_SPEED = 5f;
    private float currentSpeed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        inputComponent = GetComponent<InputComponent>();
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        bool isMoving = inputComponent.movementDirection != Vector2.zero;

        if (isMoving)
        {
            currentSpeed = Mathf.Min(currentSpeed + MOV_ACCELERATION * Time.deltaTime, MAX_SPEED);
            body.velocity = inputComponent.movementDirection * currentSpeed;
        }
        else
        {
            currentSpeed = Mathf.Max(currentSpeed - STOP_ACCELERATION * Time.deltaTime, 0f);
            body.velocity = body.velocity.normalized * currentSpeed;
        }
    }
}
