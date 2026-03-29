namespace JDG.Application.Services
{
    /// <summary>
    /// Provides access to player card collections via clean interfaces.
    /// Phase 166: Created to decouple InGameInvocationCard from legacy ICardCollectionService.
    /// </summary>
    public interface ICardCollectionProvider
    {
        /// <summary>
        /// Gets the card collection for the current player.
        /// </summary>
        Cards.IPlayerCardCollection GetCurrentPlayerCardCollection();

        /// <summary>
        /// Gets the card collection for the opponent player.
        /// </summary>
        Cards.IPlayerCardCollection GetOpponentPlayerCardCollection();
    }
}
