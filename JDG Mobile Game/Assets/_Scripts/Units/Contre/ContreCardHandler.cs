using Cards;
using JDG.Application;
using JDG.Application.Services;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Infrastructure.Cards;

/// <summary>
/// Handler responsible for contre card-specific behaviors in the game.
/// Phase 17-18: Updated constructor signature to match base class changes.
/// Phase 28: Added IPlayerStatusProvider parameter.
/// Phase 34: Uses ILocalizationService instead of LocalizationSystem.Instance.
/// Phase 36: Added IEventBus parameter.
/// </summary>
public class ContreCardHandler : CardHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContreCardHandler"/> class.
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
    public ContreCardHandler(
        InGameMenuScript menuScript,
        ICardCollectionService cardCollectionService,
        IPlayerStatusProvider playerStatusProvider,
        ILocalizationService localizationService,
        IEventBus eventBus)
        : base(menuScript, cardCollectionService, playerStatusProvider, localizationService, eventBus)
    {
    }

    /// <summary>
    /// Handles the card's behavior and updates the UI elements associated with a contre card.
    /// Phase 135: Added game state validation.
    /// </summary>
    /// <param name="card">The in-game card to be handled.</param>
    public override void HandleCard(InGameCard card)
    {
        // Phase 135: Validate game state before enabling button
        if (!menuScript.CanInteractWithCards())
        {
            menuScript.putCardButton.interactable = false;
            return;
        }

        menuScript.putCardButtonText.SetText(localizationService.GetLocalizedValue(LocalizationKeys.BUTTON_CONTRE));
        menuScript.putCardButton.interactable = true;
    }

    /// <summary>
    /// Handles the card placement behavior for contre cards.
    /// Contre cards have immediate effect and are discarded after use.
    /// Phase 135: Added game state validation.
    /// </summary>
    /// <param name="card">The in-game card that is being placed.</param>
    public override void HandleCardPut(InGameCard card)
    {
        if (card == null) return;

        // Phase 135: Validate game state before placing card
        if (!menuScript.CanPlaceCards())
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log("ContreCardHandler: Cannot place contre card - wrong phase or game over");
#endif
            return;
        }

        // Get the owner of the card
        var playerCards = cardCollectionService.GetCurrentPlayerCards();
        var owner = playerCards.IsPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2;

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