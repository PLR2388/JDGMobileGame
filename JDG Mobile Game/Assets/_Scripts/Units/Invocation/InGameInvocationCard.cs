using System.Collections.Generic;
using System.Linq;
using _Scripts.Cards.InvocationCards;
using Cards;
using Cards.InvocationCards;
using JDG.Application;
using JDG.Application.Abilities;
using JDG.Application.Cards;
using JDG.Application.Services;
using JDG.Domain.Events;
using DomainCardFamily = JDG.Domain.Enums.CardFamily;

namespace _Scripts.Units.Invocation
{
    /// <summary>
    /// Phase 17-18: Removed CardManager singleton dependency via ICardCollectionService.
    /// Phase 24-25: Removed ServiceLocator, using constructor injection.
    /// Phase 7: Added IAbilityProvider for AbilityLibrary → AbilityRegistry migration.
    /// Phase 49: Implements IInGameInvocationCard for complete abstraction.
    /// Phase 105: Added ModernAbilities for IAbility migration.
    /// </summary>
    public class InGameInvocationCard : InGameCard, IInGameInvocationCard
    {
        public InvocationCard BaseInvocationCard;
        private bool blockAttackNextTurn;
        private int remainedAttackThisTurn;
        private bool cancelEffect;

        private const int DefaultNumberAttacksPerTurn = 1;

        // Phase 24-25: Injected dependencies
        private readonly IEventBus _eventBus;
        private readonly ICardCollectionService _cardCollectionService;

        // Phase 42ag: Required ability provider (legacy AbilityLibrary removed)
        private readonly IAbilityProvider _abilityProvider;

        // Phase 56: Condition provider (replaces ConditionLibrary.Instance)
        private readonly IConditionProvider _conditionProvider;

        /// <summary>
        /// Gets or sets whether the card effect is canceled.
        /// Phase 23: Migrated from static UnityEvent to EventBus.
        /// Phase 24-25: Uses injected _eventBus.
        /// </summary>
        public bool CancelEffect
        {
            get => cancelEffect;
            set
            {
                cancelEffect = value;
                UpdateInvocationCardForAbilities();

                // Phase 24-25: Use injected _eventBus
                var domainOwner = (JDG.Domain.CardOwner)(int)CardOwner;
                _eventBus.Publish(new InvocationCancelledEvent
                {
                    CancelledCard = this,
                    Owner = domainOwner
                });
            }
        }

        public bool CanDirectAttack { get; set; }

        public bool CantBeAttack { get; set; }

        public bool Aggro { get; set; }

        private List<global::Condition> conditions = new List<global::Condition>();

        /// <summary>
        /// List of legacy abilities associated with the invocation card.
        /// Phase 105: Marked for deprecation - use ModernAbilities instead.
        /// </summary>
        public List<Ability> Abilities = new List<Ability>();

        /// <summary>
        /// List of modern IAbility implementations for this card.
        /// Phase 105: New property for clean architecture migration.
        /// </summary>
        public List<IAbility> ModernAbilities { get; private set; } = new List<IAbility>();


        public int NumberOfTurnOnField { get; private set; }

        public int NumberOfDeaths { get; private set; }

        public InGameEquipmentCard EquipmentCard { get; set; }

        /// <summary>
        /// Initializes an instance of the InGameInvocationCard.
        /// Phase 24-25: Added dependency injection for IEventBus and ICardCollectionService.
        /// Phase 42ag: IAbilityProvider is now required (legacy AbilityLibrary removed).
        /// Phase 56: Added IConditionProvider to replace ConditionLibrary.Instance.
        /// Phase 61: Made IConditionProvider required (removed fallback to legacy singleton).
        /// </summary>
        /// <param name="invocationCard">The base invocation card.</param>
        /// <param name="cardOwner">The owner of the card.</param>
        /// <param name="eventBus">EventBus for publishing domain events.</param>
        /// <param name="cardCollectionService">Service for accessing player cards.</param>
        /// <param name="abilityProvider">Provider for abilities (required).</param>
        /// <param name="conditionProvider">Provider for conditions (required).</param>
        /// <returns>A new InGameInvocationCard instance.</returns>
        public InGameInvocationCard(
            InvocationCard invocationCard,
            CardOwner cardOwner,
            IEventBus eventBus,
            ICardCollectionService cardCollectionService,
            IAbilityProvider abilityProvider,
            IConditionProvider conditionProvider)
        {
            BaseInvocationCard = invocationCard;
            CardOwner = cardOwner;
            _eventBus = eventBus;
            _cardCollectionService = cardCollectionService;
            _abilityProvider = abilityProvider;
            _conditionProvider = conditionProvider;
            Reset();
        }

