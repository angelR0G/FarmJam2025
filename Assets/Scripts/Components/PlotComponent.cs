using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotComponent : MonoBehaviour
{
    // Components
    private SpriteRenderer sprite;
    private InteractionTriggerComponent trigger;


    private GameObject crop;
    public Sprite dryTexture;
    public Sprite wateredTexture;

    // Start is called before the first frame update
    void Start()
    {
        trigger = GetComponent<InteractionTriggerComponent>();
        sprite = GetComponent<SpriteRenderer>();
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

        crop.GetComponent<CropComponent>().stateChanged.AddListener(UpdateGroundTexture);
    }

    public bool IsPlanted()
    {
        return crop != null;
    }

    private void UpdateGroundTexture()
    {
        CropComponent cropComp;
        if (crop == null || !crop.TryGetComponent<CropComponent>(out cropComp)) return;

        sprite.sprite = cropComp.currentState == cropComp.wateredState ? wateredTexture : dryTexture;
    }
}
