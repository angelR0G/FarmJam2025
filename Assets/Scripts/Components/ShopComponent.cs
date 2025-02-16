using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopComponent : MonoBehaviour
{
    private static Dictionary<ItemId, int> shopItems = new Dictionary<ItemId, int> {
        {ItemId.Corn, 1 },
        {ItemId.WaterCan, 5 }
    };

    private static Dictionary<ItemId, int> sellItemsValue = new Dictionary<ItemId, int>
    {
        {ItemId.Corn, 1},
    };

    public void BuyItem(ItemId itemId, int quantity, InventoryComponent inventoryToBeSaved)
    {
        if (!shopItems.ContainsKey(itemId)) return;

        int totalPrice = shopItems[itemId] * quantity;

        if (GameManager.Instance.currentMoney < totalPrice || !inventoryToBeSaved.HasSpaceFor(itemId, quantity)) return;

        inventoryToBeSaved.AddItem(itemId, quantity);
        GameManager.Instance.currentMoney -= totalPrice;
    }

    public void SellItem(ItemId itemId)
    {
        if (!sellItemsValue.ContainsKey(itemId)) return;

        GameManager.Instance.currentMoney += sellItemsValue[itemId];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
