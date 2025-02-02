using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputComponent : MonoBehaviour
{
    public Vector2 movementDirection {  get; private set; } = Vector2.zero;
    public bool interact { get; private set; } = false;


    public UnityEvent interactInputEvent;


    public void OnMove(InputValue value)
    {
        movementDirection = value.Get<Vector2>();
    }

    public void OnInteract(InputValue value)
    {
        interact = value.isPressed;

        if (interact)
        {
            interactInputEvent.Invoke();
        }
    }
}
