using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropOfferingComponent : MonoBehaviour
{
    private ItemId expectedOffering = ItemId.Default;
    private int requiredCropsQuantity = 0;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<InteractionTriggerComponent>().interactionCallback = PlacingOffering;
    }

    public void PlacingOffering(PlayerComponent player)
    {
        if (IsOfferingCompleted())
        {
            Debug.Log("~~ Ya he colocado el cultivo. Ahora tengo que ofrecer mi sangre ~~");
        }
        else if (expectedOffering == ItemId.Default)
        {
            Debug.Log("~~ No tengo que hacer nada aqui, por ahora ~~");
        }
        else
        {
            // Check whether is offering the expected crop
            ItemComponent offering = player.inventory.GetEquipedItem();

            if (offering == null || offering.Id != expectedOffering)
            {
                Debug.Log("~~ Esto no es lo que deberia colocar ~~");
            }
            else
            {
                int offeringQuantity = Mathf.Min(player.inventory.GetEquipedItemQuantity(), requiredCropsQuantity);
                requiredCropsQuantity -= offeringQuantity;
                player.inventory.RemoveEquipedItem(offeringQuantity);
            }
        }
    }

    public void RequestNewOffering(ItemId newOffering, int requiredQuantity)
    {
        RemoveOffering();

        expectedOffering = newOffering;
        requiredCropsQuantity = requiredQuantity;
    }

    public bool IsOfferingCompleted()
    {
        return expectedOffering != ItemId.Default && requiredCropsQuantity <= 0;
    }

    public void RemoveOffering()
    {
        expectedOffering = ItemId.Default;
    }
}
