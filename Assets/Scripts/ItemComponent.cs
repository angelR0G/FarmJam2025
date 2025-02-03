using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemComponent : MonoBehaviour
{
    public ItemType type { get; private set; } = ItemType.Default;
    public int id { get;  set; } = -1;
    public int maxStack { get; private set; } = 1;
}

public class ItemSlot
{
    public ItemComponent item { get; private set; } = null;
    public int amount { get; private set; } = 0;

    public void SetAmount(int newAmount)
    {
        if (item == null) return;

        amount = Math.Clamp(newAmount, 0, item.maxStack);
    }

    public void AddAmount(int increment)
    {
        if (item == null) return;

        amount = Math.Clamp(amount + increment, 0, item.maxStack);
    }

    public void SetItem(ItemComponent newItem)
    {
        item = newItem;
        amount = 0;
    }

    public bool IsFull()
    {
        return amount == item.maxStack;
    }
}

public enum ItemType
{
    Default,
    Seed,
    Crop,
    EvilCorp,
    Tool
}