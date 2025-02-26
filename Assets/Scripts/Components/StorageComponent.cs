using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageComponent : MonoBehaviour
{
    public int storageSize = 10;

    protected List<ItemSlot> items;

    public void Awake()
    {
        items = new List<ItemSlot>(storageSize);

        for (int i = 0; i < storageSize; i++)
        {
            items.Add(new ItemSlot());
        }
    }

    public ItemSlot GetItemByIndex(int index)
    {
        // Check if it is a valid index
        if (index < 0 || index >= storageSize) return null;

        // Get the item from the correct array
        return items[index];
    }

    public virtual int AddItem(ItemId itemId, int amount = 1)
    {
        int itemIndex = 0;
        int amountSaved = 0;

        // First check slots with that objects that are not full yet
        while (amountSaved < amount)
        {
            itemIndex = FindIncompleteItemSlotIndex(itemId, itemIndex);
            if (itemIndex == -1) break;

            amountSaved += AddItemToSlot(itemId, amount - amountSaved, itemIndex);

            itemIndex++;
        }

        // If there is not enough space, check if there are empty slots
        itemIndex = 0;
        while (amountSaved < amount)
        {
            itemIndex = FindFreeSlotIndex(itemIndex);
            if (itemIndex == -1) break;

            amountSaved += AddItemToSlot(itemId, amount - amountSaved, itemIndex);

            itemIndex++;
        }

        return amountSaved;
    }

    protected int AddItemToSlot(ItemId itemId, int amount, int slotIndex)
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

    public virtual bool HasSpaceFor(ItemId id, int amount = 1)
    {
        int freeSpaceCount = 0;
        int slotIndex = 0;

        // First check slots with that objects that are not full yet
        while (freeSpaceCount < amount)
        {
            slotIndex = FindIncompleteItemSlotIndex(id, slotIndex);
            if (slotIndex == -1) break;

            freeSpaceCount += items[slotIndex].item.MaxStack - items[slotIndex].amount;

            slotIndex++;
        }

        // If there is not enough space, check if there are empty slots
        slotIndex = 0;
        while (freeSpaceCount < amount)
        {
            slotIndex = FindFreeSlotIndex(slotIndex);
            if (slotIndex == -1) break;

            freeSpaceCount += ItemFactory.GetItemData(id).maxStack;

            slotIndex++;
        }

        return freeSpaceCount >= amount;
    }

    public void RemoveItemByIndex(int index, int quantity)
    {
        if (index < 0 || index > items.Count || quantity <= 0) return;

        if (items[index].amount <= quantity)
        {
            Destroy(items[index].item);
            items[index].SetItem(null);
        }
        else
            items[index].AddAmount(-quantity);
    }

    public void RemoveItemById(ItemId id, int quantity = 1)
    {
        int searchIndex = 0;

        while (quantity > 0 && searchIndex < items.Count)
        {
            int slotIndex = FindSlotIndexById(id, searchIndex);

            if (slotIndex == -1) break;

            ItemSlot itemSlot = items[slotIndex];
            int amountToReduce = Math.Min(itemSlot.amount, quantity);

            if (itemSlot.amount <= amountToReduce)
            {
                Destroy(itemSlot.item);
                itemSlot.SetItem(null);
            }
            else
            {
                itemSlot.AddAmount(-amountToReduce);
            }

            quantity -= amountToReduce;
            searchIndex = slotIndex + 1;
        }
    }

    protected int FindIncompleteItemSlotIndex(ItemId itemId, int start = 0)
    {
        for (int i = start; i < items.Count; i++)
        {
            if (items[i].item != null && items[i].item.Id == itemId && !items[i].IsFull()) return i;
        }

        return -1;
    }

    protected int FindSlotIndexById(ItemId itemId, int start = 0)
    {
        for (int i = start; i < items.Count; i++)
        {
            if (items[i].item != null && items[i].item.Id == itemId) return i;
        }

        return -1;
    }

    protected int FindFreeSlotIndex(int start = 0)
    {
        for (int i = start; i < items.Count; i++)
        {
            if (items[i].item == null) return i;
        }

        return -1;
    }
}
