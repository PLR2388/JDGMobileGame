using JDG.Application.Cards;

namespace JDG.Application.Services
{
    /// <summary>
    /// Service for resolving card visual data.
    /// Phase 39: Created to abstract Unity Material dependencies from presenters.
    ///
    /// This service allows presenters to work with cards without directly
    /// depending on Unity types like Material. The implementation in
    /// Infrastructure layer handles the actual Unity Material resolution.
    /// </summary>
    public interface ICardVisualService
    {
        /// <summary>
        /// Gets the visual material for a card.
        /// Returns the material as object to avoid Unity dependency in Application layer.
        /// Cast to UnityEngine.Material in the implementation/consumer.
        /// </summary>
        /// <param name="card">The card to get visual for</param>
        /// <returns>The Material object (UnityEngine.Material)</returns>
        object GetMaterial(IInGameCard card);

        /// <summary>
        /// Gets the visual material by visual ID.
        /// Returns the material as object to avoid Unity dependency in Application layer.
        /// </summary>
        /// <param name="visualId">The visual identifier</param>
        /// <returns>The Material object (UnityEngine.Material)</returns>
        object GetMaterialByVisualId(string visualId);

        /// <summary>
        /// Checks if a visual exists for the given card.
        /// </summary>
        /// <param name="card">The card to check</param>
        /// <returns>True if visual exists, false otherwise</returns>
        bool HasVisual(IInGameCard card);
    }
}
