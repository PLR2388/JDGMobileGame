using JDG.Application;
using JDG.Application.Services;
using JDG.Core;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Infrastructure.Services;

namespace JDG.Infrastructure.Cards.Handlers
{
    /// <summary>
    /// Handler responsible for contre card-specific behaviors in the game.
    /// Phase 166: Moved to JDG.Infrastructure. Uses ICardMenuView and ICardCollectionProvider.
    /// </summary>
    public class ContreCardHandler : CardHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContreCardHandler"/> class.
        /// Phase 166: Simplified constructor to match base class changes.
        /// </summary>
        /// <param name="menuView">The card menu view for UI interactions.</param>
        /// <param name="cardCollectionProvider">The provider for accessing player card collections.</param>
        /// <param name="localizationService">The service for localized text values.</param>
        /// <param name="eventBus">The event bus for publishing card events.</param>
        public ContreCardHandler(
            ICardMenuView menuView,
            ICardCollectionProvider cardCollectionProvider,
            ILocalizationService localizationService,
            IEventBus eventBus)
            : base(menuView, cardCollectionProvider, localizationService, eventBus)
        {
        }

        /// <summary>
        /// Handles the card's behavior and updates the UI elements associated with a contre card.
        /// Phase 135: Added game state validation.
        /// Phase 166: Uses ICardMenuView abstraction.
        /// </summary>
        /// <param name="card">The in-game card to be handled.</param>
        public override void HandleCard(InGameCard card)
        {
            // Phase 135: Validate game state before enabling button
            if (!menuView.CanInteractWithCards())
            {
                menuView.SetPutCardButtonInteractable(false);
                return;
            }

            menuView.SetPutCardButtonText(localizationService.GetLocalizedValue(LocalizationKeys.BUTTON_CONTRE));
            menuView.SetPutCardButtonInteractable(true);
        }

        /// <summary>
        /// Handles the card placement behavior for contre cards.
        /// Contre cards have immediate effect and are discarded after use.
        /// Phase 135: Added game state validation.
        /// Phase 166: Uses ICardMenuView and ICardCollectionProvider.
        /// </summary>
        /// <param name="card">The in-game card that is being placed.</param>
        public override void HandleCardPut(InGameCard card)
        {
            if (card == null) return;

            // Phase 135: Validate game state before placing card
            if (!menuView.CanPlaceCards())
            {
#if UNITY_EDITOR
                UnityEngine.Debug.Log("ContreCardHandler: Cannot place contre card - wrong phase or game over");
#endif
                return;
            }

            // Get the owner of the card
            var playerCards = cardCollectionProvider.GetCurrentPlayerCardCollection();
            var owner = playerCards.IsPlayerOne ? CardOwner.Player1 : CardOwner.Player2;

            // Publish the contre card play request event
            eventBus.Publish(new ContreCardPlayRequestedEvent
            {
                ContreCard = card,
                Owner = owner
            });

            // Note: The actual contre effect execution and discarding
            // should be handled by a subscriber to ContreCardPlayRequestedEvent
            // (e.g., a ContreCardService or the game loop)
        }
    }
}
