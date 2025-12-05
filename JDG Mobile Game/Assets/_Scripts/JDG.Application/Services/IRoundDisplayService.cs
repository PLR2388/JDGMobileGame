namespace JDG.Application.Services
{
    /// <summary>
    /// Service interface for round display management.
    /// Phase 9: Removes RoundDisplayManager singleton dependency.
    /// </summary>
    public interface IRoundDisplayService
    {
        /// <summary>
        /// Sets the displayed round text.
        /// </summary>
        /// <param name="value">The text value to set.</param>
        void SetRoundText(string value);

        /// <summary>
        /// Updates the UI elements based on the game phase in the next round.
        /// </summary>
        /// <param name="rotate">Whether to rotate the camera for the end phase.</param>
        void AdaptUIToPhaseIdInNextRound(bool rotate);
    }
}
