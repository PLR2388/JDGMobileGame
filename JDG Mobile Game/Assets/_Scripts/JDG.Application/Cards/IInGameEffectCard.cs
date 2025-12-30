using System.Collections.Generic;
using JDG.Application.Abilities;

namespace JDG.Application.Cards
{
    /// <summary>
    /// Interface representing an effect card in the game.
    /// Phase 41: Created to abstract InGameEffectCard dependencies for use cases.
    /// Phase 114: Updated to use IAbility instead of legacy EffectAbility.
    /// </summary>
    public interface IInGameEffectCard : IInGameCard
    {
        /// <summary>
        /// Gets the modern effect abilities associated with this card.
        /// Phase 114: Changed from legacy EffectAbility to modern IAbility.
        /// </summary>
        IReadOnlyList<IAbility> EffectAbilities { get; }
    }
}
