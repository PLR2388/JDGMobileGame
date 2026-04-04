using System.Collections.Generic;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;

namespace JDG.Domain.Entities
{
    /// <summary>
    /// Domain entity representing the runtime state of an effect card in play.
    /// Pure C# - no Unity dependencies.
    /// ECS-ready: Structured to easily convert to ECS components.
    ///
    /// Phase 69: Created as part of UseCase migration.
    ///
    /// Effect cards are simpler than invocation cards - they mainly track
    /// which abilities are active and whether they've been used.
    /// </summary>
    public class EffectCardState : InGameCardState
    {
        /// <summary>
        /// List of effect ability names on this card.
        /// </summary>
        public IReadOnlyList<EffectAbilityName> EffectAbilities { get; }

        /// <summary>
        /// Whether this effect card has been activated this turn.
        /// Some effects can only be used once per turn.
        /// </summary>
        public bool ActivatedThisTurn { get; set; }

        /// <summary>
        /// Creates a new EffectCardState with the specified abilities.
        /// </summary>
        public EffectCardState(
            CardId cardDefinitionId,
            CardOwner owner,
            IEnumerable<EffectAbilityName> effectAbilities)
            : base(CardId.New(), cardDefinitionId, CardType.Effect, owner)
        {
            EffectAbilities = new List<EffectAbilityName>(effectAbilities).AsReadOnly();
            ActivatedThisTurn = false;
        }

        /// <summary>
        /// Marks this effect as activated.
        /// </summary>
        public void Activate()
        {
            ActivatedThisTurn = true;
        }

        /// <summary>
        /// Resets the activation state for a new turn.
        /// </summary>
        public void ResetForNewTurn()
        {
            ActivatedThisTurn = false;
        }

        public override string ToString()
        {
            return $"EffectCardState[{Id}] Abilities={EffectAbilities.Count}, Owner={Owner}";
        }
    }
}
