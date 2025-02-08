using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotComponent : MonoBehaviour
{
    // Components
    private SpriteRenderer sprite;
    private InteractionTriggerComponent trigger;


    private GameObject plantedCrop;
    public Sprite dryTexture;
    public Sprite wateredTexture;

    // Start is called before the first frame update
    void Start()
    {
        trigger = GetComponent<InteractionTriggerComponent>();
        sprite = GetComponent<SpriteRenderer>();
        plantedCrop = null;

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
            CropComponent plantedCropComponent = plantedCrop.GetComponent<CropComponent>();

            if (plantedCropComponent.currentState == plantedCropComponent.grownState)
            {
                CollectCrop(player);
            }
            else
            {
                Debug.Log("~~ Que hermosa planta ~~");
            }
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
        plantedCrop = Instantiate(seed.GetComponent<SeedItemComponent>().cropPrefab, transform);

        plantedCrop.GetComponent<CropComponent>().stateChanged.AddListener(UpdateGroundTexture);
    }

    public bool IsPlanted()
    {
        return plantedCrop != null;
    }

    private void UpdateGroundTexture()
    {
        CropComponent cropComp;
        if (plantedCrop == null || !plantedCrop.TryGetComponent<CropComponent>(out cropComp)) return;

        sprite.sprite = cropComp.currentState == cropComp.wateredState ? wateredTexture : dryTexture;
    }

    private void CollectCrop(PlayerComponent player)
    {
        player.inventory.AddItem(plantedCrop.GetComponent<CropComponent>().collectableCrop);
        Destroy(gameObject);
    }
}
