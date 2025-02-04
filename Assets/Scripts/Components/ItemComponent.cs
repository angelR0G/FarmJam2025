using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemComponent : MonoBehaviour
{
    [SerializeField]
    private ItemType type = ItemType.Default;
    [SerializeField]
    private ItemId id = ItemId.Default;
    [SerializeField]
    private int maxStack = 1;
    [SerializeField]
    private String itemName = "Item";

    public ItemType Type { get { return type; }  private set { type = value; } }
    public ItemId Id { get { return id; }  private set { id = value; } }
    public int MaxStack { get { return maxStack; }  private set { maxStack = value; } }
    public String ItemName { get { return itemName; }  private set { itemName = value; } }
}

public class ItemSlot
{
    public ItemComponent item { get; private set; } = null;
    public int amount { get; private set; } = 0;

    public void SetAmount(int newAmount)
    {
        if (item == null) return;

        amount = Math.Clamp(newAmount, 0, item.MaxStack);
    }

    public void AddAmount(int increment)
    {
        if (item == null) return;

        amount = Math.Clamp(amount + increment, 0, item.MaxStack);
    }

    public void SetItem(ItemComponent newItem)
    {
        item = newItem;
        amount = 0;
    }

    public bool IsFull()
    {
        return amount == item.MaxStack;
    }
}

public enum ItemType
{
    Default,
    Seed,
    Crop,
    EvilCrop,
    Tool
}

public enum ItemId
{
    Default,
    CornSeed,
    Dagger
}