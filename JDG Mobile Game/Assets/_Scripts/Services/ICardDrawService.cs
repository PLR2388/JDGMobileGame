using UnityEngine.Events;

/// <summary>
/// Service for card drawing operations.
/// Replaces CardManager's card drawing responsibilities.
/// Part of Phase 4 - CardManager decomposition.
///
/// Note: This interface is in the default assembly because it uses UnityAction and
/// implementations depend on legacy types. It will be moved to JDG.Application once
/// these dependencies are refactored.
/// </summary>
public interface ICardDrawService
{
    /// <summary>
    /// Draws a card for the current player.
    /// </summary>
    /// <param name="onNoCard">Callback invoked when there are no cards left to draw.</param>
    void DrawCard(UnityAction onNoCard);
}
