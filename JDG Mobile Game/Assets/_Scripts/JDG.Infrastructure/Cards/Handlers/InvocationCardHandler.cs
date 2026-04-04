using JDG.Application;
using JDG.Application.Services;
using JDG.Core;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Infrastructure.Services;

namespace JDG.Infrastructure.Cards.Handlers
{
    /// <summary>
    /// Handler responsible for invocation card-specific behaviors in the game.
    /// Phase 166: Moved to JDG.Infrastructure. Uses ICardMenuView and ICardCollectionProvider.
    /// </summary>
    public class InvocationCardHandler : CardHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvocationCardHandler"/> class.
        /// Phase 166: Simplified constructor to match base class changes.
        /// </summary>
        /// <param name="menuView">The card menu view for UI interactions.</param>
        /// <param name="cardCollectionProvider">The provider for accessing player card collections.</param>
        /// <param name="localizationService">The service for localized text values.</param>
        /// <param name="eventBus">The event bus for publishing card events.</param>
        public InvocationCardHandler(
            ICardMenuView menuView,
            ICardCollectionProvider cardCollectionProvider,
            ILocalizationService localizationService,
            IEventBus eventBus)
            : base(menuView, cardCollectionProvider, localizationService, eventBus)
        {
        }

        /// <summary>
        /// Handles the card's behavior and updates the UI elements associated with an invocation card.
        /// Phase 166: Uses ICardMenuView and ICardCollectionProvider.
        /// </summary>
        /// <param name="card">The in-game card to be handled.</param>
        public override void HandleCard(InGameCard card)
        {
            var playerCard = cardCollectionProvider.GetCurrentPlayerCardCollection();
            menuView.SetPutCardButtonText(localizationService.GetLocalizedValue(LocalizationKeys.BUTTON_PUT_CARD));
            var invocationCard = card as InGameInvocationCard;
            menuView.SetPutCardButtonInteractable(
                invocationCard?.CanBeSummoned(playerCard) == true && playerCard.InvocationCards.Count < 4);
        }

        /// <summary>
        /// Phase 109: Publishes InvocationCardPlayRequestedEvent via EventBus only.
        /// Phase 166: Uses ICardCollectionProvider.
        /// </summary>
        /// <param name="card">The in-game card that is being placed.</param>
        public override void HandleCardPut(InGameCard card)
        {
            var invocationCard = card as InGameInvocationCard;

            // Phase 36/109: Publish via EventBus
            var playerCards = cardCollectionProvider.GetCurrentPlayerCardCollection();
            var owner = playerCards.IsPlayerOne ? CardOwner.Player1 : CardOwner.Player2;
            eventBus.Publish(new InvocationCardPlayRequestedEvent
            {
                InvocationCard = invocationCard,
                Owner = owner
            });
        }
    }
}
