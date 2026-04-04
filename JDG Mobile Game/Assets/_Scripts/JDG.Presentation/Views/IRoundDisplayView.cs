namespace JDG.Presentation.Views
{
    /// <summary>
    /// View interface for displaying round/turn information.
    /// Part of Phase 28 - MonoBehaviour Wave 1 migration to MVP pattern.
    /// </summary>
    public interface IRoundDisplayView
    {
        /// <summary>
        /// Sets the round/phase text displayed to the user.
        /// </summary>
        /// <param name="text">Localized text to display</param>
        void SetRoundText(string text);

        /// <summary>
        /// Sets the player turn indicator text.
        /// </summary>
        /// <param name="playerName">Localized player name</param>
        void SetPlayerTurnText(string playerName);

        /// <summary>
        /// Shows or hides the "in hand" button.
        /// </summary>
        /// <param name="visible">True to show, false to hide</param>
        void SetInHandButtonVisible(bool visible);

        /// <summary>
        /// Rotates the camera for the end phase transition.
        /// Unity-specific operation that stays in the view.
        /// </summary>
        void RotateCamera();
    }
}
