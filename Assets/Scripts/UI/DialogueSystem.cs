using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance = null;

    List<Dialogue> queuedDialogues;
    Dialogue displayedDialogue;
    bool isDisplayingDialogue;

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
    }

    public void DisplayDialogue(Dialogue dialogueToDisplay)
    {
        if (dialogueToDisplay == null) return;

        if (displayedDialogue != null)
        {
            if (displayedDialogue.forcedToFinish)
                return;
            else
                CancelInvoke("OnDialogueDisplayTimeFinished");
        }

        if (!isDisplayingDialogue)
            ShowDialogueDisplay();

        displayedDialogue = dialogueToDisplay;

        // TODO: Update UI text
        Debug.Log(dialogueToDisplay.text);

        Invoke("OnDialogueDisplayTimeFinished", displayedDialogue.displayTime);
    }

    public void QueueDialogue(Dialogue newDialogue)
    {
        if (queuedDialogues == null) return;
        bool dialogueQueued = false;

        for (int i = 0; i < queuedDialogues.Count; i++)
        {
            if (queuedDialogues[i].priority < newDialogue.priority)
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
        Debug.Log("###############################################");

        isDisplayingDialogue = true;
    }

    private void HideDialogueDisplay()
    {
        // TODO: Hide dialogue UI
        Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

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

    public Dialogue(string t, int p = 0, bool finish = false, float time = 5f)
    {
        text = t; priority = p; forcedToFinish = finish; displayTime = time;
    }
}
