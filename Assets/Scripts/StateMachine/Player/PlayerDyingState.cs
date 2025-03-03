using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDyingState : PlayerState
{
    public override void EnterState()
    {
        player.animator.SetTrigger("Die");
        player.IsInteractionEnabled = false;

        UIEffects.Instance.Invoke("PerformFadeIn", 3f);
    }
}
