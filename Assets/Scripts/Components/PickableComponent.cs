using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableComponent : MonoBehaviour
{
    public ItemId id;
    public int amount = 1;
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        GetComponent<InteractionTriggerComponent>().interactionCallback = OnPickUp;

        sprite.sprite = ItemFactory.GetItem(id).sprite;

        if (amount <= 0) Destroy(gameObject);
    }

    private void OnPickUp(PlayerComponent player)
    {
        int savedItems = player.inventory.AddItem(id, amount);

        if (savedItems == amount)
            Destroy(gameObject);
        else if (savedItems == 0)
            Debug.Log("~~ Esto no me cabe ~~");
        else
            amount -= savedItems;
    }
}
