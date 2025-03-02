using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : StorageComponent

{
    [SerializeField] GameObject chestUIObject;
    // Start is called before the first frame update
    void Start()
    {
        transform.GetComponent<InteractionTriggerComponent>().interactionCallback = OpenChestUI;
    }


    public void OpenChestUI(PlayerComponent player)
    {
        chestUIObject.GetComponent<ChestDisplay>().OpenChest(player, gameObject.GetComponent<Chest>());
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
