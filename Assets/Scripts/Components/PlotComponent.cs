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
    public bool evilCropsAllowed = false;

    // Start is called before the first frame update
    void Start()
    {
        trigger = GetComponent<InteractionTriggerComponent>();
        sprite = GetComponent<SpriteRenderer>();
        plantedCrop = null;

        trigger.interactionCallback = Interact;
        GameManager.Instance.nightStart += DisappearAtDawn;
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
                DialogueSystem.Instance.DisplayDialogue(new Dialogue("What a beautiful crop!", -1));
            }
        }
        else 
        {
            if ((equipedItemType == ItemType.Seed && !evilCropsAllowed) || (equipedItemType == ItemType.EvilCrop && evilCropsAllowed))
            {
                PlantCrop(equipedItem as SeedItemComponent);
                player.inventory.RemoveEquipedItem();

                // Remove callbacks that automatically destroy plot
                GameManager.Instance.nightEnd -= DestroyPlot;
                GameManager.Instance.nightStart -= DisappearAtDawn;
            }
            else if (equipedItemType == ItemType.Seed || equipedItemType == ItemType.EvilCrop)
            {
                DialogueSystem.Instance.DisplayDialogue(new Dialogue("This soil is not suitable for this plant."));
            }
            else
            {
                DialogueSystem.Instance.DisplayDialogue(new Dialogue("I can plant a seed here."));
            }
        }
    }

    public void PlantCrop(SeedItemComponent seed)
    {
        plantedCrop = Instantiate(seed.cropPrefab, transform);

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
        if (plantedCrop.CompareTag("EvilCrop"))
        {
            // Evil crops become carriable objects
            CropComponent cropComp = plantedCrop.GetComponent<CropComponent>();
            cropComp.body.bodyType = RigidbodyType2D.Dynamic;
            cropComp.cropCollider.enabled = true;

            plantedCrop.AddComponent<InteractionTriggerComponent>();
            plantedCrop.AddComponent<CarriableComponent>();

            plantedCrop.transform.SetParent(transform.parent, true);

            Destroy(gameObject);
        }
        else
        {
            // Normal plants are added to player's inventory
            bool plantAdded = player.inventory.AddItem(plantedCrop.GetComponent<CropComponent>().collectableCrop) > 0;

            if (plantAdded)
                Destroy(gameObject);
        }
    }

    private void DisappearAtDawn(object sender, int hour)
    {
        GameManager.Instance.nightEnd += DestroyPlot;
        GameManager.Instance.nightStart -= DisappearAtDawn;
    }

    private void DestroyPlot(object sender, int hour)
    {
        GameManager.Instance.nightEnd -= DestroyPlot;
        Destroy(gameObject);
    }
}
