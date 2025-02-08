using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableComponent : MonoBehaviour
{
    public ItemId id;
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        GetComponent<InteractionTriggerComponent>().interactionCallback = OnPickUp;

        sprite.sprite = ItemFactory.GetItem(id).sprite;
    }

    private void OnPickUp(PlayerComponent player)
    {
        if (player.inventory.AddItem(id)) Destroy(gameObject);
    }
}
