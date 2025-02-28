using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private InventoryComponent playerInventory;
    private List<GameObject> inventorySlots = new List<GameObject>();
    [SerializeField] private Image marker;
    [SerializeField] private Image liquidBar;
    [SerializeField] private Color waterLiquid;
    [SerializeField] private Color bloodLiguid;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform g in transform.GetChild(2).GetComponentsInChildren<Transform>())
        {
            if (g.name.StartsWith("Slot")) {
                inventorySlots.Add(g.gameObject);
            }
            
        }
    }

    private void UpdateItemSprites()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            ItemSlot itemSlot = playerInventory.GetItemByIndex(i); 
            if(itemSlot.item != null)
            {
                inventorySlots[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = itemSlot.item.Sprite;
                if(inventorySlots[i].transform.GetChild(1).name=="ItemNumber")
                    inventorySlots[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = itemSlot.amount.ToString();
                inventorySlots[i].transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                inventorySlots[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = null;
                if (inventorySlots[i].transform.GetChild(1).name == "ItemNumber")
                    inventorySlots[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "0";
                inventorySlots[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    private void UpdateSlotMark(int index)
    {
        if (index < 0)
        {
            marker.enabled = false;
        }
        else
        {
            marker.transform.SetParent(inventorySlots[index].transform);
            marker.rectTransform.position = Vector3.zero;
            marker.transform.position = Vector3.zero;
            marker.rectTransform.anchoredPosition3D = Vector3.zero;
            marker.enabled = true;
        }
    }

    private void UpdateLiquidBar()
    {
        PlayerWateringState watering = player.GetComponent<PlayerComponent>().wateringState;

        liquidBar.color = (watering.isBlood) ? bloodLiguid : waterLiquid;
        liquidBar.fillAmount = watering.getFillPorcentage();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSlotMark(playerInventory.GetActiveIndex());
        UpdateItemSprites();
        UpdateLiquidBar();
    }
}
