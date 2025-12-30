using System.Collections.Generic;
using JDG.Application.Abilities;

namespace JDG.Application.Cards
{
    /// <summary>
    /// Interface representing an equipment card in the game.
    /// Phase 41: Created to abstract InGameEquipmentCard dependencies for use cases.
    /// Phase 117: Updated to use IAbility instead of object.
    /// </summary>
    public interface IInGameEquipmentCard : IInGameCard
    {
        /// <summary>
        /// Gets the equipment abilities associated with this card.
        /// Phase 117: Now returns IAbility instead of object.
        /// </summary>
        IReadOnlyList<IAbility> EquipmentAbilities { get; }

        /// <summary>
        /// Whether this equipment can be placed on cards that already have equipment.
        /// Phase 117: Added to support CanAlwaysBePut functionality.
        /// </summary>
        bool CanAlwaysBePlaced { get; }
    }
}
