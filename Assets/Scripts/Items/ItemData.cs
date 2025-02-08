using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
    public ItemType type = ItemType.Default;
    public ItemId id = ItemId.Default;
    public int maxStack = 1;
    public String itemName = "Item";
    public Sprite sprite = null;
}
