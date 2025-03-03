using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class InteractionTriggerComponent : MonoBehaviour
{
    public Action<PlayerComponent> interactionCallback;
    public UnityEvent onTriggerDestroyed = new UnityEvent();
    private UnityAction destroyCallback;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerComponent player;
        if (collision.gameObject.TryGetComponent<PlayerComponent>(out player))
        {
            player.SetInteractableObject(this);
            onTriggerDestroyed.AddListener(() => player.RemoveInteractableObject(this));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerComponent player;
        if (collision.gameObject.TryGetComponent<PlayerComponent>(out player))
        {
            player.RemoveInteractableObject(this);
            onTriggerDestroyed.RemoveAllListeners();
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

    private void OnDestroy()
    {
        onTriggerDestroyed.Invoke();
    }
}
