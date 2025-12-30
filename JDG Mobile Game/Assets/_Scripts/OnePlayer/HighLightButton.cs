using System;
using System.Collections;
using JDG.Application;
using JDG.Domain.Events;
using OnePlayer;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

/// <summary>
/// Provides functionality to highlight a button based on certain criteria and conditions.
/// Phase 122: Migrated from static UnityEvent to EventBus.
/// </summary>
public class HighLightButton : MonoBehaviour
{
    [Tooltip("Element associated with this button")]
    [SerializeField] private HighlightElement element;

    [Tooltip("Color to be used when pulsing")]
    [SerializeField] private Color pulseColor = Color.green;

    [Tooltip("Default color for the button")]
    [SerializeField] private Color defaultColor = Color.white;

    [Tooltip("Duration of a single pulse in seconds")]
    [SerializeField] private float pulseDuration = 0.5f;

    /// <summary>
    /// Flag indicating whether the button should be highlighted.
    /// </summary>
    public bool isActivated = false;
    private bool waitEndTurn = true;

    private Button buttonComponent;
    private Image imageComponent;

    // Phase 122: EventBus subscription
    private IEventBus _eventBus;
    private IDisposable _highlightSubscription;

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 122: Added EventBus for highlight events.
    /// </summary>
    [Inject]
    public void Construct(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    /// <summary>
    /// Initialization of component references.
    /// </summary>
    private void Awake()
    {
        buttonComponent = GetComponent<Button>();
        imageComponent = GetComponent<Image>();
    }

    /// <summary>
    /// Subscribes to highlight events when the script starts.
    /// Phase 122: Subscribe via EventBus.
    /// </summary>
    private void Start()
    {
        _highlightSubscription = _eventBus?.Subscribe<HighlightRequestedEvent>(OnHighlightRequested);
    }

    /// <summary>
    /// Cleanup when destroyed.
    /// Phase 122: Dispose EventBus subscription.
    /// </summary>
    private void OnDestroy()
    {
        _highlightSubscription?.Dispose();
    }

    /// <summary>
    /// Handles HighlightRequestedEvent from EventBus.
    /// Phase 122: Replaces static UnityEvent listener.
    /// </summary>
    private void OnHighlightRequested(HighlightRequestedEvent evt)
    {
        UpdateStatus((HighlightElement)evt.Element, evt.IsActivated);
    }

    /// <summary>
    /// Updates the activation status based on the received highlight element.
    /// </summary>
    /// <param name="highlightElement">The element to check.</param>
    /// <param name="activated">Activation status.</param>
    private void UpdateStatus(HighlightElement highlightElement, bool activated)
    {
        if (highlightElement == element)
        {
            isActivated = activated;
        }
    }

    /// <summary>
    /// Handles button interaction and pulsing effect each frame.
    /// </summary>
    private void Update()
    {
        if (isActivated)
        {
            buttonComponent.interactable = true;
            if (waitEndTurn)
            {
                StartCoroutine(PulseCoroutine());
            }
        }
        else
        {
           imageComponent.color = Color.white;
            waitEndTurn = true;
        }
    }
    
    /// <summary>
    /// Coroutine that gives a pulsing effect to the button.
    /// </summary>
    /// <returns>Yield instruction.</returns>
    private IEnumerator PulseCoroutine()
    {
        waitEndTurn = false;
        yield return new WaitForSeconds(pulseDuration);
        imageComponent.color = pulseColor;
        yield return new WaitForSeconds(pulseDuration);
        imageComponent.color = defaultColor;
        waitEndTurn = true;
    }
}
