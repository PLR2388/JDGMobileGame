using System;
using System.Collections;
using System.Linq;
using OnePlayer.DialogueBox;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Unity event that broadcasts the current dialogue index.
/// </summary>
[Serializable]
public class CurrentDialogIndex : UnityEvent<int>
{
}

/// <summary>
/// Unity event that broadcasts when a specific trigger has been executed.
/// </summary>
public class TriggerDoneEvent : UnityEvent<NextDialogueTrigger>
{
    
}

/// <summary>
/// Represents the user interface for dialogues in the game.
/// </summary>
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

    /// <summary>
    /// Initialization method.
    /// </summary>
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

    /// <summary>
    /// Updates the current dialogue trigger.
    /// </summary>
    /// <param name="nextDialogueTrigger">The next dialogue trigger.</param>
    private void TriggerReceived(NextDialogueTrigger nextDialogueTrigger)
    {
        currentTrigger = nextDialogueTrigger;
    }

    /// <summary>
    /// Displays the given dialogue on the UI.
    /// </summary>
    /// <param name="dialogueObject">The dialogue data to be displayed.</param>
    public void ShowDialogue(DialogueObject dialogueObject)
    {
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue((dialogueObject)));
    }

    /// <summary>
    /// Steps through each line of dialogue.
    /// </summary>
    /// <param name="dialogueObject">The dialogue data to be stepped through.</param>
    /// <returns>An IEnumerator for coroutine.</returns>
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
                var length = ComputeLengthText(dialogueObject, dialogue, soundDialogIndex, i);
                typewriterEffect.AdaptSpeedToLength(currentAudioClip.length, length);
                PlaySound(currentAudioClip);
            }
            
            yield return typewriterEffect.Run(dialogue, textLabel);

            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses) break;
            
            yield return new WaitUntil(() =>
            {
                NextDialogueTrigger nextDialogueTrigger = dialogueObject.NextDialogueTriggers[i];
                return IsNextDialogueReady(nextDialogueTrigger);
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
    
    /// <summary>
    /// Determines if the conditions for the next dialogue are met.
    /// </summary>
    /// <param name="nextDialogueTrigger">The type of trigger for the next dialogue.</param>
    /// <returns>True if conditions are met; otherwise, false.</returns>
    private bool IsNextDialogueReady(NextDialogueTrigger nextDialogueTrigger)
    {
        switch (nextDialogueTrigger)
        {
            case NextDialogueTrigger.Tap:
                return InputManager.IsTap;
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
                SceneLoaderSystem.LoadMainScreen();
                return true;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return false;
    }

    /// <summary>
    /// Plays the given sound.
    /// </summary>
    /// <param name="audioClip">The audio clip to be played.</param>
    private void PlaySound(AudioClip audioClip)
    {
        audioSource.Stop();
        audioSource.PlayOneShot(audioClip);
        currentSoundIndex++;
    }
    
    /// <summary>
    /// Computes the total length of the text based on the dialogue object and the current sound index.
    /// </summary>
    /// <param name="dialogueObject">The dialogue data.</param>
    /// <param name="dialogue">The current dialogue text.</param>
    /// <param name="soundDialogIndex">Array of sound dialogue indices.</param>
    /// <param name="i">The current index in the dialogue array.</param>
    /// <returns>The computed length of the text.</returns>
    private static int ComputeLengthText(DialogueObject dialogueObject, string dialogue, int[] soundDialogIndex, int i)
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
        return length;
    }

    /// <summary>
    /// Closes the dialogue box UI.
    /// </summary>
    private void CloseDialogueBox()
    {
        dialogueBox.SetActive(false);
        textLabel.text = String.Empty;
    }
}