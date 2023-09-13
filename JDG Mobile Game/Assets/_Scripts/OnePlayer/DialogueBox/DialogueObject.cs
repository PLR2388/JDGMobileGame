using OnePlayer.DialogueBox;
using UnityEngine;

/// <summary>
/// Represents a dialogue configuration with options for dialogue text, triggers, responses, and associated audio.
/// </summary>
[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    [SerializeField] [TextArea] private string[] dialogue;
    [SerializeField] private NextDialogueTrigger[] nextDialogueTriggers;
    [SerializeField] private Response[] responses;
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private int[] soundDialogueIndex;

    /// <summary>
    /// Gets the dialogue lines.
    /// </summary>
    public string[] Dialogue => dialogue;

    /// <summary>
    /// Gets a value indicating whether this dialogue object has associated responses.
    /// </summary>
    public bool HasResponses => Responses != null && Responses.Length > 0;

    /// <summary>
    /// Gets the array of possible responses for the dialogue.
    /// </summary>
    public Response[] Responses => responses;

    /// <summary>
    /// Gets the array of triggers for subsequent dialogues.
    /// </summary>
    public NextDialogueTrigger[] NextDialogueTriggers => nextDialogueTriggers;

    /// <summary>
    /// Gets the indices of the dialogue lines associated with audio clips.
    /// </summary>
    public int[] SoundDialogueIndex => soundDialogueIndex;

    /// <summary>
    /// Gets the array of audio clips associated with the dialogue lines.
    /// </summary>
    public AudioClip[] AudioClips => audioClips;
}
