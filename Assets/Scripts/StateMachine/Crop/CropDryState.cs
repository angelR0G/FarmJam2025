using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropDryState : CropState
{
    public override void EnterState()
    {
        crop.cropCollider.enabled = true;
    }

    public override void ExitState()
    {
        crop.cropCollider.enabled = false;
    }
}
