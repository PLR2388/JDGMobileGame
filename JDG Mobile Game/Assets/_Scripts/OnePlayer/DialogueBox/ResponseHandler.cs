using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages and displays player responses during a dialogue.
/// </summary>
public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private RectTransform responseBox;
    [SerializeField] private RectTransform responseButtonTemplate;
    [SerializeField] private RectTransform responseContainer;

    private DialogueUI dialogueUI;

    private readonly List<GameObject> activeResponseButtons = new List<GameObject>();

    /// <summary>
    /// Initialize the ResponseHandler by getting the required DialogueUI component.
    /// </summary>
    private void Start()
    {
        dialogueUI = GetComponent<DialogueUI>();
    }

    /// <summary>
    /// Display the possible responses to the player.
    /// </summary>
    /// <param name="responses">An array of possible responses.</param>
    public void ShowResponses(Response[] responses)
    {
        ClearResponseBox();
        
        float responseBoxHeight = 0;
        foreach (Response response in responses)
        {
            CreateResponseButton(response);
            responseBoxHeight += responseButtonTemplate.sizeDelta.y;
        }

        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight);
        responseBox.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// Create a button for the given response.
    /// </summary>
    /// <param name="response">The response for which the button is created.</param>
    /// <returns>The created button GameObject.</returns>
    private void CreateResponseButton(Response response)
    {
        GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);
        responseButton.SetActive(true);
        
        TMP_Text textComponent = responseButton.GetComponent<TMP_Text>();
        textComponent.text = response.ResponseText;

        Button buttonComponent = responseButton.GetComponent<Button>();
        buttonComponent.onClick.AddListener(() => OnPickedResponse(response));
        
        activeResponseButtons.Add(responseButton);
    }

    /// <summary>
    /// Handle the player's selected response and proceed with the dialogue.
    /// </summary>
    /// <param name="response">The selected player response.</param>
    private void OnPickedResponse(Response response)
    {
        ClearResponseBox();
        dialogueUI.ShowDialogue(response.DialogueObject);
    }
    
    /// <summary>
    /// Clear all active response buttons and hide the response box.
    /// </summary>
    private void ClearResponseBox()
    {
        responseBox.gameObject.SetActive(false);

        for (int i = activeResponseButtons.Count - 1; i >= 0; i--)
        {
            Destroy(activeResponseButtons[i]);
            activeResponseButtons.RemoveAt(i);
        }
    }
}
