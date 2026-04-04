using System;
using System.Collections;
using JDG.Application;
using JDG.Domain.Events;
using UnityEngine;
using VContainer;

namespace OnePlayer
{
    /// <summary>
    /// Enum representing different highlight elements.
    /// Phase 143: Added AttackButton and OpponentSelector for multi-step attack flow.
    /// </summary>
    public enum HighlightElement
    {
        Invocations, Space, Deck, YellowTrash, Effect, Field, InHandButton, NextPhaseButton, Tentacules, LifePoints,
        AttackButton,       // Phase 143: For highlighting attack button
        OpponentSelector    // Phase 143: For highlighting card in opponent selector
    }

    /// <summary>
    /// Component responsible for handling the visual highlighting of certain game elements.
    /// Phase 122: Removed static HighlightEvent - now uses EventBus.
    /// </summary>
    public class HighLightPlane : MonoBehaviour
    {
        [SerializeField] private HighlightElement element;

        private const float PulseDuration = 0.5f;
        private static readonly Color PulseColor = Color.green;

        private bool isActivated;
        private bool waitEndTurn = true;

        private MeshRenderer meshRenderer;

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
        /// Initialize component references.
        /// </summary>
        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        /// <summary>
        /// Set up event listeners when the component starts.
        /// Phase 122: Subscribe to HighlightRequestedEvent via EventBus.
        /// </summary>
        private void Start()
        {
            _highlightSubscription = _eventBus?.Subscribe<HighlightRequestedEvent>(OnHighlightRequested);
        }

        /// <summary>
        /// Ensure event listeners are cleaned up when the component is destroyed.
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
        /// Updates the activation status of the highlight.
        /// </summary>
        /// <param name="highlightElement">The element to check.</param>
        /// <param name="activated">Whether the highlight is activated or not.</param>
        private void UpdateStatus(HighlightElement highlightElement, bool activated)
        {
            if (highlightElement == element)
            {
                isActivated = activated;
            }
        }

        /// <summary>
        /// Handles the visual update of the highlight effect every frame.
        /// </summary>
        void Update()
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
                meshRenderer.material.color = Color.clear;
                waitEndTurn = true;
            }
        }

        /// <summary>
        /// Coroutine that manages the pulsing highlight effect.
        /// </summary>
        /// <returns>An IEnumerator to be used in a Coroutine.</returns>
        private IEnumerator PulseCoroutine()
        {
            waitEndTurn = false;
            yield return new WaitForSeconds(PulseDuration);
            meshRenderer.material.color = PulseColor;
            yield return new WaitForSeconds(PulseDuration);
            meshRenderer.material.color = Color.clear;
            waitEndTurn = true;
        }
    }
}