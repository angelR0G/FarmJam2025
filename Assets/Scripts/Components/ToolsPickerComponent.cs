using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToolsPickerComponent : MonoBehaviour
{
    public UnityEvent onToolsPicked;

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
        player.inventory.AddItem(ItemId.EvilLettuceSeed, 3);
        player.inventory.AddItem(ItemId.EvilSpikeSeed, 3);

        DialogueSystem.Instance.DisplayDialogue(new Dialogue("Alright, it's time to get started.", 1, true, 3f));
        DialogueSystem.Instance.QueueDialogue(new Dialogue("There should be some seeds in the chest outside. I'll use those to begin.", 1, true, 5f));
        DialogueSystem.Instance.QueueDialogue(new Dialogue("I could check the barn, but I can't do anything there now.", 1, true, 4f));
        
        onToolsPicked.Invoke();

        Destroy(gameObject);
    }
}
