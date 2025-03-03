using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestDisplay : MonoBehaviour
{
    private PlayerComponent player;
    private StorageComponent chestStorage;
    private List<GameObject> inventorySlots = new List<GameObject>();

    public void OpenChest(PlayerComponent p, StorageComponent chest)
    {
        player = p;
        chestStorage = chest;

        player.inputComponent.interactInputEvent.AddListener(CloseShop);
        player.SetEnabled(false);

        UpdateItemSprites();
        SetChestUIEnabled(true);
    }

    public void CloseShop()
    {
        player.inputComponent.interactInputEvent.RemoveListener(CloseShop);
        player.SetEnabled(true);

        SetChestUIEnabled(false);

        player = null;
        chestStorage = null;
    }

    public void SetChestUIEnabled(bool newState)
    {
        gameObject.SetActive(newState);
    }

    public void TransferItem(int pos, StorageComponent origin, StorageComponent destination)
    {
        ItemSlot slot = origin.GetItemByIndex(pos);
        if(slot.item != null && destination.HasSpaceFor(slot.item.Id, slot.amount)) {
            destination.AddItem(slot.item.Id, slot.amount);
            origin.RemoveItemByIndex(pos, slot.amount);
        }

        UpdateItemSprites();
    }


    public void TransferItemFromPlayerToChest(int pos)
    {
        if (player != null && chestStorage != null)
            TransferItem(pos, player.inventory, chestStorage);
    }

    public void TransferItemFromChestToPlayer(int pos)
    {
        if (player != null && chestStorage != null)
            TransferItem(pos, chestStorage, player.inventory);
    }


    private void UpdateItemSprites()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            ItemSlot itemSlot = chestStorage.GetItemByIndex(i);
            if (itemSlot.item != null)
            {
                inventorySlots[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = itemSlot.item.Sprite;
                if (inventorySlots[i].transform.GetChild(1).name == "ItemNumber")
                    inventorySlots[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = itemSlot.amount.ToString();
                inventorySlots[i].transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                inventorySlots[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = null;
                if (inventorySlots[i].transform.GetChild(1).name == "ItemNumber")
                    inventorySlots[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "0";
                inventorySlots[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    void Start()
    {
        foreach (Transform g in transform.GetChild(0).GetComponentsInChildren<Transform>())
        {
            if (g.name.StartsWith("Slot"))
            {
                inventorySlots.Add(g.gameObject);
            }
        }

        SetChestUIEnabled(false);
    }
}
