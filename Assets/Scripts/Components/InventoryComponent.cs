using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Progress;

public class InventoryComponent : MonoBehaviour
{
    const ushort ITEMS_SLOTS = 5;
    const ushort TOOLS_SLOTS = 5;
    const ushort INVENTORY_SIZE = ITEMS_SLOTS + TOOLS_SLOTS;

    private int activeItemIndex = 0;

    private List<ItemSlot> items = new List<ItemSlot>(INVENTORY_SIZE);

    public void BindInput(InputComponent input)
    {
        input.equipTool1Event.AddListener(() => EquipItem(0));
        input.equipTool2Event.AddListener(() => EquipItem(1));
        input.equipTool3Event.AddListener(() => EquipItem(2));
        input.equipWeaponEvent.AddListener(() => EquipItem(3));
        input.equipTorchEvent.AddListener(() => EquipItem(4));
        input.equipItem1Event.AddListener(() => EquipItem(5));
        input.equipItem2Event.AddListener(() => EquipItem(6));
        input.equipItem3Event.AddListener(() => EquipItem(7));
        input.equipItem4Event.AddListener(() => EquipItem(8));
        input.equipItem5Event.AddListener(() => EquipItem(9));
        input.equipNextItemEvent.AddListener(EquipNextItem);
        input.equipPreviousItemEvent.AddListener(EquipPreviousItem);
    }

    public void Start()
    {
        for (int i = 0; i < INVENTORY_SIZE; i++)
        {
            items.Add(new ItemSlot());
        }
    }

    public ItemComponent GetEquipedItem()
    {
        // Check if it is a valid index
        if (activeItemIndex < 0 || activeItemIndex >= INVENTORY_SIZE) return null;

        // Get the item from the correct array
        return items[activeItemIndex].item;
    }

    public int AddItem(ItemId itemId, int amount = 1)
    {
        int itemIndex = TOOLS_SLOTS;
        int amountSaved = 0;

        // First check slots with that objects that are not full yet
        while (amountSaved < amount)
        {
            itemIndex = FindIncompleteItemSlotIndex(itemId);
            if (itemIndex == -1) break;

            amountSaved += AddItemToSlot(itemId, amount - amountSaved, itemIndex);

            itemIndex++;
        }

        // If there is not enough space, check if there are empty slots
        itemIndex = TOOLS_SLOTS;
        while (amountSaved < amount)
        {
            itemIndex = FindFreeSlotIndex();
            if (itemIndex == -1) break;

            amountSaved += AddItemToSlot(itemId, amount - amountSaved, itemIndex);

            itemIndex++;
        }

        return amountSaved;
    }

    private int AddItemToSlot(ItemId itemId, int amount, int slotIndex)
    {
        if (items[slotIndex].item == null)
        {
            // The slot is empty, so creates a new item component to save the data
            ItemComponent newItem = ItemFactory.CreateItem(itemId, gameObject);

            items[slotIndex].SetItem(newItem);
            items[slotIndex].SetAmount(Math.Min(amount, newItem.MaxStack));

            return items[slotIndex].amount;
        }
        else if (items[slotIndex].item.Id == itemId)
        {
            // The slot has that item saved, so updates the number of items in the slot
            int freeSpace = items[slotIndex].item.MaxStack - items[slotIndex].amount;
            int amountToAdd = Math.Min(amount, freeSpace);

            items[slotIndex].AddAmount(amountToAdd);
            return amountToAdd;
        }

        return 0;
    }

    public bool HasSpaceFor(ItemId id, int amount = 1)
    {
        int freeSpaceCount = 0;
        int slotIndex = TOOLS_SLOTS;

        // First check slots with that objects that are not full yet
        while (freeSpaceCount < amount)
        {
            slotIndex = FindIncompleteItemSlotIndex(id);
            if (slotIndex == -1) break;

            freeSpaceCount += items[slotIndex].item.MaxStack - items[slotIndex].amount;

            slotIndex++;
        }

        // If there is not enough space, check if there are empty slots
        slotIndex = TOOLS_SLOTS;
        while (freeSpaceCount < amount)
        {
            slotIndex = FindFreeSlotIndex();
            if (slotIndex == -1) break;

            freeSpaceCount += ItemFactory.GetItemData(id).maxStack;

            slotIndex++;
        }

        return freeSpaceCount >= amount;
    }

    public void EquipItem(int index)
    {
        if (index < 0 || index >= INVENTORY_SIZE) return;

        activeItemIndex = index;

        #if UNITY_EDITOR
        if (GetEquipedItem() != null)
            Debug.Log("Objeto con id (" + GetEquipedItem().Id + ") equipado.");
        else
            Debug.Log("Equipada ranura vacía");
        #endif
    }

    public void EquipNextItem()
    {
        int nextIndex = activeItemIndex;
        do
        {
            nextIndex++;
            if (nextIndex >= INVENTORY_SIZE) nextIndex = 0;

            if (items[nextIndex].item != null)
            {
                EquipItem(nextIndex);
                break;
            }
        } while (nextIndex != activeItemIndex);
    }

    public void EquipPreviousItem()
    {
        int previousIndex = activeItemIndex;
        do
        {
            previousIndex--;
            if (previousIndex < 0) previousIndex = INVENTORY_SIZE - 1;

            if (items[previousIndex].item != null)
            {
                EquipItem(previousIndex);
                break;
            }
        } while (previousIndex != activeItemIndex);
    }

    public void RemoveItemByIndex(int index)
    {
        if (index < 0 || index > items.Count) return;

        if (items[index].amount == 1)
        {
            Destroy(items[index].item);
            items[index].SetItem(null);
        }
        else
            items[index].AddAmount(-1);
    }

    public void RemoveEquipedItem()
    {
        RemoveItemByIndex(activeItemIndex);
    }


    private int FindIncompleteItemSlotIndex(ItemId itemId, int start = TOOLS_SLOTS)
    {
        start = Mathf.Max(start, TOOLS_SLOTS);

        for (int i = start; i < items.Count; i++)
        {
            if (items[i].item != null && items[i].item.Id == itemId && !items[i].IsFull()) return i;
        }

        return -1;
    }

    private int FindFreeSlotIndex()
    {
        for (int i = TOOLS_SLOTS; i < items.Count; i++)
        {
            if (items[i].item == null) return i;
        }

        return -1;
    }
}
