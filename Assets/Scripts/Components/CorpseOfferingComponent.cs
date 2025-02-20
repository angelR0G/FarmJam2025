using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseOfferingComponent : MonoBehaviour
{
    private GameObject corpseOffered = null;
    public CropOfferingComponent cropOfferingAltar;

    private static readonly Dictionary<CorpseCreature, KeyValuePair<ItemId, int>>  offerings = new Dictionary<CorpseCreature, KeyValuePair<ItemId, int>> {
        {CorpseCreature.Pig, new KeyValuePair<ItemId, int>(ItemId.Carrot, 5) },
        {CorpseCreature.Stalker, new KeyValuePair<ItemId, int>(ItemId.Potato, 5)},
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
        if (collision.gameObject == corpseOffered)
            corpseOffered = null;
    }

    private void OfferNewCorpse(CorpseComponent newCorpseOffered)
    {
        if (newCorpseOffered.gameObject == corpseOffered) return;

        DestroyCorpse();

        corpseOffered = newCorpseOffered.gameObject;
        KeyValuePair<ItemId, int> newCropOffering = offerings[newCorpseOffered.creature];
        cropOfferingAltar.RequestNewOffering(newCropOffering.Key, newCropOffering.Value);
    }

    private void DestroyCorpse()
    {
        if (corpseOffered == null)
            return;

        Destroy(corpseOffered);
    }
}
