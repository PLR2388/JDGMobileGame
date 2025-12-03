using Cards;

/// <summary>
/// Service for accessing player card collections.
/// Replaces CardManager's card access responsibilities.
/// Part of Phase 4 - CardManager decomposition.
///
/// Note: This interface is in the default assembly because it references legacy types
/// (PlayerCards). It will be moved to JDG.Application once PlayerCards is refactored.
/// </summary>
public interface ICardCollectionService
{
    /// <summary>
    /// Gets the card collection for the current player.
    /// </summary>
    PlayerCards GetCurrentPlayerCards();

    /// <summary>
    /// Gets the card collection for the opponent player.
    /// </summary>
    PlayerCards GetOpponentPlayerCards();
}
