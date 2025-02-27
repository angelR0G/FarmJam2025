using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsPickerComponent : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<InteractionTriggerComponent>().interactionCallback = OnPickUp;
    }

    private void OnPickUp(PlayerComponent player)
    {
        player.inventory.AddTool(ItemId.WaterCan);
        player.inventory.AddTool(ItemId.Hoe);
        player.inventory.AddTool(ItemId.Dagger);
        player.inventory.AddTool(ItemId.Pitchfork);
        player.inventory.AddItem(ItemId.WheatSeed, 3);
        player.inventory.AddItem(ItemId.TomatoSeed, 3);
        player.inventory.AddItem(ItemId.PumpkinSeed, 3);
        player.inventory.AddItem(ItemId.BeanSeed, 3);
        player.inventory.AddItem(ItemId.PotatoSeed, 3);
        player.inventory.AddItem(ItemId.CarrotSeed, 3);

        Destroy(gameObject);
    }
}
