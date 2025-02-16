using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemFactory : MonoBehaviour
{
    private static ItemFactory Instance;
    public Dictionary<ItemId, ItemData> itemsData;
    public GameObject itemPrefab;
    public GameObject corpsePrefab;

    public static GameObject CreatePickableItem(ItemId id, Transform parent = null, int amount = 1)
    {
        if (Instance.itemPrefab == null || !Instance.itemsData.ContainsKey(id)) return null;

        // Creates an object and fill its values
        GameObject newItem = Instantiate(Instance.itemPrefab, parent);
        PickableComponent pickComp = newItem.GetComponent<PickableComponent>();
        pickComp.id = id;
        pickComp.amount = amount;

        return newItem;
    }

    public static GameObject CreateCorpse()
    {
        if (Instance.corpsePrefab == null) return null;

        return Instantiate(Instance.corpsePrefab); ;
    }

    public static ItemComponent CreateItem(ItemId id, GameObject owner)
    {
        if (owner == null || !Instance.itemsData.ContainsKey(id)) return null;

        ItemData data = Instance.itemsData[id];
        ItemComponent newItem;

        // Creates the specific component depending on the item data
        if (data.type == ItemType.Seed)
            newItem = owner.AddComponent<SeedItemComponent>();
        else
            newItem = owner.AddComponent<ItemComponent>();

        // Fills the value of the ItemComponent
        newItem.CopyValues(Instance.itemsData[id]);

        return newItem;
    }

    public static ItemData GetItemData(ItemId id)
    {
        return Instance.itemsData.GetValueOrDefault(id, null);
    }

    private void Awake()
    {
        Instance = this;
        itemsData = new Dictionary<ItemId, ItemData>();
        
        ItemData[] items = Resources.LoadAll<ItemData>("Items/");
        foreach (ItemData item in items)
        {
            itemsData.Add(item.id, item);
        }

        #if UNITY_EDITOR
        foreach (KeyValuePair<ItemId, ItemData> v in itemsData)
        {
            Debug.Log(v.Key + " -> " + v.Value.ToString());
        }
        #endif
    }
}
