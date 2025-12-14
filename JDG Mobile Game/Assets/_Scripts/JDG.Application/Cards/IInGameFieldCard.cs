using System.Collections.Generic;

namespace JDG.Application.Cards
{
    /// <summary>
    /// Interface representing a field card in the game.
    /// Phase 41: Created to abstract InGameFieldCard dependencies for use cases.
    /// </summary>
    public interface IInGameFieldCard : IInGameCard
    {
        /// <summary>
        /// Gets the field abilities associated with this card.
        /// Uses object to avoid coupling to legacy FieldAbility type.
        /// </summary>
        IReadOnlyList<object> FieldAbilities { get; }
    }
}
