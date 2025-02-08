using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropGrownState : CropState
{
    public override void EnterState()
    {
        crop.sprite.sprite = crop.cropSprites[crop.cropSprites.Count - 1];
    }
}
