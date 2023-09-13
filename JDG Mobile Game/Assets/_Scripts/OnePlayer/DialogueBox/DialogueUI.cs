using System;
using System.Collections;
using System.Linq;
using OnePlayer.DialogueBox;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CurrentDialogIndex : UnityEvent<int>
{
}

public class TriggerDoneEvent : UnityEvent<NextDialogueTrigger>
{
    
}

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private DialogueObject testDialogue;

    public static readonly CurrentDialogIndex DialogIndex = new CurrentDialogIndex();
    private NextDialogueTrigger currentTrigger = NextDialogueTrigger.Undefined;
    public static readonly TriggerDoneEvent TriggerDoneEvent = new TriggerDoneEvent();

    private ResponseHandler responseHandler;
    private TypewriterEffect typewriterEffect;
    private int currentSoundIndex = 0;

    private AudioSource audioSource;

    private void Start()
    {
        currentSoundIndex = 0;
        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();
        audioSource = FindObjectOfType<AudioSource>();
        CloseDialogueBox();
        ShowDialogue(testDialogue);
        TriggerDoneEvent.AddListener(TriggerReceived);
    }

    private void TriggerReceived(NextDialogueTrigger nextDialogueTrigger)
    {
        currentTrigger = nextDialogueTrigger;

    }

    public void ShowDialogue(DialogueObject dialogueObject)
    {
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue((dialogueObject)));
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        var soundDialogIndex = dialogueObject.SoundDialogueIndex;
        var audioClips = dialogueObject.AudioClips;
        for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            DialogIndex.Invoke(i);
            string dialogue = dialogueObject.Dialogue[i];

            if (soundDialogIndex.Contains(i))
            {
                var currentAudioClip = audioClips[currentSoundIndex];
                PlaySound(dialogueObject, dialogue, soundDialogIndex, i, currentAudioClip);
            }
            
            
            yield return typewriterEffect.Run(dialogue, textLabel);

            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses) break;


            yield return new WaitUntil(() =>
            {
                NextDialogueTrigger nextDialogueTrigger = dialogueObject.NextDialogueTriggers[i];
                switch (nextDialogueTrigger)
                {
                    case NextDialogueTrigger.Tap:
#if UNITY_EDITOR
                        return Input.GetKeyDown(KeyCode.Space);
#elif UNITY_ANDROID
                        return Input.touchCount > 0;
#endif
                    case NextDialogueTrigger.Automatic:
                        return true;
                    case NextDialogueTrigger.PutCard:
                        dialogueBox.SetActive(false);
                        if (currentTrigger == NextDialogueTrigger.NextPhase)
                        {
                            dialogueBox.SetActive(true);
                            currentTrigger = NextDialogueTrigger.Undefined;
                            return true;
                        }
                        break;
                    case NextDialogueTrigger.NextPhase:
                        if (currentTrigger == NextDialogueTrigger.NextPhase)
                        {
                            dialogueBox.SetActive(true);
                            currentTrigger = NextDialogueTrigger.Undefined;
                            return true;
                        }
                        break;
                    case NextDialogueTrigger.PutEffectCard:
                        dialogueBox.SetActive(false);
                        if (currentTrigger == NextDialogueTrigger.PutEffectCard)
                        {
                            dialogueBox.SetActive(true);
                            currentTrigger = NextDialogueTrigger.Undefined;
                            return true;
                        }
                        break;
                    case NextDialogueTrigger.Undefined:
                        break;
                    case NextDialogueTrigger.Attack:
                        dialogueBox.SetActive(false);
                        if (currentTrigger == NextDialogueTrigger.NextPhase)
                        {
                            dialogueBox.SetActive(true);
                            currentTrigger = NextDialogueTrigger.Undefined;
                            return true;
                        }
                        break;
                    case NextDialogueTrigger.EndVideo:
                        dialogueBox.SetActive(false);
                        if (currentTrigger == NextDialogueTrigger.EndVideo)
                        {
                            dialogueBox.SetActive(true);
                            currentTrigger = NextDialogueTrigger.Undefined;
                            return true;
                        }
                        break;
                    case NextDialogueTrigger.EndGame:
                        SceneManager.LoadSceneAsync("MainScreen", LoadSceneMode.Single);
                        return true;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return false;
            });
        }

        if (dialogueObject.HasResponses)
        {
            responseHandler.ShowResponses(dialogueObject.Responses);
        }
        else
        {
            CloseDialogueBox();
        }
    }

    private void PlaySound(DialogueObject dialogueObject, string dialogue, int[] soundDialogIndex, int i,
        AudioClip audioClip)
    {
        var length = dialogue.Length;
        var indexInSound = Array.IndexOf(soundDialogIndex, i);
        if (indexInSound < (soundDialogIndex.Length - 1))
        {
            var nextSoundDialogIndex = soundDialogIndex[indexInSound + 1]; // next DialogIndex
            if (nextSoundDialogIndex > (i + 1)) // if greater than the next one, there is multiple text so duration is longer
            {
                for (int j = (i + 1); j < nextSoundDialogIndex; j++)
                {
                    length += dialogueObject.Dialogue[j].Length;
                }
            }
        }
        else
        {
            // indexSound == soundDialogIndex.Length - 1
            // Get all text till the end
            for (int j = (i + 1); j < dialogueObject.Dialogue.Length; j++)
            {
                length += dialogueObject.Dialogue[j].Length;
            }
        }

        audioSource.Stop();
        typewriterEffect.AdaptSpeedToLength(audioClip.length, length);
        audioSource.PlayOneShot(audioClip);
        currentSoundIndex++;
    }

    private void CloseDialogueBox()
    {
        dialogueBox.SetActive(false);
        textLabel.text = String.Empty;
    }
}