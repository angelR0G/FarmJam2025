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
        if (hasFood) return;

        ItemComponent equipedItem = player.inventory.GetEquipedItem();
        if (equipedItem != null && equipedItem.Type == ItemType.Crop)
        {
            hasFood = true;
            player.inventory.RemoveEquipedItem();
            UpdateContainerState();
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
        trigger.enabled = !hasFood;
    }
}
