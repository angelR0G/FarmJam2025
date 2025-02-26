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
            DialogueSystem.Instance.DisplayDialogue(new Dialogue("To perform the ritual, I have to make three offerings: meat, vegetables, and blood."));
        }
        else if (player.inventory.GetEquipedItem().Id != ItemId.Dagger)
        {
            DialogueSystem.Instance.DisplayDialogue(new Dialogue("The offerings are ready. Now, all that's left is my blood."));
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
