using System.Collections.Generic;

namespace JDG.Application.Services
{
    /// <summary>
    /// Service interface for instantiating physical card GameObjects.
    /// Replaces UnitManager singleton from Phase 17-18.
    ///
    /// The implementation handles Unity-specific concerns (GameObject, Quaternion, Vector3)
    /// while the interface keeps the Application layer clean.
    /// </summary>
    public interface ICardInstantiationService
    {
        /// <summary>
        /// Initializes physical card GameObjects for a deck.
        /// </summary>
        /// <param name="deck">The deck of cards to instantiate.</param>
        /// <param name="deckLocationX">X position of the deck.</param>
        /// <param name="deckLocationY">Y position of the deck.</param>
        /// <param name="deckLocationZ">Z position of the deck.</param>
        /// <param name="isPlayerOne">True if cards belong to Player 1, affects rotation.</param>
        void InitializePhysicalCards(
            List<Cards.InGameCard> deck,
            float deckLocationX,
            float deckLocationY,
            float deckLocationZ,
            bool isPlayerOne);

        /// <summary>
        /// Retrieves the GameObject for a specific card by name.
        /// </summary>
        /// <param name="cardName">The unique name of the card (e.g., "FistiP1").</param>
        /// <param name="cardGameObject">The found GameObject, or null if not found.</param>
        /// <returns>True if the card GameObject was found; otherwise, false.</returns>
        bool TryGetCardGameObject(string cardName, out UnityEngine.GameObject cardGameObject);
    }
}
