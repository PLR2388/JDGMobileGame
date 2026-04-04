namespace JDG.Domain.ValueObjects
{
    /// <summary>
    /// Domain value object containing deck configuration constants.
    /// Replaces static constants from legacy GameState singleton.
    /// </summary>
    public readonly struct DeckConfiguration
    {
        /// <summary>
        /// Maximum number of cards allowed in a deck.
        /// </summary>
        public const int MaxDeckCards = 30;

        /// <summary>
        /// Maximum number of rare cards allowed in a deck.
        /// </summary>
        public const int MaxRare = 5;

        /// <summary>
        /// Initial number of cards drawn at game start.
        /// </summary>
        public const int InitialNumberOfHandCards = 5;

        /// <summary>
        /// Validates if a deck size is within legal limits.
        /// </summary>
        /// <param name="deckSize">The number of cards in the deck.</param>
        /// <returns>True if the deck size is valid; otherwise, false.</returns>
        public static bool IsValidDeckSize(int deckSize)
        {
            return deckSize > 0 && deckSize <= MaxDeckCards;
        }

        /// <summary>
        /// Validates if the number of rare cards is within legal limits.
        /// </summary>
        /// <param name="rareCount">The number of rare cards in the deck.</param>
        /// <returns>True if the rare count is valid; otherwise, false.</returns>
        public static bool IsValidRareCount(int rareCount)
        {
            return rareCount >= 0 && rareCount <= MaxRare;
        }
    }
}
