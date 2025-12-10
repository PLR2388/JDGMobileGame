using System.Collections.Generic;
using System.Linq;
using _Scripts.Cards.InvocationCards;
using Cards;
using Cards.InvocationCards;
using JDG.Application;
using JDG.Domain.Events;

namespace _Scripts.Units.Invocation
{
    /// <summary>
    /// Phase 17-18: Removed CardManager singleton dependency via ICardCollectionService.
    /// Phase 24-25: Removed ServiceLocator, using constructor injection.
    /// </summary>
    public class InGameInvocationCard : InGameCard
    {
        public InvocationCard BaseInvocationCard;
        private bool blockAttackNextTurn;
        private int remainedAttackThisTurn;
        private bool cancelEffect;

        private const int DefaultNumberAttacksPerTurn = 1;

        // Phase 24-25: Injected dependencies
        private readonly IEventBus _eventBus;
        private readonly ICardCollectionService _cardCollectionService;

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
        public List<Ability> Abilities = new List<Ability>();


        public int NumberOfTurnOnField { get; private set; }

        public int NumberOfDeaths { get; private set; }

        public InGameEquipmentCard EquipmentCard { get; set; }

        /// <summary>
        /// Initializes an instance of the InGameInvocationCard.
        /// Phase 24-25: Added dependency injection for IEventBus and ICardCollectionService.
        /// </summary>
        /// <param name="invocationCard">The base invocation card.</param>
        /// <param name="cardOwner">The owner of the card.</param>
        /// <param name="eventBus">EventBus for publishing domain events.</param>
        /// <param name="cardCollectionService">Service for accessing player cards.</param>
        /// <returns>A new InGameInvocationCard instance.</returns>
        public InGameInvocationCard(InvocationCard invocationCard, CardOwner cardOwner, IEventBus eventBus, ICardCollectionService cardCollectionService)
        {
            BaseInvocationCard = invocationCard;
            CardOwner = cardOwner;
            _eventBus = eventBus;
            _cardCollectionService = cardCollectionService;
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
            conditions = BaseInvocationCard.Conditions
                .Select(conditionName => ConditionLibrary.Instance.ConditionDictionary[conditionName]).ToList();
            Abilities = BaseInvocationCard.Abilities
                .Select(abilityName => AbilityLibrary.Instance.AbilityDictionary[abilityName]).ToList();
            UpdateInvocationCardForAbilities();
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
    }
}