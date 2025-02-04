using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    private static ItemFactory Instance;
    public Dictionary<ItemId, GameObject> itemPrefabs;

    public static GameObject CreateItem(ItemId id, Transform parent = null)
    {
        GameObject prefab = Instance.itemPrefabs.GetValueOrDefault(id, null);
        if (prefab == null) return null;

        GameObject newItem = Instantiate(prefab, parent);

        return newItem;
    }

    private void Start()
    {
        Instance = this;

        GameObject[] items = Resources.LoadAll<GameObject>("Items/");
        ItemComponent comp = null;

        itemPrefabs = new Dictionary<ItemId, GameObject>();
        foreach (GameObject item in items)
        {
            if (item.TryGetComponent<ItemComponent>(out comp))
            {
                itemPrefabs.Add(comp.Id, item);
            }
        }

        #if UNITY_EDITOR
        foreach (KeyValuePair<ItemId, GameObject> v in itemPrefabs)
        {
            Debug.Log(v.Key + " -> " + v.Value.ToString());
        }
        #endif
    }
}
