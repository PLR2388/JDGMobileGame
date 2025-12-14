using System.Collections.Generic;

namespace JDG.Application.Cards
{
    /// <summary>
    /// Interface representing an effect card in the game.
    /// Phase 41: Created to abstract InGameEffectCard dependencies for use cases.
    /// </summary>
    public interface IInGameEffectCard : IInGameCard
    {
        /// <summary>
        /// Gets the effect abilities associated with this card.
        /// Uses object to avoid coupling to legacy EffectAbility type.
        /// </summary>
        IReadOnlyList<object> EffectAbilities { get; }
    }
}
