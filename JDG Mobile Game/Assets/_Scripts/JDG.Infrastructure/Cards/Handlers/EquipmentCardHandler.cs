using System.Linq;
using JDG.Application;
using JDG.Application.Services;
using JDG.Core;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Infrastructure.Services;

namespace JDG.Infrastructure.Cards.Handlers
{
    /// <summary>
    /// Handler responsible for equipment card-specific behaviors in the game.
    /// Phase 117: Uses CanAlwaysBePlaced property instead of iterating legacy abilities.
    /// Phase 166: Moved to JDG.Infrastructure. Uses ICardMenuView and ICardCollectionProvider.
    /// </summary>
    public class EquipmentCardHandler : CardHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentCardHandler"/> class.
        /// Phase 166: Simplified constructor to match base class changes.
        /// </summary>
        /// <param name="menuView">The card menu view for UI interactions.</param>
        /// <param name="cardCollectionProvider">The provider for accessing player card collections.</param>
        /// <param name="localizationService">The service for localized text values.</param>
        /// <param name="eventBus">The event bus for publishing card events.</param>
        public EquipmentCardHandler(
            ICardMenuView menuView,
            ICardCollectionProvider cardCollectionProvider,
            ILocalizationService localizationService,
            IEventBus eventBus)
            : base(menuView, cardCollectionProvider, localizationService, eventBus)
        {
        }

        /// <summary>
        /// Handles the card's behavior and updates the UI elements associated with an equipment card.
        /// Phase 117: Uses CanAlwaysBePlaced property instead of iterating legacy abilities.
        /// Phase 166: Uses ICardMenuView and ICardCollectionProvider.
        /// </summary>
        /// <param name="card">The in-game card to be handled.</param>
        public override void HandleCard(InGameCard card)
        {
            var playerCard = cardCollectionProvider.GetCurrentPlayerCardCollection();
            var opponentPlayerCard = cardCollectionProvider.GetOpponentPlayerCardCollection();
            menuView.SetPutCardButtonText(localizationService.GetLocalizedValue(LocalizationKeys.BUTTON_EQUIP_INVOCATION));
            var equipmentCard = card as InGameEquipmentCard;
            // Phase 117: Use CanAlwaysBePlaced property instead of legacy ability iteration
            menuView.SetPutCardButtonInteractable(
                playerCard.InvocationCards.Count(inGameInvocationCard =>
                    inGameInvocationCard.EquipmentCard == null) > 0 ||
                opponentPlayerCard.InvocationCards.Count(inGameInvocationCard =>
                    inGameInvocationCard.EquipmentCard == null) > 0 ||
                equipmentCard?.CanAlwaysBePlaced == true
            );
        }

        /// <summary>
        /// Handles the card placement behavior for equipment cards.
        /// Phase 109: Publishes EquipmentCardPlayRequestedEvent via EventBus only.
        /// Phase 166: Uses ICardCollectionProvider.
        /// </summary>
        /// <param name="card">The in-game card that is being placed.</param>
        public override void HandleCardPut(InGameCard card)
        {
            if (card is InGameEquipmentCard equipmentCard)
            {
                // Phase 36/109: Publish via EventBus
                var playerCards = cardCollectionProvider.GetCurrentPlayerCardCollection();
                var owner = playerCards.IsPlayerOne ? CardOwner.Player1 : CardOwner.Player2;
                eventBus.Publish(new EquipmentCardPlayRequestedEvent
                {
                    EquipmentCard = equipmentCard,
                    Owner = owner
                });
            }
        }
    }
}
