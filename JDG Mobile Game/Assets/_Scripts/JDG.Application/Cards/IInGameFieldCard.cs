using System.Collections.Generic;
using JDG.Application.Abilities;

namespace JDG.Application.Cards
{
    /// <summary>
    /// Interface representing a field card in the game.
    /// Phase 41: Created to abstract InGameFieldCard dependencies for use cases.
    /// Phase 116: Updated to return IAbility instead of object.
    /// </summary>
    public interface IInGameFieldCard : IInGameCard
    {
        /// <summary>
        /// Gets the field abilities associated with this card.
        /// Phase 116: Now returns modern IAbility instances.
        /// </summary>
        IReadOnlyList<IAbility> FieldAbilities { get; }
    }
}
