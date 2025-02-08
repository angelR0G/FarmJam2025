using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SeedItemComponent : ItemComponent
{
    public GameObject cropPrefab;

    public override void CopyValues(ItemData data)
    {
        base.CopyValues(data);

        if (data is SeedItemData)
        {
            cropPrefab = (data as SeedItemData).cropPrefab;
        }
    }
}
