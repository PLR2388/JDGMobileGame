using Cards;

/// <summary>
/// Handler responsible for field card-specific behaviors in the game.
/// Phase 17-18: Removed CardManager singleton dependency via ICardCollectionService.
/// </summary>
public class FieldCardHandler : CardHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FieldCardHandler"/> class.
    /// Phase 17-18: Added cardCollectionService parameter.
    /// </summary>
    /// <param name="menuScript">The in-game menu script associated with this handler.</param>
    /// <param name="cardCollectionService">The service for accessing player card collections.</param>
    public FieldCardHandler(InGameMenuScript menuScript, ICardCollectionService cardCollectionService)
        : base(menuScript, cardCollectionService)
    {
    }

    /// <summary>
    /// Handles the card's behavior and updates the UI elements associated with a field card.
    /// </summary>
    /// <param name="card">The in-game card to be handled.</param>
    public override void HandleCard(InGameCard card)
    {
        // Phase 17-18: Use ICardCollectionService instead of CardManager.Instance
        var playerCard = cardCollectionService.GetCurrentPlayerCards();
        menuScript.putCardButtonText.SetText(LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_PUT_CARD));
        menuScript.putCardButton.interactable = playerCard.FieldCard == null;
    }
    
    /// <summary>
    /// Handles the card placement behavior for field cards.
    /// </summary>
    /// <param name="card">The in-game card that is being placed.</param>
    public override void HandleCardPut(InGameCard card)
    {
        if (card is InGameFieldCard fieldCard)
        {
            InGameMenuScript.FieldCardEvent.Invoke(fieldCard);    
        }
    }
}