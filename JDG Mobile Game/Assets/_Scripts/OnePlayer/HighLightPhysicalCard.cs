using System;
using System.Collections;
using JDG.Application;
using JDG.Domain.Events;
using UnityEngine;
using VContainer;

namespace OnePlayer
{
    /// <summary>
    /// Provides functionality to highlight a physical card in the game.
    /// Phase 122: Migrated from static UnityEvent to EventBus.
    /// </summary>
    public class HighLightPhysicalCard : MonoBehaviour
    {
        [SerializeField] private HighlightElement element;
        [SerializeField] private Color pulseColor = Color.green;
        [SerializeField] private Color defaultColor = Color.white;
        [SerializeField] private float pulseDuration = 0.5f;


        private static readonly string TargetCardName = CardNameMappings.CardNameMap[CardNames.Tentacules];

        private bool isActivated = false;
        private bool waitEndTurn = true;

        private MeshRenderer meshRenderer;
        private PhysicalCardDisplay cardDisplay;

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
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            cardDisplay = GetComponent<PhysicalCardDisplay>();
        }

        /// <summary>
        /// Start is called before the first frame update.
        /// Phase 122: Subscribe via EventBus.
        /// </summary>
        private void Start()
        {
            _highlightSubscription = _eventBus?.Subscribe<HighlightRequestedEvent>(OnHighlightRequested);
        }

        /// <summary>
        /// Unsubscribe from events when this object is being destroyed.
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
        /// <param name="highlightElement">The element to be checked.</param>
        /// <param name="activated">Whether the element is activated.</param>
        private void UpdateStatus(HighlightElement highlightElement, bool activated)
        {
            if (highlightElement == element && cardDisplay.Card.Title == TargetCardName)
            {
                isActivated = activated;
            }
        }

        /// <summary>
        /// Update is called once per frame.
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
                meshRenderer.material.color = defaultColor;
                waitEndTurn = true;
            }
        }
        
        /// <summary>
        /// Coroutine to handle the pulsing effect on the card.
        /// </summary>
        private IEnumerator PulseCoroutine()
        {
            waitEndTurn = false;
            yield return new WaitForSeconds(pulseDuration);
            meshRenderer.material.color = pulseColor;
            yield return new WaitForSeconds(pulseDuration);
            meshRenderer.material.color = defaultColor;
            waitEndTurn = true;
        }
    }
}