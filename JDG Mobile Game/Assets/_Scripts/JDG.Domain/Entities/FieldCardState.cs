using System.Collections.Generic;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;

namespace JDG.Domain.Entities
{
    /// <summary>
    /// Domain entity representing the runtime state of a field card in play.
    /// Pure C# - no Unity dependencies.
    /// ECS-ready: Structured to easily convert to ECS components.
    ///
    /// Phase 69: Created as part of UseCase migration.
    ///
    /// Field cards affect all cards of a specific family and have continuous effects.
    /// Only one field card can be active per player at a time.
    /// </summary>
    public class FieldCardState : InGameCardState
    {
        /// <summary>
        /// The family this field card affects.
        /// </summary>
        public CardFamily AffectedFamily { get; }

        /// <summary>
        /// List of field ability names on this card.
        /// </summary>
        public IReadOnlyList<FieldAbilityName> FieldAbilities { get; }

        /// <summary>
        /// Creates a new FieldCardState with the specified family and abilities.
        /// </summary>
        public FieldCardState(
            CardId cardDefinitionId,
            CardOwner owner,
            CardFamily affectedFamily,
            IEnumerable<FieldAbilityName> fieldAbilities)
            : base(CardId.New(), cardDefinitionId, CardType.Field, owner)
        {
            AffectedFamily = affectedFamily;
            FieldAbilities = new List<FieldAbilityName>(fieldAbilities).AsReadOnly();
        }

        /// <summary>
        /// Checks if this field card affects the given family.
        /// </summary>
        public bool AffectsFamily(CardFamily family)
        {
            return AffectedFamily == family || AffectedFamily == CardFamily.Any;
        }

        public override string ToString()
        {
            return $"FieldCardState[{Id}] Family={AffectedFamily}, Abilities={FieldAbilities.Count}, Owner={Owner}";
        }
    }
}
