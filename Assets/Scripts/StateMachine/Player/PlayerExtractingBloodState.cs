using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExtractingBloodState : PlayerState
{
    private BloodContainer corpseBloodContainer;

    public AudioClip extractBloodSound;

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

                player.audioSource.clip = extractBloodSound;
                player.audioSource.Play();

                // If is destroyable by daylight, increase safe areas count to prevent being destroyed
                DaylightDestroyComponent daylightDestroyable;
                if (interactableObject.TryGetComponent<DaylightDestroyComponent>(out daylightDestroyable))
                    daylightDestroyable.SafeAreasCount++;
                return;
            }
        }

        player.ChangeState(player.walkingState);
        DialogueSystem.Instance.DisplayDialogue(new Dialogue("There is no blood to extract here."));
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
