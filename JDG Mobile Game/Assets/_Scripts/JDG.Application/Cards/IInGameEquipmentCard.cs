using System.Collections.Generic;

namespace JDG.Application.Cards
{
    /// <summary>
    /// Interface representing an equipment card in the game.
    /// Phase 41: Created to abstract InGameEquipmentCard dependencies for use cases.
    /// </summary>
    public interface IInGameEquipmentCard : IInGameCard
    {
        /// <summary>
        /// Gets the equipment abilities associated with this card.
        /// Uses object to avoid coupling to legacy EquipmentAbility type.
        /// </summary>
        IReadOnlyList<object> EquipmentAbilities { get; }
    }
}
