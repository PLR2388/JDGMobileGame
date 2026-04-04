using System.Collections.Generic;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;

namespace JDG.Domain.Entities
{
    /// <summary>
    /// Domain entity representing the runtime state of an invocation card in play.
    /// Pure C# - no Unity dependencies.
    /// ECS-ready: Structured to easily convert to ECS components.
    ///
    /// Phase 68: Created as part of UseCase migration.
    ///
    /// Holds all mutable gameplay state for invocation cards:
    /// - Current stats (may differ from base due to abilities/equipment)
    /// - Attack counters and blocking state
    /// - Ability tracking
    /// - Equipment and control state
    /// </summary>
    public class InvocationCardState : InGameCardState
    {
        private const int DefaultAttacksPerTurn = 1;

        #region Stats

        /// <summary>
        /// Base attack value from card definition.
        /// </summary>
        public float BaseAttack { get; }

        /// <summary>
        /// Base defense value from card definition.
        /// </summary>
        public float BaseDefense { get; }

        /// <summary>
        /// Current attack value (may be modified by abilities/equipment).
        /// </summary>
        public float CurrentAttack { get; set; }

        /// <summary>
        /// Current defense value (may be modified by abilities/equipment).
        /// </summary>
        public float CurrentDefense { get; set; }

        /// <summary>
        /// Base families from card definition.
        /// </summary>
        public IReadOnlyList<CardFamily> BaseFamilies { get; }

        /// <summary>
        /// Current families (may be changed by field abilities).
        /// </summary>
        private List<CardFamily> _currentFamilies;
        public IReadOnlyList<CardFamily> CurrentFamilies => _currentFamilies.AsReadOnly();

        #endregion

        #region Combat State

        /// <summary>
        /// Number of attacks remaining this turn.
        /// </summary>
        public int RemainedAttacksThisTurn { get; set; }

        /// <summary>
        /// Whether the card is blocked from attacking next turn.
        /// </summary>
        public bool BlockAttackNextTurn { get; set; }

        /// <summary>
        /// Whether this card can attack the player directly.
        /// </summary>
        public bool CanDirectAttack { get; set; }

        /// <summary>
        /// Whether this card cannot be attacked.
        /// </summary>
        public bool CantBeAttacked { get; set; }

        /// <summary>
        /// Whether this card has aggro (must be attacked first).
        /// </summary>
        public bool HasAggro { get; set; }

        #endregion

        #region Ability State

        /// <summary>
        /// List of active ability names on this card.
        /// </summary>
        private List<AbilityName> _activeAbilities;
        public IReadOnlyList<AbilityName> ActiveAbilities => _activeAbilities.AsReadOnly();

        /// <summary>
        /// List of condition names for this card.
        /// </summary>
        public IReadOnlyList<ConditionName> Conditions { get; }

        /// <summary>
        /// Whether this card is affected by effect cards.
        /// </summary>
        public bool IsAffectedByEffectCard { get; set; }

        #endregion

        #region Equipment State

        /// <summary>
        /// ID of the equipped card, if any.
        /// </summary>
        public CardId? EquippedCardId { get; set; }

        /// <summary>
        /// Name of the card receiving power from this card (for abilities like L'elfette).
        /// </summary>
        public string Receiver { get; set; }

        #endregion

        #region Death/Revive Tracking

        /// <summary>
        /// Number of times this card has died.
        /// Used for abilities that trigger on death count.
        /// </summary>
        public int NumberOfDeaths { get; private set; }

        #endregion

        /// <summary>
        /// Creates a new InvocationCardState with the specified base stats.
        /// </summary>
        public InvocationCardState(
            CardId cardDefinitionId,
            CardOwner owner,
            float baseAttack,
            float baseDefense,
            IEnumerable<CardFamily> families,
            IEnumerable<AbilityName> abilities,
            IEnumerable<ConditionName> conditions,
            bool isAffectedByEffect)
            : base(CardId.New(), cardDefinitionId, CardType.Invocation, owner)
        {
            // Base stats (immutable reference values)
            BaseAttack = baseAttack;
            BaseDefense = baseDefense;
            BaseFamilies = new List<CardFamily>(families).AsReadOnly();

            // Current stats (start at base values)
            CurrentAttack = baseAttack;
            CurrentDefense = baseDefense;
            _currentFamilies = new List<CardFamily>(families);

            // Combat state
            RemainedAttacksThisTurn = DefaultAttacksPerTurn;
            BlockAttackNextTurn = false;
            CanDirectAttack = false;
            CantBeAttacked = false;
            HasAggro = false;

            // Ability state
            _activeAbilities = new List<AbilityName>(abilities);
            Conditions = new List<ConditionName>(conditions).AsReadOnly();
            IsAffectedByEffectCard = isAffectedByEffect;

            // Equipment state
            EquippedCardId = null;
            Receiver = null;

            // Death tracking
            NumberOfDeaths = 0;
        }

