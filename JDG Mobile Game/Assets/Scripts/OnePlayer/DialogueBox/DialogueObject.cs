using OnePlayer.DialogueBox;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    [SerializeField] [TextArea] private string[] dialogue;
    [SerializeField] private NextDialogueTrigger[] nextDialogueTriggers;
    [SerializeField] private Response[] responses;

    public string[] Dialogue => dialogue;

    public bool hasResponses => Responses != null && Responses.Length > 0;

    public Response[] Responses => responses;

    public NextDialogueTrigger[] NextDialogueTriggers => nextDialogueTriggers;
}
