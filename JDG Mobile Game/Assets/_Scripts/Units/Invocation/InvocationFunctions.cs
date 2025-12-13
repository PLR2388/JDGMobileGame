using System;
using _Scripts.Units.Invocation;
using JDG.Application;
using JDG.Application.Services;
using JDG.Domain.Events;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace _Scripts.Cards.InvocationCards
{
    /// <summary>
    /// Handles the operations related to invocation cards, including placing them on the field,
    /// canceling their effects, and more.
    /// Phase 6: Refactored to delegate business logic to ICardPlacementService.
    /// Phase 23: Migrated static UnityEvent to EventBus (CancelInvocationEvent).
    /// Phase 24-25: Removed ServiceLocator, using VContainer DI.
    /// Phase 34: Uses ILocalizationService instead of LocalizationSystem.Instance.
    /// </summary>
    public class InvocationFunctions : MonoBehaviour
    {
        [SerializeField] protected Transform canvas;

        // Phase 24-25: Injected via VContainer (protected so TutoInvocationFunctions can access)
        protected ICardPlacementService _cardPlacementService;

        // Phase 23: EventBus for static UnityEvent migration
        private IEventBus _eventBus;
        private IDisposable _invocationCancelledSubscription;

        // Phase 34: ILocalizationService (protected so TutoInvocationFunctions can access)
        protected ILocalizationService _localizationService;

        /// <summary>
        /// VContainer method injection for dependencies.
        /// Phase 23: Inject IEventBus for static UnityEvent migration.
        /// Phase 24-25: Inject ICardPlacementService instead of ServiceLocator.
        /// Phase 34: Inject ILocalizationService instead of LocalizationSystem.Instance.
        /// </summary>
        [Inject]
        public void Construct(
            ICardPlacementService cardPlacementService,
            IEventBus eventBus,
            ILocalizationService localizationService)
        {
            _cardPlacementService = cardPlacementService;
            _eventBus = eventBus;
            _localizationService = localizationService;
        }


        /// <summary>
        /// Sets up the initial state and event listeners.
        /// Phase 23: Subscribes to EventBus events.
        /// </summary>
        private void Start()
        {
            // Attach listeners
            AttachInvocationEventListeners();
            _invocationCancelledSubscription = _eventBus.Subscribe<InvocationCancelledEvent>(OnInvocationCancelled);
        }

        /// <summary>
        /// Cleanup method. Unsubscribes from events when the object is destroyed.
        /// Phase 23: Disposes EventBus subscriptions.
        /// </summary>
        private void OnDestroy()
        {
            _invocationCancelledSubscription?.Dispose();
        }


        /// <summary>
        /// Attaches listeners for invocation events.
        /// </summary>
        private void AttachInvocationEventListeners()
        {
            InGameMenuScript.InvocationCardEvent.AddListener(PutInvocationCard);
        }

        /// <summary>
        /// Event handler for InvocationCancelledEvent from EventBus.
        /// Phase 23: Replaces static UnityEvent listener.
        /// </summary>
        private void OnInvocationCancelled(InvocationCancelledEvent evt)
        {
            if (evt.CancelledCard is InGameInvocationCard invocationCard)
            {
                _cardPlacementService.HandleInvocationCancelEffect(invocationCard);
            }
        }

        /// <summary>
        /// Places the invocation card on the field and applies its effect.
        /// Phase 6: Delegates to CardPlacementService.
        /// </summary>
        /// <param name="invocationCard">The invocation card to place on the field.</param>
        private void PutInvocationCard(InGameInvocationCard invocationCard)
        {
            bool success = _cardPlacementService.PlaceInvocationCard(invocationCard, canvas);

            if (!success)
            {
                // Show warning if field is full (4 invocations max)
                // Phase 34: Use injected ILocalizationService
                var config = new MessageBoxConfig(
                    _localizationService.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                    _localizationService.GetLocalizedValue(LocalizationKeys.WARNING_LIMIT_NUMBER_CARDS),
                    showOkButton: true
                );
                MessageBox.Instance.CreateMessageBox(canvas, config);
            }
        }
    }
}