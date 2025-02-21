using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarComponent : MonoBehaviour
{
    public CropOfferingComponent cropOfferingAltar;
    public CorpseOfferingComponent corpseOfferingAltar;
    public Vector3 rewardSpawnOffset = Vector3.zero;

    private void Start()
    {
        GetComponent<InteractionTriggerComponent>().interactionCallback = InspectAltar;
    }

    private void InspectAltar(PlayerComponent player)
    {
        Debug.Log("~~ Para realizar el ritual tengo que hacer tres ofrendas: carne, frutos y sangre. ~~");
    }

    public bool CanStartRitual(PlayerComponent player)
    {
        if (!cropOfferingAltar.IsOfferingCompleted())
        {
            Debug.Log("~~ Para realizar el ritual tengo que hacer una ofrenda. ~~");
        }
        else if (player.inventory.GetEquipedItem().Id != ItemId.Dagger)
        {
            Debug.Log("~~ Con mi daga completare el ritual ~~");
        }
        else
        {
            return true;
        }

        return false;
    }

    public void RitualPerformed()
    {
        corpseOfferingAltar.DestroyCorpse();
        cropOfferingAltar.RemoveOffering();

        GameObject ritualReward = ItemFactory.CreatePickableItem(ItemId.Pumpkin);
        ritualReward.transform.position = transform.position + rewardSpawnOffset;
    }
}
