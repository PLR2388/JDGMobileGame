/// <summary>
/// Service for managing turn lifecycle (start, end, card processing).
/// Replaces CardManager's turn management responsibilities.
/// Part of Phase 4 - CardManager decomposition.
///
/// Note: This interface is in the default assembly because implementations depend on
/// legacy types. It will be moved to JDG.Application once these types are refactored.
/// </summary>
public interface ITurnService
{
    /// <summary>
    /// Handles all processing at the start of a turn.
    /// Applies invocation, effect, and field card abilities for both players.
    /// </summary>
    void OnTurnStart();

    /// <summary>
    /// Handles all processing at the end of a turn for the current player.
    /// </summary>
    void HandleEndTurn();
}
