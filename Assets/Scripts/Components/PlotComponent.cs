using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotComponent : MonoBehaviour
{
    private InteractionTriggerComponent trigger;
    private GameObject crop;


    // Start is called before the first frame update
    void Start()
    {
        trigger = GetComponent<InteractionTriggerComponent>();
        crop = null;

        trigger.interactionCallback = Interact;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact(PlayerComponent player)
    {
        ItemComponent equipedItem = player.inventory.GetEquipedItem();
        ItemType equipedItemType = equipedItem == null ? ItemType.Default : equipedItem.Type;

        if (IsPlanted())
        {
            Debug.Log("~~ Que hermosa planta ~~");
        }
        else 
        {
            if (equipedItemType == ItemType.Seed)
            {
                PlantCrop(equipedItem);
                player.inventory.RemoveEquipedItem();
            }
            else
            {
                Debug.Log("~~ Aquí podría plantar una semilla ~~");
            }
        }
    }

    public void PlantCrop(ItemComponent seed)
    {
        crop = Instantiate(seed.GetComponent<SeedComponent>().cropPrefab, transform);
        crop.transform.position += new Vector3(0, 0.001f, 0);
    }

    public bool IsPlanted()
    {
        return crop != null;
    }
}
