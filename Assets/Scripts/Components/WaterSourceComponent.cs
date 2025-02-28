using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WaterSourceComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<InteractionTriggerComponent>().interactionCallback = DisplayMessage;
    }

    private void DisplayMessage(PlayerComponent player)
    {
        if (player.wateringState.GetFillPorcentage() < 1)
        {
            DialogueSystem.Instance.DisplayDialogue(new Dialogue("I can fill my watering can here."));
        }
        else
        {
            DialogueSystem.Instance.DisplayDialogue(new Dialogue("My watering can is full, but I can fill it up here later."));
        }
    }
}
