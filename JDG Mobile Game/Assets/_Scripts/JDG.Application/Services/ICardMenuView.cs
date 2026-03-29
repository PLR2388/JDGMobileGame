namespace JDG.Application.Services
{
    /// <summary>
    /// Abstraction for card menu UI interactions used by CardHandlers.
    /// Phase 166: Created to decouple CardHandler hierarchy from InGameMenuScript MonoBehaviour,
    /// enabling the handlers to move to JDG.Infrastructure assembly.
    /// </summary>
    public interface ICardMenuView
    {
        /// <summary>
        /// Sets the text displayed on the "put card" button.
        /// </summary>
        void SetPutCardButtonText(string text);

        /// <summary>
        /// Sets whether the "put card" button is interactable.
        /// </summary>
        void SetPutCardButtonInteractable(bool interactable);

        /// <summary>
        /// Checks if card interaction is allowed in the current game state.
        /// Returns false if the game is over or interaction is otherwise blocked.
        /// </summary>
        bool CanInteractWithCards();

        /// <summary>
        /// Checks if card placement is allowed in the current game state.
        /// Returns false if the game is over or the current phase does not allow placement.
        /// </summary>
        bool CanPlaceCards();
    }
}
