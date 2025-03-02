using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class WardrobeComponent : MonoBehaviour
{
    static readonly string[] hints = {
        "Have you any seed for me?",
        "Have you forgotten about the seeds?",
        "Bring me more seeds, \"friend\"."
    };

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
                OpenWardrobe();
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

        player.SetEnabled(false);

        OpenWardrobe();

        DialogueSystem dialogSys = DialogueSystem.Instance;
        dialogSys.DisplayDialogue(new Dialogue("Rise and shine, farmer.", 2, true, 2f));
        dialogSys.QueueDialogue(new Dialogue("I want you to bring me more seeds.", 2, true, 3f));
        dialogSys.QueueDialogue(new Dialogue("I am not talking about common seeds, of course. You know what I mean.", 2, true, 5f));
        dialogSys.QueueDialogue(new Dialogue("Once you have them, plant them and make them grow.", 2, true, 4f));
        dialogSys.QueueDialogue(new Dialogue("Remember that they require a special soil and nutrients to grow.", 2, true, 5f));
        dialogSys.QueueDialogue(new Dialogue("The barn is ready. You know what to do to obtain the seeds.", 2, true, 4f));
        dialogSys.QueueDialogue(new Dialogue("Beware the darkness, \"friend\".", 2, true, 4f));

        float dialoguesTime = 27f;

        // Enable player after all dialogues have finished
        Invoke("CloseWardrobe", dialoguesTime);
        Invoke("EnablePlayer", dialoguesTime + 1f);

        initialCutsceneShown = true;
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

        if (initialCutsceneShown)
            Invoke("ShowHint", 15f);
    }

    private void CloseWardrobe()
    {
        if (!isWardrobeOpen) return;

        isWardrobeOpen = false;
        animator.SetTrigger("Close");

        CancelInvoke("ShowHint");
    }

    private void ShowHint()
    {
        string hint = hints[Random.Range(0, hints.Length)];

        DialogueSystem.Instance.DisplayDialogue(new Dialogue(hint));
    }
}
