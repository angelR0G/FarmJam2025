using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class ShopComponent : MonoBehaviour
{

    [SerializeField] private GameObject itemList;
    [SerializeField] private GameObject objectQuantity;
    [SerializeField] private GameObject objectTotalCost;
    [SerializeField] private GameObject objectTextButtonConfirm;
    [SerializeField] private GameObject objectPlayer;

    private List<GameObject> itemObjects = new List<GameObject>();
    public int currentTab = 0;
    public ItemId currentItem = 0;
    private int quantity = 1;
    private static Dictionary<ItemId, int> shopItems = new Dictionary<ItemId, int> {
        {ItemId.WheatSeed, 1 },
        {ItemId.PotatoSeed, 4 },
        {ItemId.TomatoSeed, 6 },
        {ItemId.BeanSeed, 7 },
        {ItemId.CarrotSeed, 8 },
        {ItemId.PumpkinSeed, 12 },
        {ItemId.Torch, 10 },
        {ItemId.Painkillers, 10 },
        {ItemId.Antipsychotic, 10 },
    };

    private static List<ItemId> shopSeedList = new List<ItemId> {
        ItemId.WheatSeed, ItemId.PotatoSeed, ItemId.TomatoSeed, ItemId.BeanSeed, ItemId.CarrotSeed, ItemId.PumpkinSeed
    };

    private static List<ItemId> shopItemsList = new List<ItemId> {
        ItemId.Torch, ItemId.Painkillers, ItemId.Antipsychotic
    };

    private static Dictionary<ItemId, int> sellItemsValue = new Dictionary<ItemId, int>
    {
        {ItemId.Wheat, 2},
        {ItemId.Potato, 3},
        {ItemId.Tomato, 2},
        {ItemId.Bean, 3},
        {ItemId.Carrot, 6},
        {ItemId.Pumpkin, 30},
    };


    public void BuyItem(ItemId itemId, int quantity, InventoryComponent inventoryToBeSaved)
    {
        if (!shopItems.ContainsKey(itemId)) return;

        int totalPrice = shopItems[itemId] * quantity;

        if (GameManager.Instance.currentMoney < totalPrice || !inventoryToBeSaved.HasSpaceFor(itemId, quantity)) return;

        inventoryToBeSaved.AddItem(itemId, quantity);
        GameManager.Instance.UpdateMoney(-totalPrice);
    }
    public void SellItem(ItemId itemId, int quantity, InventoryComponent fromInventory)
    {
        if (!sellItemsValue.ContainsKey(itemId)) return;

        GameManager.Instance.UpdateMoney(sellItemsValue[itemId] * quantity);

        fromInventory.RemoveItemById(itemId, quantity);

    }


    public void RenderSeedDetails()
    {
        for (int i = 0; i < itemObjects.Count; i++)
        {
            if (i < shopSeedList.Count)
            {
                itemObjects[i].SetActive(true);
                Transform itemImage = itemObjects[i].transform.GetChild(0);
                Transform itemName = itemObjects[i].transform.GetChild(1);
                Transform itemPrice = itemObjects[i].transform.GetChild(2);
                ItemData itemData = ItemFactory.GetItemData(shopSeedList[i]);

                itemImage.GetComponent<Image>().sprite = itemData.sprite;
                itemName.GetComponent<TextMeshProUGUI>().text = itemData.itemName;
                itemPrice.GetComponent<TextMeshProUGUI>().text = shopItems[shopSeedList[i]].ToString();
            }
            else
            {
                itemObjects[i].SetActive(false);
            }
        }
    }

    public void RendershopItemsList()
    {
        for (int i = 0; i < itemObjects.Count; i++)
        {
            if (i < shopItemsList.Count)
            {
                itemObjects[i].SetActive(true);
                Transform itemImage = itemObjects[i].transform.GetChild(0);
                Transform itemName = itemObjects[i].transform.GetChild(1);
                Transform itemPrice = itemObjects[i].transform.GetChild(2);
                ItemData itemData = ItemFactory.GetItemData(shopItemsList[i]);

                itemImage.GetComponent<Image>().sprite = itemData.sprite;
                itemName.GetComponent<TextMeshProUGUI>().text = itemData.itemName;
                itemPrice.GetComponent<TextMeshProUGUI>().text = shopItems[shopItemsList[i]].ToString();
            }
            else
            {
                itemObjects[i].SetActive(false);
            }
        }
    }

    public void RenderSellItems()
    {
        InventoryComponent inventory = objectPlayer.GetComponent<PlayerComponent>().inventory;
        List<ItemSlot> itemsInventory = inventory.GetAllItems();
        for (int i = 0; i < itemObjects.Count; i++)
        {
            if (i < itemsInventory.Count/2 && itemsInventory[i + 5].item!= null)
            {
                itemObjects[i].SetActive(true);
                Transform itemImage = itemObjects[i].transform.GetChild(0);
                Transform itemName = itemObjects[i].transform.GetChild(1);
                Transform itemPrice = itemObjects[i].transform.GetChild(2);
                ItemData itemData = ItemFactory.GetItemData(itemsInventory[i+5].item.Id);

                itemImage.GetComponent<Image>().sprite = itemData.sprite;
                itemName.GetComponent<TextMeshProUGUI>().text = itemData.itemName;
                itemPrice.GetComponent<TextMeshProUGUI>().text = sellItemsValue[itemsInventory[i+5].item.Id].ToString();
            }
            else
            {
                itemObjects[i].SetActive(false);
            }
        }
    }
    public void RenderQuantity()
    {
        objectQuantity.GetComponent<TextMeshProUGUI>().text = quantity.ToString();
        
    }
    public void RenderTotalPrice()
    {
        switch (currentTab)
        {
            case 0:
            case 1:
                shopItems.TryGetValue(currentItem, out int price);
                objectTotalCost.GetComponent<TextMeshProUGUI>().text = (quantity * price).ToString();
                break;
            case 2:
                sellItemsValue.TryGetValue(currentItem, out int price1);
                objectTotalCost.GetComponent<TextMeshProUGUI>().text = (quantity * price1).ToString();
                break;
        }
        
    }
    public void SetCurrentTab(int tab)
    {
        currentTab = tab;
        switch (currentTab)
        {
            case 0:
                RenderSeedDetails();
                objectTextButtonConfirm.GetComponent<TextMeshProUGUI>().text = "Buy";
                break;
            case 1:
                RendershopItemsList();
                objectTextButtonConfirm.GetComponent<TextMeshProUGUI>().text = "Buy";
                break;
            case 2:
                RenderSellItems();
                objectTextButtonConfirm.GetComponent<TextMeshProUGUI>().text = "Sell";
                break;
        }
    }


    public void SetCurrentItem(int pos)
    {
        switch (currentTab)
        {
            case 0:
                currentItem = shopSeedList[pos];
                break;
            case 1:
                currentItem = shopItemsList[pos];
                break;
            case 2:
                InventoryComponent inventory = objectPlayer.GetComponent<PlayerComponent>().inventory;
                currentItem = inventory.GetAllItems()[pos+5].item.Id;
                break;
        }
    }

    public void BuyItemInterface()
    {
        switch (currentTab)
        {
            case 0:
            case 1:
                BuyItem(currentItem, quantity, objectPlayer.GetComponent<PlayerComponent>().inventory);
                break;
            case 2:
                SellItem(currentItem, quantity, objectPlayer.GetComponent<PlayerComponent>().inventory);
                break;
        }
        
    }

    public void addQuantity()
    {
        ItemData itemData = ItemFactory.GetItemData(currentItem);
        switch (currentTab)
        {
            case 0:
            case 1:
                
                if (quantity < itemData.maxStack) quantity++;
                break;
            case 2:
                InventoryComponent inventory = objectPlayer.GetComponent<PlayerComponent>().inventory;
                if (quantity < inventory.GetItemQuantity(currentItem)) quantity++;
                break;
        }
        
        RenderQuantity();
    }

    public void removeQuantity()
    {
        if(quantity>1) quantity--;
        RenderQuantity();
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in itemList.transform)
        {
            itemObjects.Add(child.gameObject);
        }
        RenderSeedDetails();
        RenderQuantity();
        RenderTotalPrice();
    }

    // Update is called once per frame
    void Update()
    {
        RenderTotalPrice();
    }
}
