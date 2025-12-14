using System.Linq;
using Cards;
using Cards.EffectCards;
using JDG.Application;
using JDG.Application.Services;
using JDG.Domain;
using JDG.Domain.Events;

/// <summary>
/// Handler responsible for effect card-specific behaviors in the game.
/// Phase 17-18: Removed CardManager singleton dependency via ICardCollectionService.
/// Phase 28: Uses IPlayerStatusProvider instead of PlayerManager.Instance.
/// Phase 34: Uses ILocalizationService instead of LocalizationSystem.Instance.
/// Phase 36: Uses IEventBus for static UnityEvent migration.
/// </summary>
public class EffectCardHandler : CardHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EffectCardHandler"/> class.
    /// Phase 17-18: Added cardCollectionService parameter.
    /// Phase 28: Added playerStatusProvider parameter.
    /// Phase 34: Added localizationService parameter.
    /// Phase 36: Added eventBus parameter.
    /// </summary>
    /// <param name="menuScript">The in-game menu script associated with this handler.</param>
    /// <param name="cardCollectionService">The service for accessing player card collections.</param>
    /// <param name="playerStatusProvider">The provider for accessing player status.</param>
    /// <param name="localizationService">The service for localized text values.</param>
    /// <param name="eventBus">The event bus for publishing card events.</param>
    public EffectCardHandler(
        InGameMenuScript menuScript,
        ICardCollectionService cardCollectionService,
        IPlayerStatusProvider playerStatusProvider,
        ILocalizationService localizationService,
        IEventBus eventBus)
        : base(menuScript, cardCollectionService, playerStatusProvider, localizationService, eventBus)
    {
    }

    /// <summary>
    /// Handles the card's behavior and updates the UI elements associated with an effect card.
    /// </summary>
    /// <param name="card">The in-game card to be handled.</param>
    public override void HandleCard(InGameCard card)
    {
        // Phase 17-18: Use ICardCollectionService instead of CardManager.Instance
        var playerCard = cardCollectionService.GetCurrentPlayerCards();
        var opponentPlayerCard = cardCollectionService.GetOpponentPlayerCards();
        var opponentPlayerStatus = playerStatusProvider.GetOpponentPlayerStatus();
        var effectCard = card as InGameEffectCard;
        menuScript.putCardButtonText.SetText(localizationService.GetLocalizedValue(LocalizationKeys.BUTTON_PUT_CARD));
        menuScript.putCardButton.interactable =
            effectCard?.EffectAbilities.All(elt =>
                elt.CanUseEffect(playerCard, opponentPlayerCard, opponentPlayerStatus)) == true && playerCard.EffectCards.Count < 4;
    }

    /// <summary>
    /// Handles the card placement behavior for effect cards.
    /// </summary>
    /// <param name="card">The in-game card that is being placed.</param>
    public override void HandleCardPut(InGameCard card)
    {
        if (card is InGameEffectCard effectCard)
        {
            // Phase 36: Publish via EventBus (primary)
            var playerCards = cardCollectionService.GetCurrentPlayerCards();
            var owner = playerCards.IsPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2;
            eventBus.Publish(new EffectCardPlayRequestedEvent
            {
                EffectCard = effectCard,
                Owner = owner
            });

            // Keep static event for backwards compatibility during migration
            InGameMenuScript.EffectCardEvent.Invoke(effectCard);
        }
    }
}