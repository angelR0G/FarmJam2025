using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class InventoryComponent : MonoBehaviour
{
    const ushort ITEMS_SLOTS = 5;
    const ushort TOOLS_SLOTS = 5;
    const ushort INVENTORY_SIZE = ITEMS_SLOTS + TOOLS_SLOTS;

    private int activeItemIndex = 0;

    private List<ItemSlot> items = new List<ItemSlot>(INVENTORY_SIZE);
    //private ItemSlot[] items = new ItemSlot[ITEMS_SLOTS];
    private ItemSlot[] tools = new ItemSlot[TOOLS_SLOTS];


    public ItemComponent GetActiveItem()
    {
        // Check if it is a valid index
        if (activeItemIndex < 0 || activeItemIndex >= INVENTORY_SIZE) return null;

        // Get the item from the correct array
        return items[activeItemIndex].item;
    }

    public bool AddItem(ItemComponent item)
    {
        int itemIndex = -1;

        if ((itemIndex = FindIncompleteItemSlotIndex(item)) != -1)
        {
            items[itemIndex].AddAmount(1);
        }
        else if ((itemIndex = FindFreeSlotIndex()) != -1)
        {
            items[itemIndex].SetItem(item);
            items[itemIndex].SetAmount(1);
        }
        else
        {
            return false;
        }

        return true;
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
                activeItemIndex = nextIndex;
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
                activeItemIndex = previousIndex;
                break;
            }
        } while (previousIndex != activeItemIndex);
    }

    public void RemoveItemByIndex(int index)
    {
        if (index < 0 || index > items.Count) return;

        if (items[index].amount == 1) 
            items[index].SetItem(null);
        else 
            items[index].AddAmount(-1);
    }


    private int FindIncompleteItemSlotIndex(ItemComponent searchItem)
    {
        for (int i = TOOLS_SLOTS; i < items.Count; i++)
        {
            if (items[i].item == searchItem && !items[i].IsFull()) return i;
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
