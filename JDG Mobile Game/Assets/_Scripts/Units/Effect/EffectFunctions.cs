using System;
using JDG.Application;
using JDG.Application.Services;
using JDG.Domain.Events;
using UnityEngine;
using VContainer;

namespace Cards.EffectCards
{
    /// <summary>
    /// Handles the functionality related to effect cards within the game.
    /// Phase 6: Refactored to delegate business logic to ICardPlacementService.
    /// Phase 24-25: Removed ServiceLocator, using VContainer DI.
    /// Phase 34: Uses ILocalizationService instead of LocalizationSystem.Instance.
    /// Phase 35: Uses IDialogService instead of MessageBox.Instance.
    /// Phase 36: Subscribes to EffectCardPlayRequestedEvent via EventBus.
    /// </summary>
    public class EffectFunctions : MonoBehaviour
    {
        [SerializeField] private GameObject miniCardMenu;
        [SerializeField] private Transform canvas;

        // Phase 24-25: Injected via VContainer
        private ICardPlacementService _cardPlacementService;

        // Phase 34: Injected via VContainer
        private ILocalizationService _localizationService;

        // Phase 35: Injected via VContainer
        private IDialogService _dialogService;

        // Phase 36: EventBus for static UnityEvent migration
        private IEventBus _eventBus;
        private IDisposable _effectPlayRequestedSubscription;

        [Inject]
        public void Construct(
            ICardPlacementService cardPlacementService,
            ILocalizationService localizationService,
            IDialogService dialogService,
            IEventBus eventBus)
        {
            _cardPlacementService = cardPlacementService;
            _localizationService = localizationService;
            _dialogService = dialogService;
            _eventBus = eventBus;
        }

        /// <summary>
        /// Initialization method. Subscribes to relevant events.
        /// Phase 36/109: Subscribes to EventBus only - static events removed.
        /// </summary>
        private void Start()
        {
            // Phase 36/109: Subscribe to EventBus
            _effectPlayRequestedSubscription = _eventBus.Subscribe<EffectCardPlayRequestedEvent>(OnEffectCardPlayRequested);
        }

        /// <summary>
        /// Cleanup method. Unsubscribes from events when the object is destroyed.
        /// Phase 36: Disposes EventBus subscriptions.
        /// </summary>
        private void OnDestroy()
        {
            _effectPlayRequestedSubscription?.Dispose();
        }

        /// <summary>
        /// Event handler for EffectCardPlayRequestedEvent from EventBus.
        /// Phase 36: Replaces static UnityEvent listener.
        /// </summary>
        private void OnEffectCardPlayRequested(EffectCardPlayRequestedEvent evt)
        {
            if (evt.EffectCard is InGameEffectCard effectCard)
            {
                PutEffectCard(effectCard);
            }
        }

        /// <summary>
        /// Processes the placement of an effect card on the field. If there are less than 4 effect cards on the field, it applies the card's effects.
        /// Otherwise, a warning is shown to the player.
        /// Phase 6: Delegates to CardPlacementService.
        /// Phase 35: Uses IDialogService instead of MessageBox.Instance.
        /// </summary>
        /// <param name="effectCard">The effect card the user put on the field.</param>
        private void PutEffectCard(InGameEffectCard effectCard)
        {
            bool success = _cardPlacementService.PlaceEffectCard(effectCard, canvas);

            if (success)
            {
                // Hide mini card menu on successful placement
                miniCardMenu.SetActive(false);
            }
            else
            {
                // Show warning if field is full (4 effects max)
                // Phase 34: Use injected ILocalizationService
                // Phase 35: Use injected IDialogService instead of MessageBox.Instance
                _dialogService.ShowMessageBox(canvas, new MessageBoxOptions
                {
                    Title = _localizationService.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                    Message = _localizationService.GetLocalizedValue(LocalizationKeys.WARNING_LIMIT_EFFECT_CARDS),
                    ShowOkButton = true
                });
            }
        }
    }
}