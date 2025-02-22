using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriableComponent : MonoBehaviour
{
    public PlayerComponent carrier;
    public Rigidbody2D body;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        GetComponent<InteractionTriggerComponent>().interactionCallback = SetCarrier;
    }

    public void SetCarrier(PlayerComponent newCarrier)
    {
        carrier = newCarrier;
        SetCollisionEnabled(newCarrier == null);

        if (newCarrier != null)
        {
            newCarrier.carringState.carriedObject = this;
            newCarrier.ChangeState(newCarrier.carringState);
        }
    }

    public void SetCollisionEnabled(bool newState)
    {
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            if (!col.isTrigger)
            {
                col.enabled = newState;
            }
        }
    }
}
