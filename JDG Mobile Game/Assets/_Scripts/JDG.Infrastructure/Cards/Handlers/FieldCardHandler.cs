using JDG.Application;
using JDG.Application.Services;
using JDG.Core;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Infrastructure.Services;

namespace JDG.Infrastructure.Cards.Handlers
{
    /// <summary>
    /// Handler responsible for field card-specific behaviors in the game.
    /// Phase 166: Moved to JDG.Infrastructure. Uses ICardMenuView and ICardCollectionProvider.
    /// </summary>
    public class FieldCardHandler : CardHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldCardHandler"/> class.
        /// Phase 166: Simplified constructor to match base class changes.
        /// </summary>
        /// <param name="menuView">The card menu view for UI interactions.</param>
        /// <param name="cardCollectionProvider">The provider for accessing player card collections.</param>
        /// <param name="localizationService">The service for localized text values.</param>
        /// <param name="eventBus">The event bus for publishing card events.</param>
        public FieldCardHandler(
            ICardMenuView menuView,
            ICardCollectionProvider cardCollectionProvider,
            ILocalizationService localizationService,
            IEventBus eventBus)
            : base(menuView, cardCollectionProvider, localizationService, eventBus)
        {
        }

        /// <summary>
        /// Handles the card's behavior and updates the UI elements associated with a field card.
        /// Phase 166: Uses ICardMenuView and ICardCollectionProvider.
        /// </summary>
        /// <param name="card">The in-game card to be handled.</param>
        public override void HandleCard(InGameCard card)
        {
            var playerCard = cardCollectionProvider.GetCurrentPlayerCardCollection();
            menuView.SetPutCardButtonText(localizationService.GetLocalizedValue(LocalizationKeys.BUTTON_PUT_CARD));
            menuView.SetPutCardButtonInteractable(playerCard.FieldCard == null);
        }

        /// <summary>
        /// Handles the card placement behavior for field cards.
        /// Phase 109: Publishes FieldCardPlayRequestedEvent via EventBus only.
        /// Phase 166: Uses ICardCollectionProvider.
        /// </summary>
        /// <param name="card">The in-game card that is being placed.</param>
        public override void HandleCardPut(InGameCard card)
        {
            if (card is InGameFieldCard fieldCard)
            {
                // Phase 36/109: Publish via EventBus
                var playerCards = cardCollectionProvider.GetCurrentPlayerCardCollection();
                var owner = playerCards.IsPlayerOne ? CardOwner.Player1 : CardOwner.Player2;
                eventBus.Publish(new FieldCardPlayRequestedEvent
                {
                    FieldCard = fieldCard,
                    Owner = owner
                });
            }
        }
    }
}
