using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using JDG.Application;
using JDG.Application.Services;
using JDG.Domain.Events;
using UnityEngine;
using VContainer;

namespace Cards.EquipmentCards
{
    /// <summary>
    /// Handles the functionalities associated with equipment cards within the game.
    /// Phase 6: Refactored to delegate business logic to ICardPlacementService.
    /// Phase 24-25: Removed ServiceLocator, using VContainer DI.
    /// Phase 34: Uses ILocalizationService instead of LocalizationSystem.Instance.
    /// Phase 35: Uses IDialogService instead of CardSelector.Instance.
    /// Phase 36: Subscribes to EquipmentCardPlayRequestedEvent via EventBus.
    /// </summary>
    public class EquipmentFunctions : MonoBehaviour
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
        private IDisposable _equipmentPlayRequestedSubscription;

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
        /// Initializes listeners for equipment card events.
        /// Phase 36/109: Subscribes to EventBus only - static events removed.
        /// </summary>
        private void Start()
        {
            // Phase 36/109: Subscribe to EventBus
            _equipmentPlayRequestedSubscription = _eventBus.Subscribe<EquipmentCardPlayRequestedEvent>(OnEquipmentCardPlayRequested);
        }

        /// <summary>
        /// Cleans up listeners upon object destruction.
        /// Phase 36: Disposes EventBus subscriptions.
        /// </summary>
        private void OnDestroy()
        {
            _equipmentPlayRequestedSubscription?.Dispose();
        }

        /// <summary>
        /// Event handler for EquipmentCardPlayRequestedEvent from EventBus.
        /// Phase 36: Replaces static UnityEvent listener.
        /// </summary>
        private void OnEquipmentCardPlayRequested(EquipmentCardPlayRequestedEvent evt)
        {
            if (evt.EquipmentCard is InGameEquipmentCard equipmentCard)
            {
                DisplayEquipmentPopUp(equipmentCard);
            }
        }

        /// <summary>
        /// Displays a pop-up for equipping a card, showing invocations on which equipment can be added.
        /// Phase 6: Delegates business logic to CardPlacementService.
        /// Phase 35: Uses injected IDialogService instead of CardSelector.Instance.
        /// </summary>
        /// <param name="equipmentCard">The equipment card the player wishes to apply.</param>
        private void DisplayEquipmentPopUp(InGameEquipmentCard equipmentCard)
        {
            // Get valid targets from service
            var validTargets = _cardPlacementService.GetEquipmentTargets(equipmentCard);

            // Convert to List<object> for IDialogService
            var cardObjects = new List<object>();
            foreach (var card in validTargets) cardObjects.Add(card);

            // Display card selector UI
            // Phase 34: Use injected ILocalizationService
            // Phase 35: Use injected IDialogService instead of CardSelector.Instance
            var options = new CardSelectorOptions
            {
                Title = _localizationService.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_INVOCATION_FOR_EQUIPMENT),
                Cards = cardObjects,
                ShowNegativeButton = true,
                ShowPositiveButton = true,
                OnPositiveSingle = (card) =>
                {
                    if (card is InGameInvocationCard selectedInvocationCard)
                    {
                        // Delegate to service for business logic
                        _cardPlacementService.PlaceEquipmentCard(equipmentCard, selectedInvocationCard, canvas);

                        // Hide UI after placement
                        miniCardMenu.SetActive(false);
                    }
                },
                OnNegative = () =>
                {
                    // Hide UI on cancel
                    miniCardMenu.SetActive(false);
                }
            };

            _dialogService.ShowCardSelector(canvas, options);
        }
    }
}