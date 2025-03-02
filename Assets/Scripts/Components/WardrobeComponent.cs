using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardrobeComponent : MonoBehaviour
{
    SpriteRenderer sprite;
    Animator animator;

    private bool initialCutsceneShown = false;
    private bool isWardrobeOpen = false;
    private PlayerComponent player;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerComponent p;
        if (collision.TryGetComponent<PlayerComponent>(out p))
        {
            player = p;

            if (!initialCutsceneShown)
            { 
                PlayInitialCutscene();
            }
            else
            {

            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == player.gameObject)
        {
            CloseWardrobe();
            player = null;
        }
    }

    private void PlayInitialCutscene()
    {
        initialCutsceneShown = true;

        player.SetEnabled(false);

        OpenWardrobe();

        DialogueSystem dialogSys = DialogueSystem.Instance;
        dialogSys.DisplayDialogue(new Dialogue("Rise and shine, farmer.", 2, true, 2f));
        dialogSys.QueueDialogue(new Dialogue("I want you to bring me more seeds.", 2, true, 3f));
        dialogSys.QueueDialogue(new Dialogue("I am not talking about common seeds, of course. You know what I mean.", 2, true, 5f));
        dialogSys.QueueDialogue(new Dialogue("Remember that everything is prepared in the barn.", 2, true, 4f));
        dialogSys.QueueDialogue(new Dialogue("Beware the darkness, \"friend\".", 2, true, 4f));

        float dialoguesTime = 19f;

        Invoke("CloseWardrobe", dialoguesTime);
        // Enable player after all dialogues have finished
        Invoke("EnablePlayer", dialoguesTime);
    }

    public void EnablePlayer()
    {
        if (player != null)
            player.SetEnabled(true);
    }

    private void OpenWardrobe()
    {
        if (isWardrobeOpen) return;

        isWardrobeOpen = true;
        animator.SetTrigger("Open");
    }

    private void CloseWardrobe()
    {
        if (!isWardrobeOpen) return;

        isWardrobeOpen = false;
        animator.SetTrigger("Close");
    }
}
