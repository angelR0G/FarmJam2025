using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : StorageComponent
{
    [SerializeField] GameObject chestUIObject;
    [SerializeField] List<ItemId> initialObjects;

    void Start()
    {
        transform.GetComponent<InteractionTriggerComponent>().interactionCallback = OpenChestUI;

        foreach (ItemId item in initialObjects)
        {
            AddItem(item);
        }
    }

    public void OpenChestUI(PlayerComponent player)
    {
        chestUIObject.GetComponent<ChestDisplay>().OpenChest(player, this);
    }
}
