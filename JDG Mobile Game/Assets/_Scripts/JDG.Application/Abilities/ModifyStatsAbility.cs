using System.Linq;
using JDG.Domain;
using JDG.Domain.Enums;
using JDG.Domain.Events;
using JDG.Application.Repositories;

namespace JDG.Application.Abilities
{
    /// <summary>
    /// Ability that modifies ATK/DEF stats of cards.
    /// Can buff friendly cards or debuff opponent cards.
    /// Phase 159: Changed from int to float for half-star support.
    /// </summary>
    public class ModifyStatsAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IEventBus _eventBus;
        private readonly float _atkModifier;
        private readonly float _defModifier;
        private readonly bool _targetOpponent;
        private readonly CardType _targetCardType;

        public AbilityName Name { get; }
        public string Description { get; }

        /// <summary>
        /// Creates a modify stats ability.
        /// Phase 159: Changed from int to float for half-star support.
        /// </summary>
        /// <param name="abilityName">The specific ability name</param>
        /// <param name="atkModifier">ATK change (positive = buff, negative = debuff)</param>
        /// <param name="defModifier">DEF change (positive = buff, negative = debuff)</param>
        /// <param name="targetOpponent">True to target opponent's cards</param>
        /// <param name="targetCardType">Type of card to target</param>
        /// <param name="playerRepository">Repository for accessing player data</param>
        /// <param name="eventBus">Event bus for publishing events</param>
        public ModifyStatsAbility(
            AbilityName abilityName,
            float atkModifier,
            float defModifier,
            bool targetOpponent,
            CardType targetCardType,
            IPlayerRepository playerRepository,
            IEventBus eventBus)
        {
            Name = abilityName;
            _atkModifier = atkModifier;
            _defModifier = defModifier;
            _targetOpponent = targetOpponent;
            _targetCardType = targetCardType;
            _playerRepository = playerRepository;
            _eventBus = eventBus;

            string sign = (atkModifier >= 0 && defModifier >= 0) ? "+" : "";
            Description = $"Modify {targetCardType} stats by {sign}{atkModifier}/{sign}{defModifier}";
        }

        public bool CanActivate(AbilityContext context)
        {
            var targetPlayerId = _targetOpponent ? context.OpponentPlayerId : context.CurrentPlayerId;
            var player = _playerRepository.GetPlayer(targetPlayerId);
            return player != null && player.Field.Any(c => c.Type == _targetCardType);
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var targetPlayerId = _targetOpponent ? context.OpponentPlayerId : context.CurrentPlayerId;
            var player = _playerRepository.GetPlayer(targetPlayerId);

            if (player == null)
            {
                return AbilityResult.Failure("Player not found");
            }

            // Find all cards of target type on field
            var targetCards = player.Field.Where(c => c.Type == _targetCardType).ToList();

            if (targetCards.Count == 0)
            {
                return AbilityResult.Failure($"No {_targetCardType} cards on field");
            }

            // Modify stats for all matching cards
            int cardsModified = 0;
            foreach (var card in targetCards)
            {
                card.ModifyStats(_atkModifier, _defModifier);
                cardsModified++;

                // Publish event for each card
                _eventBus.Publish(new CardStatsModifiedEvent
                {
                    CardId = card.Id.ToGuid(),
                    Owner = targetPlayerId.ToCardOwner(),
                    AtkChange = _atkModifier,
                    DefChange = _defModifier,
                    NewAtk = card.Stats?.Attack ?? 0,
                    NewDef = card.Stats?.Defense ?? 0
                });
            }

            _playerRepository.SavePlayer(player);

            return AbilityResult.Success($"Modified stats of {cardsModified} card(s)");
        }
    }

    /// <summary>
    /// Factory for creating modify stats abilities.
    /// </summary>
    public class ModifyStatsAbilityFactory
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IEventBus _eventBus;

        public ModifyStatsAbilityFactory(
            IPlayerRepository playerRepository,
            IEventBus eventBus)
        {
            _playerRepository = playerRepository;
            _eventBus = eventBus;
        }

        public ModifyStatsAbility CreateBuff(float atk, float def, CardType targetType)
        {
            return new ModifyStatsAbility(
                AbilityName.Default,
                atk,
                def,
                targetOpponent: false,
                targetType,
                _playerRepository,
                _eventBus
            );
        }

        public ModifyStatsAbility CreateDebuff(float atk, float def, CardType targetType)
        {
            return new ModifyStatsAbility(
                AbilityName.Default,
                -atk,
                -def,
                targetOpponent: true,
                targetType,
                _playerRepository,
                _eventBus
            );
        }
    }
}
