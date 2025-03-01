using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodContainerComponent : MonoBehaviour
{
    public Collider2D trigger;
    public SpriteRenderer sprite;

    public Sprite emptySprite;
    public Sprite filledSprite;

    private bool hasFood;

    public bool HasFood { get { return hasFood; } private set { hasFood = value; } }

    private void Start()
    {
        GetComponent<InteractionTriggerComponent>().interactionCallback = FillContainer;
        sprite = GetComponent<SpriteRenderer>();
        UpdateContainerState();
    }

    public void FillContainer(PlayerComponent player)
    {
        if (hasFood) 
        {
            DialogueSystem.Instance.DisplayDialogue(new Dialogue("The trough is full."));
            return;
        }

        ItemComponent equipedItem = player.inventory.GetEquipedItem();
        if (equipedItem != null && equipedItem.Type == ItemType.Crop)
        {
            hasFood = true;
            player.inventory.RemoveEquipedItem();
            UpdateContainerState();
        }
        else
        {
            DialogueSystem.Instance.DisplayDialogue(new Dialogue("I can put crops here to feed the pigs."));
        }
    }

    public void EmptyContainer()
    {
        hasFood = false;
        UpdateContainerState();
    }

    public void UpdateContainerState()
    {
        sprite.sprite = hasFood ? filledSprite : emptySprite;
    }

    public Vector3 GetContainerPosition()
    {
        Vector3 pos = transform.position;
        pos.y += sprite.size.y / 2;

        return pos;
    }
}
