using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbusherDyingState : AmbusherState
{
    public override void EnterState()
    {
        enemy.animator.SetTrigger("Die");
    }
}
