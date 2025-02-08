using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemComponent : MonoBehaviour
{
    [SerializeField]
    protected ItemType type = ItemType.Default;
    [SerializeField]
    protected ItemId id = ItemId.Default;
    [SerializeField]
    protected int maxStack = 1;
    [SerializeField]
    protected String itemName = "Item";
    [SerializeField]
    protected Sprite sprite = null;

    public ItemType Type { get { return type; }  private set { type = value; } }
    public ItemId Id { get { return id; }  private set { id = value; } }
    public int MaxStack { get { return maxStack; }  private set { maxStack = value; } }
    public String ItemName { get { return itemName; }  private set { itemName = value; } }
    public Sprite Sprite { get { return sprite; } private set { sprite = value; } }

    public void CopyValues(ItemComponent other)
    {
        type = other.type;
        id = other.id;
        maxStack = other.maxStack;
        itemName = other.itemName;
        sprite = other.sprite;
    }

    public virtual void CopyValues(ItemData data)
    {
        type = data.type;
        id = data.id;
        maxStack = data.maxStack;
        itemName = data.itemName;
        sprite = data.sprite;
    }
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
    Dagger,
    Hoe,
    WaterCan,
    Corn,
}