        #region Combat Methods

        /// <summary>
        /// Checks if this card can attack this turn.
        /// </summary>
        public bool CanAttack()
        {
            return RemainedAttacksThisTurn > 0 && !BlockAttackNextTurn;
        }

        /// <summary>
        /// Records that this card performed an attack.
        /// </summary>
        public void AttackPerformed()
        {
            if (RemainedAttacksThisTurn > 0)
            {
                RemainedAttacksThisTurn--;
            }
        }

        /// <summary>
        /// Blocks this card's attack for the next turn.
        /// </summary>
        public void BlockAttack()
        {
            BlockAttackNextTurn = true;
        }

        /// <summary>
        /// Unblocks this card's attack.
        /// </summary>
        public void UnblockAttack()
        {
            BlockAttackNextTurn = false;
        }

        /// <summary>
        /// Sets the number of attacks remaining this turn.
        /// </summary>
        public void SetRemainedAttacks(int count)
        {
            RemainedAttacksThisTurn = count;
        }

        #endregion

        #region Stats Methods

        /// <summary>
        /// Resets current stats to base values.
        /// </summary>
        public void ResetToBaseStats()
        {
            CurrentAttack = BaseAttack;
            CurrentDefense = BaseDefense;
            _currentFamilies = new List<CardFamily>(BaseFamilies);
        }

        /// <summary>
        /// Modifies current stats by delta values.
        /// Phase 158: Added minimum value clamping to 0 to prevent negative stats.
        /// </summary>
        public void ModifyStats(float attackDelta, float defenseDelta)
        {
            CurrentAttack += attackDelta;
            CurrentDefense += defenseDelta;

            // Phase 158: Clamp to minimum 0 to prevent negative stats
            // Negative stats would break damage calculations
            if (CurrentAttack < 0) CurrentAttack = 0;
            if (CurrentDefense < 0) CurrentDefense = 0;
        }

        /// <summary>
        /// Sets the current families (e.g., by field ability).
        /// </summary>
        public void SetFamilies(IEnumerable<CardFamily> families)
        {
            _currentFamilies = new List<CardFamily>(families);
        }

        /// <summary>
        /// Checks if this card is destroyed (defense <= 0).
        /// </summary>
        public bool IsDestroyed => CurrentDefense <= 0;

        #endregion

        #region Turn/Death Methods

        /// <summary>
        /// Resets this card's state for a new turn.
        /// </summary>
        public void ResetForNewTurn()
        {
            RemainedAttacksThisTurn = DefaultAttacksPerTurn;
            UnblockAttack();
        }

        /// <summary>
        /// Increments the death counter.
        /// </summary>
        public void IncrementDeathCount()
        {
            NumberOfDeaths++;
        }

        /// <summary>
        /// Resets all field state when card is removed from field/dies.
        /// </summary>
        public override void ResetFieldState()
        {
            base.ResetFieldState();
            ResetToBaseStats();
            UnblockAttack();
            Free();
            EquippedCardId = null;
        }

        #endregion

        #region Ability Methods

        /// <summary>
        /// Checks if this card has an action ability.
        /// This requires the IAbilityProvider to check - returns true if any abilities are active.
        /// </summary>
        public bool HasActiveAbilities => _activeAbilities.Count > 0;

        /// <summary>
        /// Adds an ability to this card.
        /// </summary>
        public void AddAbility(AbilityName ability)
        {
            if (!_activeAbilities.Contains(ability))
            {
                _activeAbilities.Add(ability);
            }
        }

        /// <summary>
        /// Removes an ability from this card.
        /// </summary>
        public void RemoveAbility(AbilityName ability)
        {
            _activeAbilities.Remove(ability);
        }

        #endregion

        public override string ToString()
        {
            return $"InvocationCardState[{Id}] ATK={CurrentAttack}/{BaseAttack}, DEF={CurrentDefense}/{BaseDefense}, Owner={Owner}";
        }
    }
}
