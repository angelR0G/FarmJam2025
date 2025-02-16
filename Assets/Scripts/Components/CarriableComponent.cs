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

        if (newCarrier != null)
        {
            newCarrier.carringState.carriedObject = this;
            newCarrier.ChangeState(newCarrier.carringState);
        }
    }
}
