using System;
using JDG.Application;
using JDG.Domain.Events;
using JDG.Infrastructure.Cards;
using Sound;
using UnityEngine;
using VContainer;
using DomainCardOwner = JDG.Domain.CardOwner;
using DomainCardType = JDG.Domain.Enums.CardType;

namespace Cards.FieldCards
{
    /// <summary>
    /// Provides functionalities related to field cards in the game, such as placing a card on the field.
    /// Phase 6: Refactored to delegate business logic to ICardPlacementService.
    /// Phase 24-25: Removed ServiceLocator, using VContainer DI.
    /// Phase 36: Subscribes to FieldCardPlayRequestedEvent via EventBus.
    /// </summary>
    public class FieldFunctions : MonoBehaviour
    {
        [SerializeField] private GameObject miniCardMenu; // The UI component representing a mini card menu.

        // Phase 24-25: Injected via VContainer
        private ICardPlacementService _cardPlacementService;

        // Phase 36: EventBus for static UnityEvent migration
        private IEventBus _eventBus;
        private IDisposable _fieldPlayRequestedSubscription;

        /// <summary>
        /// Phase 156: Added null checks for injected dependencies.
        /// </summary>
        [Inject]
        public void Construct(ICardPlacementService cardPlacementService, IEventBus eventBus)
        {
            _cardPlacementService = cardPlacementService ?? throw new System.ArgumentNullException(
                nameof(cardPlacementService), "FieldFunctions requires ICardPlacementService");
            _eventBus = eventBus ?? throw new System.ArgumentNullException(
                nameof(eventBus), "FieldFunctions requires IEventBus for event subscriptions");
        }

        /// <summary>
        /// Initialization method that sets up listeners for relevant events.
        /// Phase 36/109: Subscribes to EventBus only - static events removed.
        /// </summary>
        private void Start()
        {
            // Phase 36/109: Subscribe to EventBus
            _fieldPlayRequestedSubscription = _eventBus.Subscribe<FieldCardPlayRequestedEvent>(OnFieldCardPlayRequested);
        }

        /// <summary>
        /// Called when the object is being destroyed and unsubscribes from events.
        /// Phase 36: Disposes EventBus subscriptions.
        /// </summary>
        private void OnDestroy()
        {
            _fieldPlayRequestedSubscription?.Dispose();
        }

        /// <summary>
        /// Event handler for FieldCardPlayRequestedEvent from EventBus.
        /// Phase 36: Replaces static UnityEvent listener.
        /// </summary>
        private void OnFieldCardPlayRequested(FieldCardPlayRequestedEvent evt)
        {
            if (evt.FieldCard is InGameFieldCard fieldCard)
            {
                PutFieldCard(fieldCard);
            }
        }

        /// <summary>
        /// Places a field card onto the game field and applies its associated effects.
        /// Phase 6: Delegates to CardPlacementService.
        /// Phase 144: Publishes CardPlayedEvent for ability triggers.
        /// </summary>
        /// <param name="fieldCard">The field card to be placed on the field.</param>
        private void PutFieldCard(InGameFieldCard fieldCard)
        {
            bool success = _cardPlacementService.PlaceFieldCard(fieldCard);

            if (success)
            {
                // Hide mini card menu on successful placement
                miniCardMenu.SetActive(false);

                // Phase 144: Publish CardPlayedEvent to trigger abilities
                _eventBus.Publish(new CardPlayedEvent
                {
                    CardId = Guid.NewGuid(),
                    Owner = (DomainCardOwner)(int)fieldCard.CardOwner,
                    CardType = DomainCardType.Field,
                    CardTitle = fieldCard.Title
                });
            }
            // Note: No warning shown for field cards - original logic just returns silently
        }
    }
}