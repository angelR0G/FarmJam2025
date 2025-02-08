using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    private static ItemFactory Instance;
    public Dictionary<ItemId, ItemData> itemsData;
    public GameObject itemPrefab;

    public static GameObject CreateItem(ItemId id, Transform parent = null, int amount = 1)
    {
        if (Instance.itemPrefab == null || !Instance.itemsData.ContainsKey(id)) return null;

        // Creates an object and fill its values
        GameObject newItem = Instantiate(Instance.itemPrefab, parent);
        PickableComponent pickComp = newItem.GetComponent<PickableComponent>();
        pickComp.id = id;
        pickComp.amount = amount;

        return newItem;
    }

    public static ItemData GetItem(ItemId id)
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
