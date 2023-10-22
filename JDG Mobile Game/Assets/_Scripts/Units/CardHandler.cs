using Cards;

/// <summary>
/// Represents a base class for handling card-specific behaviors within the game.
/// </summary>
public abstract class CardHandler
{
    /// <summary>
    /// Reference to the in-game menu script for UI interactions or other operations.
    /// </summary>
    protected InGameMenuScript menuScript;

    /// <summary>
    /// Initializes a new instance of the <see cref="CardHandler"/> class.
    /// </summary>
    /// <param name="menuScript">The in-game menu script associated with this handler.</param>
    public CardHandler(InGameMenuScript menuScript)
    {
        this.menuScript = menuScript;
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