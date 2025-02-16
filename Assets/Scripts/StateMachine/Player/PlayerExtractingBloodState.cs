using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExtractingBloodState : PlayerState
{
    private BloodContainer corpseBloodContainer;

    public override void EnterState() 
    {
        corpseBloodContainer = GetBloodContainereAtTheirPosition();

        if (corpseBloodContainer)
        {
            player.IsInteractionEnabled = false;
            player.animator.SetTrigger("Dig");
            player.onAnimFinished = OnAnimationFinished;
        }
        else {
            player.ChangeState(player.walkingState);
            Debug.Log("~~ No hay sangre que extraer ~~");
        }
    }

    public void OnAnimationFinished()
    {
        // Remove callback
        player.onAnimFinished = null;
        player.wateringState.AddBlood(corpseBloodContainer.blood);
        
        Destroy(corpseBloodContainer.gameObject);

        player.ChangeState(player.walkingState);
    }

    private BloodContainer GetBloodContainereAtTheirPosition()
    {
        BloodContainer bloodContainer;
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        foreach (Collider2D c in collisions)
        {
            // Does not take into account the player
            if (c.TryGetComponent<BloodContainer>(out bloodContainer))
            {
                return bloodContainer;
            }
        }

        return null;
    }
}
