using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarComponent : MonoBehaviour
{
    private static readonly Dictionary<CorpseCreature, ItemId> ritualRewards = new Dictionary<CorpseCreature, ItemId> {
        {CorpseCreature.Pig, ItemId.EvilSpikeSeed },
        {CorpseCreature.Stalker, ItemId.EvilBulbSeed},
        {CorpseCreature.Corrosive, ItemId.EvilLightSeed},
        {CorpseCreature.Ambusher, ItemId.EvilLettuceSeed},
        {CorpseCreature.Nightmare, ItemId.EvilHeartSeed},
    };

    public CropOfferingComponent cropOfferingAltar;
    public CorpseOfferingComponent corpseOfferingAltar;
    public Vector3 rewardSpawnOffset = Vector3.zero;

    private void Start()
    {
        GetComponent<InteractionTriggerComponent>().interactionCallback = InspectAltar;
    }

    private void InspectAltar(PlayerComponent player)
    {
        CanStartRitual(player);
    }

    public bool CanStartRitual(PlayerComponent player)
    {
        if (!corpseOfferingAltar.IsOfferingCompleted() || !cropOfferingAltar.IsOfferingCompleted())
        {
            Debug.Log("~~ Para realizar el ritual tengo que hacer tres ofrendas: carne, frutos y sangre. ~~");
        }
        else if (player.inventory.GetEquipedItem().Id != ItemId.Dagger)
        {
            Debug.Log("~~ Las ofrendas estan listas, solo falta mi sangre ~~");
        }
        else
        {
            return true;
        }

        return false;
    }

    public void RitualPerformed()
    {
        ItemId rewardItemId = ritualRewards[corpseOfferingAltar.GetOfferedCreature()];
        corpseOfferingAltar.DestroyCorpse();
        cropOfferingAltar.RemoveOffering();

        GameObject ritualReward = ItemFactory.CreatePickableItem(rewardItemId);
        ritualReward.transform.position = transform.position + rewardSpawnOffset;
    }
}