        public CardFamily[] Families { get; set; }

        public float Attack { get; set; }

        public float Defense { get; set; }

        /// <summary>
        /// This represent the card that receive the power from this card
        /// only use for L'elfette and Sangoku right now that give their
        /// atk and def
        /// </summary>
        public string Receiver { get; set; } = null;

        /// <summary>
        /// Resets the card to its initial state.
        /// </summary>
        private void Reset()
        {
            title = BaseInvocationCard.Title;
            Description = BaseInvocationCard.Description;
            DetailedDescription = BaseInvocationCard.DetailedDescription;
            type = BaseInvocationCard.Type;
            BaseCard = BaseInvocationCard;
            materialCard = BaseInvocationCard.MaterialCard;
            collector = BaseInvocationCard.Collector;
            NumberOfTurnOnField = 0;
            NumberOfDeaths = 0;
            remainedAttackThisTurn = DefaultNumberAttacksPerTurn;

            IsControlled = false;

            Attack = BaseInvocationCard.BaseInvocationCardStats.Attack;
            Defense = BaseInvocationCard.BaseInvocationCardStats.Defense;
            Families = BaseInvocationCard.BaseInvocationCardStats.Families;
            EquipmentCard = null;
            IsAffectedByEffectCard = BaseInvocationCard.BaseInvocationCardStats.AffectedByEffect;

            // Phase 61: Use injected provider (fallback removed)
            conditions = BaseInvocationCard.Conditions
                .Select(conditionName => _conditionProvider.GetCondition(conditionName) as global::Condition)
                .Where(condition => condition != null)
                .ToList();

            // Phase 42ag: IAbilityProvider is now required (legacy AbilityLibrary removed)
            // Legacy abilities (for backward compatibility)
            Abilities = BaseInvocationCard.Abilities
                .Select(abilityName => _abilityProvider.GetAbility(abilityName))
                .Where(ability => ability != null)
                .ToList();
            UpdateInvocationCardForAbilities();

            // Phase 105: Populate modern abilities
            ModernAbilities = BaseInvocationCard.Abilities
                .Select(abilityName => _abilityProvider.GetModernAbility(abilityName))
                .Where(ability => ability != null)
                .ToList();
        }
        
        /// <summary>
        /// Updates the invocation card for abilities.
        /// </summary>
        private void UpdateInvocationCardForAbilities()
        {
            foreach (var ability in Abilities)
            {
                ability.InvocationCard = this;
            }
        }

        /// <summary>
        /// Checks if the card can be summoned.
        /// </summary>
        /// <param name="playerCards">The player cards.</param>
        /// <returns>true if can be summoned; otherwise, false.</returns>
        public bool CanBeSummoned(PlayerCards playerCards)
        {
            return conditions.Count == 0 || conditions.TrueForAll(condition => condition.CanBeSummoned(playerCards));
        }

        /// <summary>
        /// Blocks the card's attack for the next turn.
        /// </summary>
        public void BlockAttack()
        {
            blockAttackNextTurn = true;
        }

        /// <summary>
        /// Unblocks the card's attack.
        /// </summary>
        public void UnblockAttack()
        {
            blockAttackNextTurn = false;
        }

        public bool IsAffectedByEffectCard { get; set; } = true;

        /// <summary>
        /// Sets the remained attack for this turn.
        /// </summary>
        /// <param name="number">The number of remained attacks.</param>
        public void SetRemainedAttackThisTurn(int number)
        {
            remainedAttackThisTurn = number;
        }

        /// <summary>
        /// Increments the number of deaths.
        /// </summary>
        public void IncrementNumberDeaths()
        {
            NumberOfDeaths++;
        }

        /// <summary>
        /// Checks if the card can attack.
        /// </summary>
        /// <returns>true if can attack; otherwise, false.</returns>
        public bool CanAttack()
        {
            if (EquipmentCard == null) return remainedAttackThisTurn > 0 && !blockAttackNextTurn;
            return remainedAttackThisTurn > 0 && !blockAttackNextTurn; //& !equipmentBlockedAttack;
        }

