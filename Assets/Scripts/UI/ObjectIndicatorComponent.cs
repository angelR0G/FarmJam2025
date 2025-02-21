using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectIndicatorComponent : MonoBehaviour
{
    public SpriteRenderer itemSprite;
    public TextMeshPro quantityText;
    public GameObject indicator;

    public void SetVisibility(bool newVisibility)
    {
        indicator.SetActive(newVisibility);
    }

    public void DisplayItem(ItemId itemId, int quantity)
    {
        itemSprite.sprite = ItemFactory.GetItemData(itemId).sprite;
        quantityText.text = quantity.ToString();

        SetVisibility(true);
    }
}
