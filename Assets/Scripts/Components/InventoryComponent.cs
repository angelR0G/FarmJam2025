using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Events;

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

    public void EquipItem(int index)
    {
        if (index < 0 || index >= INVENTORY_SIZE) return;

        activeItemIndex = index;
        #if UNITY_EDITOR
        Debug.Log("Objeto con id (" + GetActiveItem().Id + ") equipado.");
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