        /// <summary>
        /// Checks if the card has an available action.
        /// </summary>
        /// <returns>true if has action; otherwise, false.</returns>
        public bool HasAction()
        {
            return Abilities.Exists(elt => elt.IsAction);
        }

        /// <summary>
        /// Marks that the card has done an attack this turn.
        /// </summary>
        public void AttackTurnDone()
        {
            remainedAttackThisTurn--;
        }

     
        /// <summary>
        /// Resets the card for a new turn.
        /// </summary>
        public void ResetNewTurn()
        {
            remainedAttackThisTurn = DefaultNumberAttacksPerTurn;
        }

        /// <summary>
        /// Checks if invoking the card is possible.
        /// Phase 17-18: Uses ICardCollectionService instead of CardManager.Instance.
        /// Phase 24-25: Uses injected _cardCollectionService.
        /// </summary>
        /// <returns>true if invocation is possible; otherwise, false.</returns>
        public bool IsInvocationPossible()
        {
            // Phase 24-25: Use injected _cardCollectionService
            return CanBeSummoned(_cardCollectionService.GetCurrentPlayerCards());
        }

        /// <summary>
        /// SetEquipmentCard.
        /// Change equipment card.
        /// If user decided to remove an equipment (card = null), one should remove all equipment effect
        /// <param name="card">new equipment card</param>
        /// </summary>
        public void SetEquipmentCard(InGameEquipmentCard card)
        {
            EquipmentCard = card;
        }

        public bool IsControlled { get; private set; }

        /// <summary>
        /// Controls the card.
        /// </summary>
        public void ControlCard()
        {
            IsControlled = true;
        }

        /// <summary>
        /// Frees the card.
        /// </summary>
        public void FreeCard()
        {
            IsControlled = false;
        }

        /// <summary>
        /// Increments the number of turns on the field.
        /// </summary>
        public void IncrementNumberTurnOnField()
        {
            NumberOfTurnOnField++;
            UpdateInvocationCardForAbilities();
        }

        /// <summary>
        /// Gets the current defense value of the card.
        /// </summary>
        /// <returns>The current defense value.</returns>
        public float GetCurrentDefense()
        {
            return Defense;
        }

        /// <summary>
        /// Gets the current attack value of the card.
        /// </summary>
        /// <returns>The current attack value.</returns>
        public float GetCurrentAttack()
        {
            return Attack;
        }

        #region IInGameInvocationCard Implementation

        /// <summary>
        /// Gets the base attack value from the card definition.
        /// Phase 49: Added for IInGameInvocationCard interface.
        /// Note: BaseInvocationCardStats is a struct (value type), so no ?. operator is needed.
        /// </summary>
        public float BaseAttack => BaseInvocationCard?.BaseInvocationCardStats.Attack ?? 0;

        /// <summary>
        /// Gets the base defense value from the card definition.
        /// Phase 49: Added for IInGameInvocationCard interface.
        /// Note: BaseInvocationCardStats is a struct (value type), so no ?. operator is needed.
        /// </summary>
        public float BaseDefense => BaseInvocationCard?.BaseInvocationCardStats.Defense ?? 0;

        /// <summary>
        /// Gets the abilities as a read-only list of objects.
        /// Phase 49: Explicit implementation for IInGameInvocationCard interface.
        /// </summary>
        IReadOnlyList<object> IInGameInvocationCard.Abilities => Abilities.Cast<object>().ToList().AsReadOnly();

        /// <summary>
        /// Gets the equipment card as an interface type.
        /// Phase 49: Explicit implementation for IInGameInvocationCard interface.
        /// </summary>
        IInGameEquipmentCard IInGameInvocationCard.EquipmentCard => EquipmentCard;

        /// <summary>
        /// Sets the equipment card via interface type.
        /// Phase 72: Added for IInGameInvocationCard interface extension.
        /// </summary>
        void IInGameInvocationCard.SetEquipmentCard(IInGameEquipmentCard card)
        {
            EquipmentCard = card as InGameEquipmentCard;
        }

        /// <summary>
        /// Gets or sets the families via interface type.
        /// Phase 68: Explicit implementation to convert between Cards.CardFamily and JDG.Domain.Enums.CardFamily.
        /// </summary>
        DomainCardFamily[] IInGameInvocationCard.Families
        {
            get => Families?.Select(f => (DomainCardFamily)(int)f).ToArray() ?? System.Array.Empty<DomainCardFamily>();
            set => Families = value?.Select(f => (CardFamily)(int)f).ToArray();
        }

        #endregion
    }
}