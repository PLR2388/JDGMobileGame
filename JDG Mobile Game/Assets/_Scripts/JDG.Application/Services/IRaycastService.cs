using JDG.Application.Cards;

namespace JDG.Application.Services
{
    /// <summary>
    /// Service for raycasting to detect card interactions in the game.
    /// Provides a testable abstraction over Unity's physics raycasting.
    ///
    /// Phase 166: Refactored to use IInGameCard interface instead of concrete InGameCard.
    /// Moved to JDG.Application.Services.
    ///
    /// Part of Phase 1 migration - removes CardRaycastManager singleton.
    /// </summary>
    public interface IRaycastService
    {
        /// <summary>
        /// Retrieves the card under the user's current touch or click position.
        /// </summary>
        /// <returns>The IInGameCard being touched, or null if no card is detected.</returns>
        IInGameCard GetTouchedCard();
    }
}
