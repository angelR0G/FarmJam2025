using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropOfferingComponent : MonoBehaviour
{
    private ItemId expectedOffering = ItemId.Default;
    private int requiredCropsQuantity = 0;
    public ObjectIndicatorComponent indicator;
    public SpriteRenderer sprite;

    [Header("Sprites")]
    public Sprite emptyOfferingSprite;
    public Sprite cropOfferedSprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        GetComponent<InteractionTriggerComponent>().interactionCallback = PlacingOffering;
        indicator.RemoveSprite();
        indicator.SetVisibility(false);
    }

    public void PlacingOffering(PlayerComponent player)
    {
        if (IsOfferingCompleted())
        {
            DialogueSystem.Instance.DisplayDialogue(new Dialogue("The crops are placed. Now, I have to offer my blood."));
        }
        else if (expectedOffering == ItemId.Default)
        {
            DialogueSystem.Instance.DisplayDialogue(new Dialogue("I don't have to do anything here... For now."));
        }
        else
        {
            // Check whether is offering the expected crop
            ItemComponent offering = player.inventory.GetEquipedItem();

            if (offering == null || offering.Id != expectedOffering)
            {
                DialogueSystem.Instance.DisplayDialogue(new Dialogue("This isn't right. I have to offer something else."));
            }
            else
            {
                int offeringQuantity = Mathf.Min(player.inventory.GetEquipedItemQuantity(), requiredCropsQuantity);
                requiredCropsQuantity -= offeringQuantity;
                player.inventory.RemoveEquipedItem(offeringQuantity);

                if (requiredCropsQuantity > 0)
                    indicator.DisplayItem(expectedOffering, requiredCropsQuantity);
                else
                    indicator.SetVisibility(false);

                sprite.sprite = cropOfferedSprite;
            }
        }
    }

    public void RequestNewOffering(ItemId newOffering, int requiredQuantity)
    {
        if (expectedOffering != newOffering)
        {
            RemoveOffering();

            expectedOffering = newOffering;
            requiredCropsQuantity = requiredQuantity;
        }

        if (newOffering != ItemId.Default)
            indicator.DisplayItem(expectedOffering, requiredCropsQuantity);
    }

    public bool IsOfferingCompleted()
    {
        return expectedOffering != ItemId.Default && requiredCropsQuantity <= 0;
    }

    public void RemoveOffering()
    {
        expectedOffering = ItemId.Default;
        indicator.RemoveSprite();
        indicator.SetVisibility(false);
        sprite.sprite = emptyOfferingSprite;
    }
}
