using System.Linq;
using Cards;
using JDG.Application;
using JDG.Application.Abilities;
using JDG.Application.Services;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Cards;

/// <summary>
/// Handler responsible for effect card-specific behaviors in the game.
/// Phase 17-18: Removed CardManager singleton dependency via ICardCollectionService.
/// Phase 28: Uses IPlayerStatusProvider instead of PlayerManager.Instance.
/// Phase 34: Uses ILocalizationService instead of LocalizationSystem.Instance.
/// Phase 36: Uses IEventBus for static UnityEvent migration.
/// Phase 114: Updated to use modern IAbility for ability checks.
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
    /// Phase 114: Updated to use modern IAbility.CanActivate() instead of legacy CanUseEffect().
    /// </summary>
    /// <param name="card">The in-game card to be handled.</param>
    public override void HandleCard(InGameCard card)
    {
        // Phase 17-18: Use ICardCollectionService instead of CardManager.Instance
        var playerCard = cardCollectionService.GetCurrentPlayerCards();
        var effectCard = card as InGameEffectCard;
        menuScript.putCardButtonText.SetText(localizationService.GetLocalizedValue(LocalizationKeys.BUTTON_PUT_CARD));

        // Phase 114: Check if all modern abilities can activate
        bool canPutCard = effectCard != null &&
                          playerCard.EffectCards.Count < 4 &&
                          CanAllAbilitiesActivate(effectCard, playerCard.IsPlayerOne);

        menuScript.putCardButton.interactable = canPutCard;
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
        var owner = isPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2;
        var ownerId = PlayerId.FromCardOwner(owner);
        var opponentOwner = isPlayerOne ? JDG.Domain.CardOwner.Player2 : JDG.Domain.CardOwner.Player1;
        var opponentId = PlayerId.FromCardOwner(opponentOwner);
        var context = new AbilityContext(ownerId, opponentId, null, JDG.Domain.AbilityName.Default);

        return effectCard.ModernEffectAbilities.All(ability => ability.CanActivate(context));
    }

    /// <summary>
    /// Handles the card placement behavior for effect cards.
    /// Phase 109: Publishes EffectCardPlayRequestedEvent via EventBus only.
    /// </summary>
    /// <param name="card">The in-game card that is being placed.</param>
    public override void HandleCardPut(InGameCard card)
    {
        if (card is InGameEffectCard effectCard)
        {
            // Phase 36/109: Publish via EventBus
            var playerCards = cardCollectionService.GetCurrentPlayerCards();
            var owner = playerCards.IsPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2;
            eventBus.Publish(new EffectCardPlayRequestedEvent
            {
                EffectCard = effectCard,
                Owner = owner
            });
        }
    }
}