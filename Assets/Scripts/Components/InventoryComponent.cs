using System.Collections.Generic;
using UnityEngine;

public class InventoryComponent : StorageComponent
{
    public readonly int TOOLS_SLOTS = 5;

    private int activeItemIndex = 0;
    public bool blockInventory = false;

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

    public int GetActiveIndex()
    {
        return activeItemIndex;
    }

    public List<ItemSlot> GetAllItems()
    {
        return items;
    }

    public ItemComponent GetEquipedItem()
    {
        // Check if it is a valid index
        if (activeItemIndex < 0 || activeItemIndex >= storageSize) return null;

        // Get the item from the correct array
        return items[activeItemIndex].item;
    }

    public int GetEquipedItemQuantity()
    {
        if (activeItemIndex < 0 || activeItemIndex >= storageSize) return 0;

        return items[activeItemIndex].amount;
    }

    public int GetItemQuantity(ItemId id)
    {
        int quantity=0;
        foreach(ItemSlot slot in items)
        {
            if(slot.item != null && slot.item.Id == id)
            {
                quantity += slot.amount;
            }
        }
        return quantity;
    }

    public void AddTool(ItemId toolId)
    {
        for (int i = 0; i < TOOLS_SLOTS; i++)
        {
            if (items[i].item == null)
            {
                items[i].SetItem(ItemFactory.CreateItem(toolId, gameObject));
                return;
            }
        }
    }

    public override int AddItem(ItemId itemId, int amount = 1)
    {
        int itemIndex = TOOLS_SLOTS;
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
        itemIndex = TOOLS_SLOTS;
        while (amountSaved < amount)
        {
            itemIndex = FindFreeSlotIndex(itemIndex);
            if (itemIndex == -1) break;

            amountSaved += AddItemToSlot(itemId, amount - amountSaved, itemIndex);

            itemIndex++;
        }

        return amountSaved;
    }

    public override bool HasSpaceFor(ItemId id, int amount = 1)
    {
        int freeSpaceCount = 0;
        int slotIndex = TOOLS_SLOTS;

        // First check slots with that objects that are not full yet
        while (freeSpaceCount < amount)
        {
            slotIndex = FindIncompleteItemSlotIndex(id, slotIndex);
            if (slotIndex == -1) break;

            freeSpaceCount += items[slotIndex].item.MaxStack - items[slotIndex].amount;

            slotIndex++;
        }

        // If there is not enough space, check if there are empty slots
        slotIndex = TOOLS_SLOTS;
        while (freeSpaceCount < amount)
        {
            slotIndex = FindFreeSlotIndex(slotIndex);
            if (slotIndex == -1) break;

            freeSpaceCount += ItemFactory.GetItemData(id).maxStack;

            slotIndex++;
        }

        return freeSpaceCount >= amount;
    }

    public void EquipItem(int index)
    {
        if (blockInventory || index < 0 || index >= storageSize) return;

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
            if (nextIndex >= storageSize) nextIndex = 0;

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
            if (previousIndex < 0) previousIndex = storageSize - 1;

            if (items[previousIndex].item != null)
            {
                EquipItem(previousIndex);
                break;
            }
        } while (previousIndex != activeItemIndex);
    }

    public void RemoveEquipedItem(int quantity = 1)
    {
        RemoveItemByIndex(activeItemIndex, quantity);
    }
}
