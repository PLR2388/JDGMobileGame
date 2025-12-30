using System;
using System.Collections;
using System.Linq;
using JDG.Application;
using JDG.Application.Services;
using JDG.Domain.Events;
using OnePlayer.DialogueBox;
using TMPro;
using UnityEngine;
using VContainer;

/// <summary>
/// Represents the user interface for dialogues in the game.
/// Phase 55: Uses ISceneLoaderService instead of SceneLoaderSystem static calls.
/// Phase 123: Removed static UnityEvents - uses EventBus for DialogueTriggerCompletedEvent and DialogueIndexChangedEvent.
/// </summary>
public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private DialogueObject testDialogue;

    private NextDialogueTrigger currentTrigger = NextDialogueTrigger.Undefined;

    private ResponseHandler responseHandler;
    private TypewriterEffect typewriterEffect;
    private int currentSoundIndex = 0;

    private AudioSource audioSource;

    // Phase 55: ISceneLoaderService instead of SceneLoaderSystem static calls
    private ISceneLoaderService _sceneLoaderService;

    // Phase 90: ITutorialStateService replaces DialogueTutoHandler singleton
    private ITutorialStateService _tutorialStateService;

    // Phase 123: EventBus for dialogue events
    private IEventBus _eventBus;
    private IDisposable _triggerSubscription;

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 55: Added ISceneLoaderService to replace SceneLoaderSystem static calls.
    /// Phase 90: Added ITutorialStateService to replace DialogueTutoHandler singleton.
    /// Phase 123: Added IEventBus for dialogue events.
    /// </summary>
    [Inject]
    public void Construct(ISceneLoaderService sceneLoaderService, ITutorialStateService tutorialStateService, IEventBus eventBus)
    {
        _sceneLoaderService = sceneLoaderService;
        _tutorialStateService = tutorialStateService;
        _eventBus = eventBus;
    }

    /// <summary>
    /// Initialization method.
    /// Phase 123: Subscribe to DialogueTriggerCompletedEvent via EventBus.
    /// </summary>
    private void Start()
    {
        currentSoundIndex = 0;
        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();
        audioSource = FindFirstObjectByType<AudioSource>();
        CloseDialogueBox();
        ShowDialogue(testDialogue);
        // Phase 123: Subscribe via EventBus
        _triggerSubscription = _eventBus?.Subscribe<DialogueTriggerCompletedEvent>(OnTriggerCompleted);
    }

    /// <summary>
    /// Cleanup when the object is destroyed.
    /// Phase 123: Dispose EventBus subscription.
    /// </summary>
    private void OnDestroy()
    {
        _triggerSubscription?.Dispose();
    }

    /// <summary>
    /// Handles DialogueTriggerCompletedEvent from EventBus.
    /// Phase 123: Replaces static TriggerDoneEvent listener.
    /// </summary>
    private void OnTriggerCompleted(DialogueTriggerCompletedEvent evt)
    {
        TriggerReceived((NextDialogueTrigger)evt.TriggerType);
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
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    /// <summary>
    /// Steps through each line of dialogue.
    /// </summary>
    /// <param name="dialogueObject">The dialogue data to be stepped through.</param>
    /// <returns>An IEnumerator for coroutine.</returns>
    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        if (dialogueObject == null || dialogueObject.Dialogue == null)
        {
            Debug.LogError("DialogueUI.StepThroughDialogue: dialogueObject or Dialogue is null");
            yield break;
        }

        var soundDialogIndex = dialogueObject.SoundDialogueIndex ?? Array.Empty<int>();
        var audioClips = dialogueObject.AudioClips ?? Array.Empty<AudioClip>();

        for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            // Phase 123: Publish via EventBus
            _eventBus?.Publish(new DialogueIndexChangedEvent { DialogueIndex = i });
            // Phase 90: Also update tutorial state service
            _tutorialStateService?.SetDialogIndex(i);
            string dialogue = dialogueObject.Dialogue[i];

            if (soundDialogIndex.Contains(i))
            {
                // Bounds check for audioClips array
                if (currentSoundIndex >= 0 && currentSoundIndex < audioClips.Length)
                {
                    var currentAudioClip = audioClips[currentSoundIndex];
                    if (currentAudioClip != null)
                    {
                        var length = ComputeLengthText(dialogueObject, dialogue, soundDialogIndex, i);
                        typewriterEffect?.AdaptSpeedToLength(currentAudioClip.length, length);
                        PlaySound(currentAudioClip);
                    }
                }
                else
                {
                    Debug.LogWarning($"DialogueUI: currentSoundIndex {currentSoundIndex} out of bounds for audioClips length {audioClips.Length}");
                }
            }

            // Null check for typewriterEffect
            if (typewriterEffect != null)
            {
                yield return typewriterEffect.Run(dialogue, textLabel);
            }

            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses) break;

            // Bounds check for NextDialogueTriggers array
            var triggers = dialogueObject.NextDialogueTriggers;
            if (triggers != null && i < triggers.Length)
            {
                yield return new WaitUntil(() =>
                {
                    NextDialogueTrigger nextDialogueTrigger = triggers[i];
                    return IsNextDialogueReady(nextDialogueTrigger);
                });
            }
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
                // Phase 55: Use ISceneLoaderService instead of SceneLoaderSystem
                _sceneLoaderService.LoadMainScreen();
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