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
    /// Phase 35: Uses IDialogService instead of MessageBox.Instance.
    /// Phase 36: Subscribes to InvocationCardPlayRequestedEvent via EventBus.
    /// </summary>
    public class InvocationFunctions : MonoBehaviour
    {
        [SerializeField] protected Transform canvas;

        // Phase 24-25: Injected via VContainer (protected so TutoInvocationFunctions can access)
        protected ICardPlacementService _cardPlacementService;

        // Phase 23: EventBus for static UnityEvent migration
        private IEventBus _eventBus;
        private IDisposable _invocationCancelledSubscription;
        // Phase 36: EventBus subscription for card play request
        private IDisposable _invocationPlayRequestedSubscription;

        // Phase 34: ILocalizationService (protected so TutoInvocationFunctions can access)
        protected ILocalizationService _localizationService;

        // Phase 35: IDialogService (protected so TutoInvocationFunctions can access)
        protected IDialogService _dialogService;

        /// <summary>
        /// VContainer method injection for dependencies.
        /// Phase 23: Inject IEventBus for static UnityEvent migration.
        /// Phase 24-25: Inject ICardPlacementService instead of ServiceLocator.
        /// Phase 34: Inject ILocalizationService instead of LocalizationSystem.Instance.
        /// Phase 35: Inject IDialogService instead of MessageBox.Instance.
        /// </summary>
        [Inject]
        public void Construct(
            ICardPlacementService cardPlacementService,
            IEventBus eventBus,
            ILocalizationService localizationService,
            IDialogService dialogService)
        {
            _cardPlacementService = cardPlacementService;
            _eventBus = eventBus;
            _localizationService = localizationService;
            _dialogService = dialogService;
        }


        /// <summary>
        /// Sets up the initial state and event listeners.
        /// Phase 23: Subscribes to EventBus events.
        /// Phase 36: Subscribes to InvocationCardPlayRequestedEvent via EventBus.
        /// </summary>
        private void Start()
        {
            // Phase 36: Subscribe to EventBus (primary)
            _invocationPlayRequestedSubscription = _eventBus.Subscribe<InvocationCardPlayRequestedEvent>(OnInvocationCardPlayRequested);
            _invocationCancelledSubscription = _eventBus.Subscribe<InvocationCancelledEvent>(OnInvocationCancelled);

            // Keep static event listener during migration (will be removed once EventBus is fully adopted)
            // Note: Static events still fire during dual-dispatch period but we handle via EventBus now
        }

        /// <summary>
        /// Cleanup method. Unsubscribes from events when the object is destroyed.
        /// Phase 23: Disposes EventBus subscriptions.
        /// Phase 36: Disposes InvocationCardPlayRequestedEvent subscription.
        /// </summary>
        private void OnDestroy()
        {
            _invocationCancelledSubscription?.Dispose();
            _invocationPlayRequestedSubscription?.Dispose();
        }

        /// <summary>
        /// Event handler for InvocationCardPlayRequestedEvent from EventBus.
        /// Phase 36: Replaces static UnityEvent listener.
        /// </summary>
        private void OnInvocationCardPlayRequested(InvocationCardPlayRequestedEvent evt)
        {
            if (evt.InvocationCard is InGameInvocationCard invocationCard)
            {
                PutInvocationCard(invocationCard);
            }
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
        /// Phase 35: Uses IDialogService instead of MessageBox.Instance.
        /// </summary>
        /// <param name="invocationCard">The invocation card to place on the field.</param>
        private void PutInvocationCard(InGameInvocationCard invocationCard)
        {
            bool success = _cardPlacementService.PlaceInvocationCard(invocationCard, canvas);

            if (!success)
            {
                // Show warning if field is full (4 invocations max)
                // Phase 34: Use injected ILocalizationService
                // Phase 35: Use injected IDialogService instead of MessageBox.Instance
                _dialogService.ShowMessageBox(canvas, new MessageBoxOptions
                {
                    Title = _localizationService.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                    Message = _localizationService.GetLocalizedValue(LocalizationKeys.WARNING_LIMIT_NUMBER_CARDS),
                    ShowOkButton = true
                });
            }
        }
    }
}