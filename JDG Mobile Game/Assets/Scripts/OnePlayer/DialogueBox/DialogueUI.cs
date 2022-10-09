using System;
using System.Collections;
using OnePlayer.DialogueBox;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class NextDialogueEvent : UnityEvent<NextDialogueTrigger>
{
}

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private DialogueObject testDialogue;

    public static readonly NextDialogueEvent NextDialogueTriggerEvent = new NextDialogueEvent();
    private NextDialogueTrigger currentTrigger = NextDialogueTrigger.Undefined;

    private ResponseHandler responseHandler;
    private TypewriterEffect typewriterEffect;

    private void Start()
    {
        NextDialogueTriggerEvent.AddListener(ReceiveNextDialogueEvent);
        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();
        CloseDialogueBox();
        ShowDialogue(testDialogue);
    }

    private void ReceiveNextDialogueEvent(NextDialogueTrigger nextDialogueEvent)
    {
        currentTrigger = nextDialogueEvent;
    }

    public void ShowDialogue(DialogueObject dialogueObject)
    {
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue((dialogueObject)));
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            string dialogue = dialogueObject.Dialogue[i];
            yield return typewriterEffect.Run(dialogue, textLabel);

            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.hasResponses) break;


            yield return new WaitUntil(() =>
            {
                NextDialogueTrigger nextDialogueTrigger = dialogueObject.NextDialogueTriggers[i];
                if (nextDialogueTrigger == NextDialogueTrigger.Tap)
                {
#if UNITY_EDITOR
                    return Input.GetKeyDown(KeyCode.Space);
#elif UNITY_ANDROID
                        return Input.GetTouch(0);
#endif
                }
                else if (nextDialogueTrigger == currentTrigger)
                {
                    currentTrigger = NextDialogueTrigger.Undefined;
                    return true;
                }
                else
                {
                    return false;
                }
            }); // Wait for press on Space bar to go to next
        }

        if (dialogueObject.hasResponses)
        {
            responseHandler.ShowResponses(dialogueObject.Responses);
        }
        else
        {
            CloseDialogueBox();
        }
    }

    private void CloseDialogueBox()
    {
        dialogueBox.SetActive(false);
        textLabel.text = String.Empty;
    }
}