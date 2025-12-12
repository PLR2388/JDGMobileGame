using Cards;

/// <summary>
/// Represents a base class for handling card-specific behaviors within the game.
/// Phase 17-18: Added ICardCollectionService for card state access.
/// Phase 28: Added IPlayerStatusProvider for player status access.
/// </summary>
public abstract class CardHandler
{
    /// <summary>
    /// Reference to the in-game menu script for UI interactions or other operations.
    /// </summary>
    protected InGameMenuScript menuScript;

    /// <summary>
    /// Phase 17-18: Service for accessing player card collections.
    /// </summary>
    protected ICardCollectionService cardCollectionService;

    /// <summary>
    /// Phase 28: Provider for accessing player status.
    /// </summary>
    protected IPlayerStatusProvider playerStatusProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CardHandler"/> class.
    /// Phase 17-18: Added cardCollectionService parameter.
    /// Phase 28: Added playerStatusProvider parameter.
    /// </summary>
    /// <param name="menuScript">The in-game menu script associated with this handler.</param>
    /// <param name="cardCollectionService">The service for accessing player card collections.</param>
    /// <param name="playerStatusProvider">The provider for accessing player status.</param>
    public CardHandler(InGameMenuScript menuScript, ICardCollectionService cardCollectionService, IPlayerStatusProvider playerStatusProvider)
    {
        this.menuScript = menuScript;
        this.cardCollectionService = cardCollectionService;
        this.playerStatusProvider = playerStatusProvider;
    }

    /// <summary>
    /// Provides behavior definitions when a card is interacted with or activated.
    /// Implementations should define how the UI or other game elements respond to this interaction.
    /// </summary>
    /// <param name="card">The in-game card to be handled.</param>
    public abstract void HandleCard(InGameCard card);

    /// <summary>
    /// Provides behavior definitions when a card is placed or positioned within the game.
    /// Implementations should define the game's response to placing the card, such as triggering effects.
    /// </summary>
    /// <param name="card">The in-game card that is being placed.</param>
    public abstract void HandleCardPut(InGameCard card);
}