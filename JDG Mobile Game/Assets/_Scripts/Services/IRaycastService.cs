using Cards;
using JDG.Infrastructure.Cards;
using JetBrains.Annotations;

/// <summary>
/// Service for raycasting to detect card interactions in the game.
/// Provides a testable abstraction over Unity's physics raycasting.
///
/// Note: This service is in the default assembly (not JDG.Infrastructure) because
/// it depends on legacy types (InGameCard, PhysicalCardDisplay) that haven't been
/// migrated yet. Once card types are refactored into Domain/Application layers,
/// this service can be moved to JDG.Infrastructure.
///
/// Part of Phase 1 migration - removes CardRaycastManager singleton.
/// </summary>
public interface IRaycastService
{
    /// <summary>
    /// Retrieves the InGameCard under the user's current touch or click position.
    /// </summary>
    /// <returns>The InGameCard being touched or null if no card is detected.</returns>
    [CanBeNull]
    InGameCard GetTouchedCard();
}
