using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseOfferingComponent : MonoBehaviour
{
    private CorpseComponent corpseOffered = null;
    public CropOfferingComponent cropOfferingAltar;

    private static readonly Dictionary<CorpseCreature, KeyValuePair<ItemId, int>>  offerings = new Dictionary<CorpseCreature, KeyValuePair<ItemId, int>> {
        {CorpseCreature.Pig, new KeyValuePair<ItemId, int>(ItemId.Carrot, 4) },
        {CorpseCreature.Stalker, new KeyValuePair<ItemId, int>(ItemId.Potato, 8)},
        {CorpseCreature.Corrosive, new KeyValuePair<ItemId, int>(ItemId.Wheat, 10)},
        {CorpseCreature.Ambusher, new KeyValuePair<ItemId, int>(ItemId.Bean, 6)},
        {CorpseCreature.Nightmare, new KeyValuePair<ItemId, int>(ItemId.Pumpkin, 2)},
    };

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CorpseComponent corpseEntering;
        if (!collision.TryGetComponent<CorpseComponent>(out corpseEntering))
            return;

        OfferNewCorpse(corpseEntering);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (corpseOffered != null && collision.gameObject == corpseOffered.gameObject)
            RemoveCorpse();
    }

    private void OfferNewCorpse(CorpseComponent newCorpseOffered)
    {
        if (newCorpseOffered == corpseOffered) return;

        DestroyCorpse();

        corpseOffered = newCorpseOffered;
        corpseOffered.onCorpseConsumed.AddListener(RemoveCorpse);

        KeyValuePair<ItemId, int> newCropOffering = offerings[newCorpseOffered.creature];
        cropOfferingAltar.RequestNewOffering(newCropOffering.Key, newCropOffering.Value);
    }

    public CorpseCreature GetOfferedCreature()
    {
        return corpseOffered.creature;
    }

    public bool IsOfferingCompleted()
    {
        return corpseOffered != null;
    }

    public void DestroyCorpse()
    {
        if (corpseOffered == null)
            return;

        corpseOffered.onCorpseConsumed.RemoveListener(RemoveCorpse);
        corpseOffered.ConsumeCorpse();
    }

    private void RemoveCorpse()
    {
        corpseOffered = null;
        cropOfferingAltar.indicator.RemoveSprite();
        cropOfferingAltar.indicator.SetVisibility(false);
    }
}
