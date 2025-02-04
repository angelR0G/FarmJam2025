using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteractionTriggerComponent : MonoBehaviour
{
    public Action<PlayerComponent> interactionCallback;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerComponent player;
        if (collision.gameObject.TryGetComponent<PlayerComponent>(out player))
        {
            player.SetInteractableObject(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerComponent player;
        if (collision.gameObject.TryGetComponent<PlayerComponent>(out player))
        {
            player.RemoveInteractableObject(this);
        }
    }

    public void Interact(PlayerComponent interactingPlayer)
    {
        if (interactionCallback != null)
        {
            #if UNITY_EDITOR
            Debug.Log("Interacting with " + gameObject.name);
            #endif
            interactionCallback.Invoke(interactingPlayer);
        }
    }
}
