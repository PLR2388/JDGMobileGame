using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.InvocationCards;
using JDG.Application;
using JDG.Application.Abilities;
using JDG.Application.Cards;
using JDG.Application.Services;
using JDG.Domain.Events;
using DomainCardFamily = JDG.Domain.Enums.CardFamily;

namespace JDG.Infrastructure.Cards
{
    /// <summary>
    /// Phase 17-18: Removed CardManager singleton dependency via ICardCollectionProvider.
    /// Phase 24-25: Removed ServiceLocator, using constructor injection.
    /// Phase 7: Added IAbilityProvider for AbilityLibrary → AbilityRegistry migration.
    /// Phase 49: Implements IInGameInvocationCard for complete abstraction.
    /// Phase 105: Added ModernAbilities for IAbility migration.
    /// Phase 118: Removed legacy Abilities list, uses only ModernAbilities.
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
        private readonly ICardCollectionProvider _cardCollectionProvider;

        // Phase 42ag: Required ability provider (legacy AbilityLibrary removed)
        private readonly IAbilityProvider _abilityProvider;

        // Phase 56: Condition provider (replaces ConditionLibrary.Instance)
        private readonly IConditionProvider _conditionProvider;

        /// <summary>
        /// Gets or sets whether the card effect is canceled.
        /// Phase 23: Migrated from static UnityEvent to EventBus.
        /// Phase 24-25: Uses injected _eventBus.
        /// Phase 118: Removed UpdateInvocationCardForAbilities call.
        /// </summary>
        public bool CancelEffect
        {
            get => cancelEffect;
            set
            {
                cancelEffect = value;
                // Phase 118: Removed UpdateInvocationCardForAbilities call
                // Modern abilities check CancelEffect in their CanActivate implementation

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

        private List<Condition> conditions = new List<Condition>();

        /// <summary>
        /// List of modern IAbility implementations for this card.
        /// Phase 118: This is now the only ability list (legacy Abilities removed).
        /// </summary>
        public List<IAbility> ModernAbilities { get; private set; } = new List<IAbility>();

        /// <summary>
        /// Cached abilities as IReadOnlyList<object> for interface implementation.
        /// Phase 145: Added to prevent list allocation on every access.
        /// </summary>
        private IReadOnlyList<object> _abilitiesCache;


        public int NumberOfTurnOnField { get; private set; }

        public int NumberOfDeaths { get; private set; }

        /// <summary>
        /// Gets or sets the number of times this card has been revived.
        /// Used by resurrection abilities with limited revive counts.
        /// Phase 142: Added for domain Card sync.
        /// </summary>
        public int TimesRevived { get; set; }

        /// <summary>
        /// Gets or sets the bonus attacks granted for this turn.
        /// Phase 142: Added for domain Card sync.
        /// </summary>
        public int BonusAttacks { get; set; }

        public InGameEquipmentCard EquipmentCard { get; set; }

        /// <summary>
        /// Initializes an instance of the InGameInvocationCard.
        /// Phase 24-25: Added dependency injection for IEventBus and ICardCollectionProvider.
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
            ICardCollectionProvider cardCollectionService,
            IAbilityProvider abilityProvider,
            IConditionProvider conditionProvider)
        {
            BaseInvocationCard = invocationCard;
            CardOwner = cardOwner;
            _eventBus = eventBus;
            _cardCollectionProvider = cardCollectionService;
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
            TimesRevived = 0;
            BonusAttacks = 0;
            remainedAttackThisTurn = DefaultNumberAttacksPerTurn;

            IsControlled = false;

            Attack = BaseInvocationCard.BaseInvocationCardStats.Attack;
            Defense = BaseInvocationCard.BaseInvocationCardStats.Defense;
            Families = BaseInvocationCard.BaseInvocationCardStats.Families;
            EquipmentCard = null;
            IsAffectedByEffectCard = BaseInvocationCard.BaseInvocationCardStats.AffectedByEffect;

            // Phase 61: Use injected provider (fallback removed)
            // Phase 146: Added warning for missing conditions
            conditions = BaseInvocationCard.Conditions
                .Select(conditionName => {
                    var condition = _conditionProvider.GetCondition(conditionName) as Condition;
                    if (condition == null)
                        UnityEngine.Debug.LogWarning($"[InGameInvocationCard] Condition '{conditionName}' not found for card '{title}'");
                    return condition;
                })
                .Where(condition => condition != null)
                .ToList();

            // Phase 118: Populate modern abilities only (legacy Abilities removed)
            // Phase 146: Added warning for missing abilities
            ModernAbilities = BaseInvocationCard.Abilities
                .Select(abilityName => {
                    var ability = _abilityProvider.GetModernAbility(abilityName);
                    if (ability == null)
                        UnityEngine.Debug.LogWarning($"[InGameInvocationCard] Ability '{abilityName}' not found for card '{title}'");
                    return ability;
                })
                .Where(ability => ability != null)
                .ToList();

            // Phase 145: Cache abilities as objects to prevent allocation on every interface access
            _abilitiesCache = ModernAbilities.Cast<object>().ToList().AsReadOnly();
        }
        
        // Phase 118: Removed UpdateInvocationCardForAbilities - no longer needed
        // Modern IAbility implementations don't need a reference to the card

        /// <summary>
        /// Checks if the card can be summoned.
        /// </summary>
        /// <param name="playerCards">The player cards.</param>
        /// <returns>true if can be summoned; otherwise, false.</returns>
        public bool CanBeSummoned(IPlayerCardCollection playerCards)
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
        /// Phase 118: Uses ModernAbilities - action abilities can activate when not in passive mode.
        /// </summary>
        /// <returns>true if has action; otherwise, false.</returns>
        public bool HasAction()
        {
            // Phase 118: An ability is an action if it can be manually activated
            // and is not purely passive (doesn't have a specific trigger)
            return ModernAbilities.Any(ability =>
                !(ability is IPassiveAbility passiveAbility) ||
                passiveAbility.Trigger == AbilityTrigger.Continuous);
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
        /// Phase 17-18: Uses ICardCollectionProvider instead of CardManager.Instance.
        /// Phase 24-25: Uses injected _cardCollectionProvider.
        /// </summary>
        /// <returns>true if invocation is possible; otherwise, false.</returns>
        public bool IsInvocationPossible()
        {
            // Phase 24-25: Use injected _cardCollectionProvider
            return CanBeSummoned(_cardCollectionProvider.GetCurrentPlayerCardCollection());
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
            // Phase 118: Removed UpdateInvocationCardForAbilities call
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
        /// Phase 118: Now returns ModernAbilities (legacy Abilities removed).
        /// Phase 145: Now returns cached value to prevent allocation on every access.
        /// </summary>
        IReadOnlyList<object> IInGameInvocationCard.Abilities => _abilitiesCache;

        /// <summary>
        /// Gets the equipment card as an interface type.
        /// Phase 49: Explicit implementation for IInGameInvocationCard interface.
        /// </summary>
        IInGameEquipmentCard IInGameInvocationCard.EquipmentCard => EquipmentCard;

        /// <summary>
        /// Sets the equipment card via interface type.
        /// Phase 72: Added for IInGameInvocationCard interface extension.
        /// Phase 144: Fixed unsafe cast - now logs error instead of silently failing.
        /// Phase 146: Changed to return bool indicating success/failure.
        /// </summary>
        bool IInGameInvocationCard.SetEquipmentCard(IInGameEquipmentCard card)
        {
            if (card == null)
            {
                EquipmentCard = null;
                return true;
            }

            if (card is InGameEquipmentCard equipmentCard)
            {
                EquipmentCard = equipmentCard;
                return true;
            }

            UnityEngine.Debug.LogError($"[InGameInvocationCard.SetEquipmentCard] Expected InGameEquipmentCard but got {card.GetType().Name}. Equipment not assigned.");
            return false;
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