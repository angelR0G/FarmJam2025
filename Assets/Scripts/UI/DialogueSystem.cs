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
    [SerializeField]
    GameObject dialogObject;
    Vector3 dialogMeasures = new Vector3(340, 80, 0);

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
        dialogObject.GetComponentInChildren<TextMeshProUGUI>().text = dialogueToDisplay.text;

        Invoke("OnDialogueDisplayTimeFinished", displayedDialogue.displayTime);
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
        Debug.Log("###############################################");
        dialogObject.transform.GetChild(0).DOScale(new Vector3(0, 0, 0), 0);
        dialogObject.SetActive(true);

        dialogObject.transform.GetChild(0).DOScale(new Vector3(1, 1, 1), 0.8f);

        isDisplayingDialogue = true;
    }

    private void HideDialogueDisplay()
    {
        // TODO: Hide dialogue UI
        Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        StartCoroutine(HideDialogCorroutine());
        
        isDisplayingDialogue = false;
    }
    IEnumerator HideDialogCorroutine()
    {
        dialogObject.transform.GetChild(0).DOScale(new Vector3(0, 0, 0), 0.8f);
        yield return new WaitForSeconds(1);
        dialogObject.SetActive(false);
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
