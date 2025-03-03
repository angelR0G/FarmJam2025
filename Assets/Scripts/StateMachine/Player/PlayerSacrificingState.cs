using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSacrificingState : PlayerState
{
    public AltarComponent altar;
    public AudioClip knifeCutSound;

    public override void EnterState()
    {
        if (altar && altar.CanStartRitual(player))
        {
            player.IsInteractionEnabled = false;
            player.animator.SetTrigger("Sacrifice");
            player.onAnimFinished = FinishRitual;

            player.audioSource.clip = knifeCutSound;
        }
        else
        {
            player.ChangeState(player.walkingState);
        }
    }

    public override void ExitState()
    {
        player.onAnimFinished = null;
        player.IsInteractionEnabled = true;
    }

    private void FinishRitual()
    {
        altar.RitualPerformed();
        player.ChangeState(player.walkingState);
    }
}
