using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExtractingBloodState : PlayerState
{
    private BloodContainer corpseBloodContainer;

    public override void EnterState() 
    {
        GameObject interactableObject = GetInteractableObjetAtTheirPosition();

        if (interactableObject != null)
        {
            AltarComponent altar;

            if (interactableObject.TryGetComponent<AltarComponent>(out altar))
            {
                // Change state to perform a ritual
                player.sacrificeState.altar = altar;
                player.ChangeState(player.sacrificeState);
                return;
            }
            else if (interactableObject.TryGetComponent<BloodContainer>(out corpseBloodContainer))
            {
                // Start extracting blood
                player.IsInteractionEnabled = false;
                player.animator.SetTrigger("ExtractBlood");
                player.onAnimFinished = OnAnimationFinished;
                return;
            }
        }

        player.ChangeState(player.walkingState);
        Debug.Log("~~ No hay sangre que extraer ~~");
    }

    public override void ExitState()
    {
        player.onAnimFinished = null;
    }

    public void OnAnimationFinished()
    {
        player.wateringState.AddBlood(corpseBloodContainer.blood);

        // Destroys blood container object
        CorpseComponent corpse;
        if (corpseBloodContainer.TryGetComponent<CorpseComponent>(out corpse))
            corpse.ConsumeCorpse();
        else 
            Destroy(corpseBloodContainer.gameObject);

        player.ChangeState(player.walkingState);
    }

    // Get posible objects that require a dagger to interact, which are altars and blood containers
    private GameObject GetInteractableObjetAtTheirPosition()
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, 0.1f);

        foreach (Collider2D c in collisions)
        {
            if (c.GetComponent<AltarComponent>() || c.GetComponent<BloodContainer>())
            {
                return c.gameObject;
            }
        }

        return null;
    }
}
