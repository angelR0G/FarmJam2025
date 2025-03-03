using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WardrobeComponent : MonoBehaviour
{
    static readonly string[] HINTS = {
        "Have you any seed for me?",
        "Have you forgotten about the seeds?",
        "Bring me more seeds, \"friend\"."
    };

    static readonly Color TEXT_COLOR = new Color(1, 0, 0);

    SpriteRenderer sprite;
    Animator animator;

    private bool initialCutsceneShown = false;
    private bool isWardrobeOpen = false;
    private bool isOfferingCompleted = false;

    private PlayerComponent player;
    private List<ItemId> seedsOffered = new List<ItemId>();

    public GameObject rewardPrefab;
    public Light2D wardrobeLight;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        wardrobeLight.pointLightInnerAngle = 0f;
        wardrobeLight.pointLightOuterAngle = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerComponent p;
        CropComponent crop;
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
                Invoke("ShowHint", 15f);
            }
        }


        if (!isOfferingCompleted && collision.GetComponent<CarriableComponent>() && collision.TryGetComponent<CropComponent>(out crop))
        {
            CancelInvoke("ShowHint");

            // Disable crop's colliders
            foreach (Collider2D col in crop.GetComponents<Collider2D>())
                col.enabled = false;
            
            if (seedsOffered.Contains(crop.collectableCrop))
            {
                RepeatedSeedOfferedCutscene(crop);
            }
            else
            {
                seedsOffered.Add(crop.collectableCrop);

                if (seedsOffered.Count == 4)
                {
                    isOfferingCompleted = true;
                    LastSeedOfferedCutscene(crop);
                }
                else
                {
                    NewSeedOfferedCutscene(crop);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (player != null && collision.gameObject == player.gameObject)
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
        dialogSys.textColor = TEXT_COLOR;
        dialogSys.DisplayDialogue(new Dialogue("Rise and shine, farmer.", 2, true, 3f));
        dialogSys.QueueDialogue(new Dialogue("I want you to bring me more seeds.", 2, true, 3f));
        dialogSys.QueueDialogue(new Dialogue("I am not talking about common seeds, of course. You know what I mean.", 2, true, 5f));
        dialogSys.QueueDialogue(new Dialogue("Once you have them, plant them and make them grow.", 2, true, 4f));
        dialogSys.QueueDialogue(new Dialogue("Remember that they require a special soil and nutrients to grow.", 2, true, 5f));
        dialogSys.QueueDialogue(new Dialogue("The barn is ready. You know what to do to obtain the seeds.", 2, true, 4f));
        dialogSys.QueueDialogue(new Dialogue("Beware the darkness, \"friend\".", 2, true, 4f));

        float dialoguesTime = 28f;

        // Enable player after all dialogues have finished
        Invoke("CloseWardrobe", dialoguesTime);
        Invoke("EnablePlayer", dialoguesTime + 1f);

        initialCutsceneShown = true;
    }

    private void NewSeedOfferedCutscene(CropComponent crop)
    {
        player?.SetEnabled(false);

        DialogueSystem dialogSys = DialogueSystem.Instance;
        dialogSys.textColor = TEXT_COLOR;
        dialogSys.DisplayDialogue(new Dialogue("Good job, farmer. You never disappoint me.", 2, true, 4f));

        Sequence sequence = DOTween.Sequence().SetRecyclable(true).SetDelay(4f);
        sequence.Append(crop.gameObject.transform.DOMove(transform.position, 0.5f).SetRecyclable(true))
            .AppendCallback(PlayTakeAnimation)
            .Append(crop.sprite.DOFade(0, 1f).SetRecyclable(true));

        float dialoguesTime = 6f;
        if (seedsOffered.Count == 1)
        {
            sequence.InsertCallback(dialoguesTime, () =>
            {
                dialogSys.QueueDialogue(new Dialogue("However, this is just the beginning. My needs are far from over...", 2, true, 4f));
                dialogSys.QueueDialogue(new Dialogue("Different bodies, different seeds. That's how it works.", 2, true, 3f));
            });
            dialoguesTime += 7f;
        }
        else if(seedsOffered.Count == 2)
        {
            sequence.InsertCallback(dialoguesTime, () => 
                dialogSys.QueueDialogue(new Dialogue("This is not enough, though. Bring me more.", 2, true, 3f))
            );
            dialoguesTime += 3f;
        }
        else if(seedsOffered.Count == 3)
        {
            sequence.InsertCallback(dialoguesTime, () =>
                dialogSys.QueueDialogue(new Dialogue("We are almost done. One last seed, and you'll finally have what you seek.", 2, true, 5f))
            );
            dialoguesTime += 5f;
        }
        sequence.AppendCallback(() => dialogSys.QueueDialogue(new Dialogue("I'll be waiting... don't keep me waiting too long.", 2, true, 3f)));
        dialoguesTime += 3f;

        // Enable player after all dialogues have finished
        Invoke("EnablePlayer", dialoguesTime);
    }

    private void RepeatedSeedOfferedCutscene(CropComponent crop)
    {
        player?.SetEnabled(false);

        DialogueSystem dialogSys = DialogueSystem.Instance;
        dialogSys.textColor = TEXT_COLOR;
        dialogSys.DisplayDialogue(new Dialogue("Mmmm... You already brought me this seed, farmer.", 2, true, 4f));
        dialogSys.QueueDialogue(new Dialogue("I'll take it, but I was expecting new seeds. Different bodies, different seeds. That's how it works.", 2, true, 5f));

        Sequence sequence = DOTween.Sequence().SetRecyclable(true).SetDelay(9f);
        sequence.Append(crop.gameObject.transform.DOMove(transform.position, 0.5f).SetRecyclable(true))
            .AppendCallback(PlayTakeAnimation)
            .Append(crop.sprite.DOFade(0, 1f).SetRecyclable(true));

        float dialoguesTime = 11f;
        if (player != null)
        {
            float health = player.healthComponent.GetHealthPercentage();
            float sanity = player.sanityComponent.GetSanityPercentage();
            if (health < 1f || sanity < 1f)
            {
                if (player.healthComponent.GetHealthPercentage() <= player.sanityComponent.GetSanityPercentage())
                {
                    sequence.InsertCallback(dialoguesTime, () =>
                    {
                        dialogSys.QueueDialogue(new Dialogue("You don't look well. Let me help you with that.", 2, true, 4f));
                        player.healthComponent.RestoreFullHealth();
                    });
                    dialoguesTime += 4f;
                }
                else
                {
                    sequence.InsertCallback(dialoguesTime, () =>
                    {
                        dialogSys.QueueDialogue(new Dialogue("It seems like you're losing your mind. Let me help you with that.", 2, true, 5f));
                        player.sanityComponent.RestoreFullSanity();
                    });
                    dialoguesTime += 5f;
                }
            }
        }
        sequence.AppendCallback(() => dialogSys.QueueDialogue(new Dialogue("Next time, bring me new seeds. I'll be waiting for you.", 2, true, 4f)));
        dialoguesTime += 4f;

        // Enable player after all dialogues have finished
        Invoke("EnablePlayer", dialoguesTime);
    }

    public void LastSeedOfferedCutscene(CropComponent crop)
    {
        player?.SetEnabled(false);

        DialogueSystem dialogSys = DialogueSystem.Instance;
        dialogSys.textColor = TEXT_COLOR;
        dialogSys.DisplayDialogue(new Dialogue("Good job, \"friend\". You brought me all the seeds I was looking for.", 2, true, 5f));

        Sequence sequence = DOTween.Sequence().SetRecyclable(true).SetDelay(4f);
        sequence.Append(crop.gameObject.transform.DOMove(transform.position, 0.5f).SetRecyclable(true))
            .AppendCallback(PlayTakeAnimation)
            .Append(crop.sprite.DOFade(0, 1f).SetRecyclable(true));

        float dialoguesTime = 7f;
        sequence.InsertCallback(dialoguesTime, () =>
        {
            dialogSys.QueueDialogue(new Dialogue("Now it's my turn. I'll give you what you've been searching for, what you lost long ago.", 2, true, 6f));
            dialogSys.QueueDialogue(new Dialogue("Here is your reward.", 2, true, 2f));
        });
        dialoguesTime += 8f;

        sequence.InsertCallback(dialoguesTime, () =>
        {
            Instantiate(rewardPrefab, transform.position + new Vector3(0.16f, -0.08f), Quaternion.identity);
        });
        dialoguesTime += 3f;

        sequence.InsertCallback(dialoguesTime, () =>
        {
            dialogSys.QueueDialogue(new Dialogue("Goodbye, farmer. We'll meet again... very soon.", 2, true, 4f));
            Invoke("CloseWardrobe", 3f);
        });
        dialoguesTime += 5f;

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
        wardrobeLight.enabled = true;
        DOTween.To(() => wardrobeLight.pointLightInnerAngle, (v) => wardrobeLight.pointLightInnerAngle = v, 70f, 0.3f).SetDelay(0.1f).SetRecyclable(true);
        DOTween.To(() => wardrobeLight.pointLightOuterAngle, (v) => wardrobeLight.pointLightOuterAngle = v, 120f, 0.3f).SetDelay(0.1f).SetRecyclable(true);
    }

    private void CloseWardrobe()
    {
        if (!isWardrobeOpen) return;

        isWardrobeOpen = false;
        animator.SetTrigger("Close");
        DOTween.To(() => wardrobeLight.pointLightInnerAngle, (v) => wardrobeLight.pointLightInnerAngle = v, 0f, 0.3f).SetRecyclable(true);
        DOTween.To(() => wardrobeLight.pointLightOuterAngle, (v) => wardrobeLight.pointLightOuterAngle = v, 0f, 0.3f)
            .OnComplete(()=> wardrobeLight.enabled = false).SetRecyclable(true);

        DialogueSystem.Instance.textColor = Color.white;
        CancelInvoke("ShowHint");
    }

    private void PlayTakeAnimation()
    {
        animator.SetTrigger("Take");
    }

    private void ShowHint()
    {
        string hint = HINTS[Random.Range(0, HINTS.Length)];
        DialogueSystem dialogSys = DialogueSystem.Instance;

        dialogSys.textColor = TEXT_COLOR;
        dialogSys.DisplayDialogue(new Dialogue(hint));
        dialogSys.textColor = Color.white;
    }
}
