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
        foreach (GameObject g in inventorySlots)
        {
            //g.GetComponent<Image>().sprite = playerInventory.GetEquipedItem().Sprite;
        }
    }

    private void UpdateSlotMark(int index)
    {
        marker.transform.SetParent(inventorySlots[index].transform);
        marker.rectTransform.position = Vector3.zero;
        marker.transform.position = Vector3.zero;
        marker.rectTransform.anchoredPosition3D = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSlotMark(playerInventory.GetActiveIndex());
        UpdateItemSprites();
    }
}
