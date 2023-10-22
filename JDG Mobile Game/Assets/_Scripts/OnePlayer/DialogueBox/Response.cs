using UnityEngine;

/// <summary>
/// Represents a player's potential response in a dialogue system.
/// </summary>
[System.Serializable]
public class Response
{
    [SerializeField] private string responseText;
    [SerializeField] private DialogueObject dialogueObject;

    /// <summary>
    /// Gets the text of the response.
    /// </summary>
    public string ResponseText => responseText;

    /// <summary>
    /// Gets the dialogue object associated with this response.
    /// </summary>
    public DialogueObject DialogueObject => dialogueObject;
}
