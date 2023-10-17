using System.Collections;
using OnePlayer;
using UnityEngine;
using UnityEngine.UI;

public class HighLightCard : MonoBehaviour
{
    [SerializeField] [Tooltip("Type of element to be highlighted.")]
    private HighlightElement element;
    [SerializeField] [Tooltip("Color to be displayed during the pulse effect.")]
    private Color pulseColor = Color.green;
    [SerializeField] [Tooltip("Default color of the card when not highlighted.")]
    private Color defaultColor = Color.clear;
    [SerializeField] [Tooltip("Duration for each pulse of the highlighting effect.")]
    private float pulseDuration = 0.5f;

    private bool isActivated = true;
    private bool waitEndTurn = true;

    private Image cardImage;

    /// <summary>
    /// Cache frequently used components.
    /// </summary>
    private void Awake()
    {
        cardImage = GetComponent<Image>();
    }
    
    /// <summary>
    /// Initialization logic for the card. Subscribes to the Highlight event.
    /// </summary>
    private void Start()
    {
        HighLightPlane.Highlight.AddListener(UpdateStatus);
    }

    /// <summary>
    /// Ensure we unsubscribe from events to avoid potential memory leaks.
    /// </summary>
    private void OnDestroy()
    {
        HighLightPlane.Highlight.RemoveListener(UpdateStatus);
    }

    /// <summary>
    /// Updates the activation status of the card's highlight effect.
    /// </summary>
    /// <param name="highlightElement">The type of element being checked.</param>
    /// <param name="activated">Whether or not the highlight effect should be active.</param>
    private void UpdateStatus(HighlightElement highlightElement, bool activated)
    {
        if (highlightElement == element)
        {
            isActivated = activated;
        }
    }

    /// <summary>
    /// Checks the card's highlight status each frame and updates its visual appearance accordingly.
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
            cardImage.color = Color.clear;
            waitEndTurn = true;
        }
    }

    /// <summary>
    /// Coroutine for handling the pulsing effect of the card highlight.
    /// </summary>
    /// <returns>Yields between color changes for the pulse effect.</returns>
    private IEnumerator PulseCoroutine()
    {
        waitEndTurn = false;
        yield return new WaitForSeconds(pulseDuration);
        cardImage.color = pulseColor;
        yield return new WaitForSeconds(pulseDuration);
        cardImage.color = defaultColor;
        waitEndTurn = true;
    }
}