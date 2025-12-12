using Cards;

/// <summary>
/// Handler responsible for contre card-specific behaviors in the game.
/// Phase 17-18: Updated constructor signature to match base class changes.
/// Phase 28: Added IPlayerStatusProvider parameter.
/// </summary>
public class ContreCardHandler : CardHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContreCardHandler"/> class.
    /// Phase 17-18: Added cardCollectionService parameter.
    /// Phase 28: Added playerStatusProvider parameter.
    /// </summary>
    /// <param name="menuScript">The in-game menu script associated with this handler.</param>
    /// <param name="cardCollectionService">The service for accessing player card collections.</param>
    /// <param name="playerStatusProvider">The provider for accessing player status.</param>
    public ContreCardHandler(InGameMenuScript menuScript, ICardCollectionService cardCollectionService, IPlayerStatusProvider playerStatusProvider)
        : base(menuScript, cardCollectionService, playerStatusProvider)
    {
    }

    /// <summary>
    /// Handles the card's behavior and updates the UI elements associated with a contre card.
    /// </summary>
    /// <param name="card">The in-game card to be handled.</param>
    public override void HandleCard(InGameCard card)
    {
        menuScript.putCardButtonText.SetText(LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_CONTRE));
        menuScript.putCardButton.interactable = true;
    }

    /// <summary>
    /// Handles the card placement behavior for contre cards.
    /// </summary>
    /// <param name="card">The in-game card that is being placed.</param>
    public override void HandleCardPut(InGameCard card)
    {
        throw new System.NotImplementedException();
    }
}