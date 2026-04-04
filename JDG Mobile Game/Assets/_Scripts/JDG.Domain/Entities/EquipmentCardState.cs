using System.Collections.Generic;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;

namespace JDG.Domain.Entities
{
    /// <summary>
    /// Domain entity representing the runtime state of an equipment card in play.
    /// Pure C# - no Unity dependencies.
    /// ECS-ready: Structured to easily convert to ECS components.
    ///
    /// Phase 69: Created as part of UseCase migration.
    ///
    /// Equipment cards attach to invocation cards and modify their behavior.
    /// They track which card they're attached to and their active abilities.
    /// </summary>
    public class EquipmentCardState : InGameCardState
    {
        /// <summary>
        /// List of equipment ability names on this card.
        /// </summary>
        public IReadOnlyList<EquipmentAbilityName> EquipmentAbilities { get; }

        /// <summary>
        /// ID of the invocation card this equipment is attached to.
        /// Null if not currently equipped.
        /// </summary>
        public CardId? AttachedToCardId { get; set; }

        /// <summary>
        /// Creates a new EquipmentCardState with the specified abilities.
        /// </summary>
        public EquipmentCardState(
            CardId cardDefinitionId,
            CardOwner owner,
            IEnumerable<EquipmentAbilityName> equipmentAbilities)
            : base(CardId.New(), cardDefinitionId, CardType.Equipment, owner)
        {
            EquipmentAbilities = new List<EquipmentAbilityName>(equipmentAbilities).AsReadOnly();
            AttachedToCardId = null;
        }

        /// <summary>
        /// Attaches this equipment to an invocation card.
        /// </summary>
        public void AttachTo(CardId invocationCardId)
        {
            AttachedToCardId = invocationCardId;
        }

        /// <summary>
        /// Detaches this equipment from its current card.
        /// </summary>
        public void Detach()
        {
            AttachedToCardId = null;
        }

        /// <summary>
        /// Checks if this equipment is currently attached to any card.
        /// </summary>
        public bool IsAttached => AttachedToCardId.HasValue;

        public override string ToString()
        {
            var attached = AttachedToCardId.HasValue ? $"Attached to {AttachedToCardId}" : "Not attached";
            return $"EquipmentCardState[{Id}] {attached}, Abilities={EquipmentAbilities.Count}, Owner={Owner}";
        }
    }
}
