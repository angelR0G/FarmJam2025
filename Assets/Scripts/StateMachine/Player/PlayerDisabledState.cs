using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisabledState : PlayerState
{
    public override void EnterState()
    {
        player.animator.SetTrigger("StopMoving");
        player.IsInteractionEnabled = false;
    }

    public override void ExitState()
    {
        player.IsInteractionEnabled = true;
    }
}
