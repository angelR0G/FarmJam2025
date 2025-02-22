using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{

    [SerializeField] private InventoryComponent playerInventory;
    private List<GameObject> inventorySlots = new List<GameObject>();
    [SerializeField] private Image marker;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform g in transform.GetChild(0).GetComponentsInChildren<Transform>())
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
            ItemComponent item = playerInventory.GetItemByIndex(i); 
            if(item != null)
            {
                inventorySlots[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = item.Sprite;
                inventorySlots[i].transform.GetChild(0).gameObject.SetActive(true);

            }
            else
            {
                inventorySlots[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = null;
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

    // Update is called once per frame
    void Update()
    {
        UpdateSlotMark(playerInventory.GetActiveIndex());
        UpdateItemSprites();
    }
}
