using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance = null;

    List<Dialogue> queuedDialogues;
    Dialogue displayedDialogue;
    bool isDisplayingDialogue;
    Sequence dialogueBoxSequence;
    [SerializeField]
    GameObject dialogObject;
    AudioSource audioSource;
    public Color textColor;

    public AudioClip dialogueSound;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        queuedDialogues = new List<Dialogue>(2);
        displayedDialogue = null;
        isDisplayingDialogue = false;

        // Intial dialog box values
        dialogObject.SetActive(false);
        dialogObject.transform.GetChild(0).transform.localScale = Vector3.zero;

        audioSource = GetComponent<AudioSource>();
    }

    public bool DisplayDialogue(Dialogue dialogueToDisplay)
    {
        if (dialogueToDisplay == null || IsTheSameDialogue(displayedDialogue, dialogueToDisplay)) return false;

        if (displayedDialogue != null)
        {
            if (displayedDialogue.forcedToFinish || displayedDialogue.priority > dialogueToDisplay.priority)
                return false;
            else
                CancelInvoke("OnDialogueDisplayTimeFinished");
        }

        if (!isDisplayingDialogue)
            ShowDialogueDisplay();

        displayedDialogue = dialogueToDisplay;

        TextMeshProUGUI textMesh = dialogObject.GetComponentInChildren<TextMeshProUGUI>();
        textMesh.text = dialogueToDisplay.text;
        textMesh.color = textColor;

        audioSource.PlayOneShot(dialogueSound);

        Invoke("OnDialogueDisplayTimeFinished", displayedDialogue.displayTime);
        return true;
    }

    public void DisplayOrQueueDialogue(Dialogue dialogueToDisplay)
    {
        bool dialogueDisplayed = DisplayDialogue(dialogueToDisplay);

        if (!dialogueDisplayed)
            QueueDialogue(dialogueToDisplay);
    }

    public void QueueDialogue(Dialogue newDialogue)
    {
        if (queuedDialogues == null || IsTheSameDialogue(displayedDialogue, newDialogue)) return;
        bool dialogueQueued = false;

        for (int i = 0; i < queuedDialogues.Count; i++)
        {
                // If the dialogue being queued is the same as another queued dialogue, do not queue it again
            if (IsTheSameDialogue(queuedDialogues[i], newDialogue))
            {
                 return;
            }
            else if (queuedDialogues[i].priority < newDialogue.priority)
            {
                queuedDialogues.Insert(i, newDialogue);
                dialogueQueued = true;
                break;
            }
        }

        if (!dialogueQueued)
            queuedDialogues.Add(newDialogue);

        if (!isDisplayingDialogue)
            DisplayNextDialogue();
    }

    public bool IsTheSameDialogue(Dialogue dialogue1, Dialogue dialogue2)
    {
        if (dialogue1 == null || dialogue2 == null) return false;

        return dialogue1.priority == dialogue2.priority && dialogue1.text == dialogue2.text;
    }

    private void DisplayNextDialogue()
    {
        if (queuedDialogues.Count > 0)
        {
            Dialogue nextDialogue = queuedDialogues[0];
            queuedDialogues.RemoveAt(0);

            DisplayDialogue(nextDialogue);
        }
    }

    private void OnDialogueDisplayTimeFinished()
    {
        displayedDialogue = null;

        if (queuedDialogues.Count > 0)
            DisplayNextDialogue();
        else
            HideDialogueDisplay();
    }

    private void ShowDialogueDisplay()
    {
        // TODO: Activate UI where dialogue is displayed
        if (dialogueBoxSequence != null && dialogueBoxSequence.IsActive())
            dialogueBoxSequence.Kill();

        dialogueBoxSequence = DOTween.Sequence();
        dialogueBoxSequence.AppendCallback(() => dialogObject.SetActive(true))
            .Append(dialogObject.transform.GetChild(0).DOScale(1, 0.8f))
            .OnKill(() => dialogueBoxSequence = null);

        isDisplayingDialogue = true;
    }

    private void HideDialogueDisplay()
    {
        // TODO: Hide dialogue UI
        if (dialogueBoxSequence != null && dialogueBoxSequence.IsActive())
            dialogueBoxSequence.Kill();

        dialogueBoxSequence = DOTween.Sequence();
        dialogueBoxSequence.Append(dialogObject.transform.GetChild(0).DOScale(0, 0.8f))
            .AppendCallback(() => dialogObject.SetActive(false))
            .OnKill(() => dialogueBoxSequence = null);
        
        isDisplayingDialogue = false;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        queuedDialogues.Clear();
        CancelInvoke("OnDialogueDisplayTimeFinished");
    }
}

public class Dialogue
{
    public string text;
    public int priority;
    public bool forcedToFinish;
    public float displayTime;

    public Dialogue(string t, int p = 0, bool finish = false, float time = 3f)
    {
        text = t; priority = p; forcedToFinish = finish; displayTime = time;
    }
}
