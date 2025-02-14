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
    public UnityEvent equipTool1Event;
    public UnityEvent equipTool2Event;
    public UnityEvent equipTool3Event;
    public UnityEvent equipWeaponEvent;
    public UnityEvent equipTorchEvent;
    public UnityEvent equipItem1Event;
    public UnityEvent equipItem2Event;
    public UnityEvent equipItem3Event;
    public UnityEvent equipItem4Event;
    public UnityEvent equipItem5Event;
    public UnityEvent equipNextItemEvent;
    public UnityEvent equipPreviousItemEvent;


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

    public void Update()
    {
        interact = false;
    }

    public void OnEquipTool1(InputValue value)
    {
        if (value.isPressed)
        {
            equipTool1Event.Invoke();
        }
    }

    public void OnEquipTool2(InputValue value)
    {
        if (value.isPressed)
        {
            equipTool2Event.Invoke();
        }
    }

    public void OnEquipTool3(InputValue value)
    {
        if (value.isPressed)
        {
            equipTool3Event.Invoke();
        }
    }

    public void OnEquipWeapon(InputValue value)
    {
        if (value.isPressed)
        {
            equipWeaponEvent.Invoke();
        }
    }

    public void OnEquipTorch(InputValue value)
    {
        if (value.isPressed)
        {
            equipTorchEvent.Invoke();
        }
    }

    public void OnEquipItem1(InputValue value)
    {
        if (value.isPressed)
        {
            equipItem1Event.Invoke();
        }
    }

    public void OnEquipItem2(InputValue value)
    {
        if (value.isPressed)
        {
            equipItem2Event.Invoke();
        }
    }
    public void OnEquipItem3(InputValue value)
    {
        if (value.isPressed)
        {
            equipItem3Event.Invoke();
        }
    }
    public void OnEquipItem4(InputValue value)
    {
        if (value.isPressed)
        {
            equipItem4Event.Invoke();
        }
    }
    public void OnEquipItem5(InputValue value)
    {
        if (value.isPressed)
        {
            equipItem5Event.Invoke();
        }
    }

    public void OnEquipNextItem(InputValue value)
    {
        if (value.Get<float>() > 1f)
            equipNextItemEvent.Invoke();
    }

    public void OnEquipPreviousItem(InputValue value)
    {
        if (value.Get<float>() > 1f)
            equipPreviousItemEvent.Invoke();
    }
}
