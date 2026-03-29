using System.Linq;
using JDG.Application;
using JDG.Application.Abilities;
using JDG.Application.Services;
using JDG.Core;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Services;

namespace JDG.Infrastructure.Cards.Handlers
{
    /// <summary>
    /// Handler responsible for effect card-specific behaviors in the game.
    /// Phase 114: Updated to use modern IAbility for ability checks.
    /// Phase 166: Moved to JDG.Infrastructure. Uses ICardMenuView and ICardCollectionProvider.
    /// </summary>
    public class EffectCardHandler : CardHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EffectCardHandler"/> class.
        /// Phase 166: Simplified constructor to match base class changes.
        /// </summary>
        /// <param name="menuView">The card menu view for UI interactions.</param>
        /// <param name="cardCollectionProvider">The provider for accessing player card collections.</param>
        /// <param name="localizationService">The service for localized text values.</param>
        /// <param name="eventBus">The event bus for publishing card events.</param>
        public EffectCardHandler(
            ICardMenuView menuView,
            ICardCollectionProvider cardCollectionProvider,
            ILocalizationService localizationService,
            IEventBus eventBus)
            : base(menuView, cardCollectionProvider, localizationService, eventBus)
        {
        }

        /// <summary>
        /// Handles the card's behavior and updates the UI elements associated with an effect card.
        /// Phase 114: Updated to use modern IAbility.CanActivate() instead of legacy CanUseEffect().
        /// Phase 166: Uses ICardMenuView and ICardCollectionProvider.
        /// </summary>
        /// <param name="card">The in-game card to be handled.</param>
        public override void HandleCard(InGameCard card)
        {
            var playerCard = cardCollectionProvider.GetCurrentPlayerCardCollection();
            var effectCard = card as InGameEffectCard;
            menuView.SetPutCardButtonText(localizationService.GetLocalizedValue(LocalizationKeys.BUTTON_PUT_CARD));

            // Phase 114: Check if all modern abilities can activate
            bool canPutCard = effectCard != null &&
                              playerCard.EffectCards.Count < 4 &&
                              CanAllAbilitiesActivate(effectCard, playerCard.IsPlayerOne);

            menuView.SetPutCardButtonInteractable(canPutCard);
        }

        /// <summary>
        /// Checks if all modern abilities on the effect card can activate.
        /// Phase 114: Helper method for modern ability check.
        /// </summary>
        private bool CanAllAbilitiesActivate(InGameEffectCard effectCard, bool isPlayerOne)
        {
            if (!effectCard.ModernEffectAbilities.Any())
                return true;

            // Create a minimal AbilityContext for the check
            var owner = isPlayerOne ? CardOwner.Player1 : CardOwner.Player2;
            var ownerId = PlayerId.FromCardOwner(owner);
            var opponentOwner = isPlayerOne ? CardOwner.Player2 : CardOwner.Player1;
            var opponentId = PlayerId.FromCardOwner(opponentOwner);
            var context = new AbilityContext(ownerId, opponentId, null, AbilityName.Default);

            return effectCard.ModernEffectAbilities.All(ability => ability.CanActivate(context));
        }

        /// <summary>
        /// Handles the card placement behavior for effect cards.
        /// Phase 109: Publishes EffectCardPlayRequestedEvent via EventBus only.
        /// Phase 166: Uses ICardCollectionProvider.
        /// </summary>
        /// <param name="card">The in-game card that is being placed.</param>
        public override void HandleCardPut(InGameCard card)
        {
            if (card is InGameEffectCard effectCard)
            {
                // Phase 36/109: Publish via EventBus
                var playerCards = cardCollectionProvider.GetCurrentPlayerCardCollection();
                var owner = playerCards.IsPlayerOne ? CardOwner.Player1 : CardOwner.Player2;
                eventBus.Publish(new EffectCardPlayRequestedEvent
                {
                    EffectCard = effectCard,
                    Owner = owner
                });
            }
        }
    }
}
