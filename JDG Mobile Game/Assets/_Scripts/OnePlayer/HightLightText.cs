using System.Collections;
using OnePlayer;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles the highlighting of text based on game events.
/// </summary>
public class HightLightText : MonoBehaviour
{
    /// <summary>
    /// The highlight element associated with this text.
    /// </summary>
    [SerializeField] private HighlightElement element;
    
    /// <summary>
    /// The duration of the pulse
    /// </summary>
    [SerializeField] private float pulseDuration = 0.5f;
        
    /// <summary>
    /// Flag to indicate if the highlight is activated.
    /// </summary>
    private bool isActivated;
    
    /// <summary>
    /// Flag to manage the pulse coroutine execution.
    /// </summary>
    private bool waitEndTurn = true;
    
    /// <summary>
    /// Cached reference to the TextMeshProUGUI component.
    /// </summary>
    private TextMeshProUGUI textMesh;
    
    /// <summary>
    /// Cache references on awake.
    /// </summary>
    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }
        
    /// <summary>
    /// Subscribe to relevant game events on start.
    /// </summary>
    private void Start()
    {
        HighLightPlane.Highlight.AddListener(UpdateStatus);
    }
    
    /// <summary>
    /// Unsubscribe from events when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        // Unsubscribe from the event when this object is destroyed
        HighLightPlane.Highlight.RemoveListener(UpdateStatus);
    }

    /// <summary>
    /// Updates the status based on game events.
    /// </summary>
    /// <param name="highlightElement">The received highlight element.</param>
    /// <param name="activated">Flag indicating if the highlight is activated.</param>
    private void UpdateStatus(HighlightElement highlightElement, bool activated)
    {
        if (highlightElement == element)
        {
            isActivated = activated;
        }
    }

    /// <summary>
    /// Handles the text update on each frame.
    /// </summary>
    private void Update()
    {
        if (isActivated)
        {
            if (waitEndTurn)
            {
                StartCoroutine(PulseCoroutine());
            }
        }
        else
        {
            SetTextColor(Color.white);
            waitEndTurn = true;
        }
    }
    
    /// <summary>
    /// Sets the text color.
    /// </summary>
    /// <param name="color">The desired color.</param>
    private void SetTextColor(Color color)
    {
        textMesh.color = color;
    }

    /// <summary>
    /// Coroutine for the pulse effect on the text.
    /// </summary>
    /// <returns>IEnumerator for the coroutine.</returns>
    private IEnumerator PulseCoroutine()
    {
        waitEndTurn = false;
        yield return new WaitForSeconds(pulseDuration);
        SetTextColor(Color.white);
        yield return new WaitForSeconds(pulseDuration);
        SetTextColor(Color.clear);
        waitEndTurn = true;
    }
}
