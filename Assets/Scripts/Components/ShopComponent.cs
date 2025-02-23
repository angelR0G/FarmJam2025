using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopComponent : MonoBehaviour
{
    private static Dictionary<ItemId, int> shopItems = new Dictionary<ItemId, int> {
        {ItemId.WheatSeed, 1 },
        {ItemId.PotatoSeed, 5 },
        {ItemId.TomatoSeed, 5 },
        {ItemId.BeanSeed, 5 },
        {ItemId.PumpkinSeed, 5 },
    };

    private static Dictionary<ItemId, int> sellItemsValue = new Dictionary<ItemId, int>
    {
        {ItemId.Wheat, 1},
        {ItemId.Potato, 1},
        {ItemId.Tomato, 1},
        {ItemId.Bean, 1},
        {ItemId.Pumpkin, 1},
    };

    public void BuyItem(ItemId itemId, int quantity, InventoryComponent inventoryToBeSaved)
    {
        if (!shopItems.ContainsKey(itemId)) return;

        int totalPrice = shopItems[itemId] * quantity;

        if (GameManager.Instance.currentMoney < totalPrice || !inventoryToBeSaved.HasSpaceFor(itemId, quantity)) return;

        inventoryToBeSaved.AddItem(itemId, quantity);
        GameManager.Instance.currentMoney -= totalPrice;
    }

    public void SellItem(ItemId itemId, int quantity, InventoryComponent fromInventory)
    {
        if (!sellItemsValue.ContainsKey(itemId)) return;

        GameManager.Instance.currentMoney += sellItemsValue[itemId];

        fromInventory.RemoveItemById(itemId, quantity);
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
