/// <summary>
/// Provides access to player status for the current turn.
/// Replaces PlayerManager.Instance singleton access pattern.
/// Part of Phase 28 - MonoBehaviour migration.
///
/// Note: This interface is in the default assembly because it uses PlayerStatus
/// (a MonoBehaviour). It will be moved to JDG.Application once PlayerStatus is
/// refactored to use domain types.
/// </summary>
public interface IPlayerStatusProvider
{
    /// <summary>
    /// Gets the current player's status.
    /// </summary>
    PlayerStatus GetCurrentPlayerStatus();

    /// <summary>
    /// Gets the opponent player's status.
    /// </summary>
    PlayerStatus GetOpponentPlayerStatus();

    /// <summary>
    /// Handles attack on opponent when the target is the player entity.
    /// </summary>
    void HandleAttackIfOpponentIsPlayer();
}
