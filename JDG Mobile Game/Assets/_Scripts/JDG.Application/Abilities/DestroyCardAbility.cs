using System.Linq;
using JDG.Domain;
using JDG.Domain.Enums;
using JDG.Application.Repositories;

namespace JDG.Application.Abilities
{
    /// <summary>
    /// Ability that destroys an opponent's invocation card.
    /// Demonstrates interaction with player field cards through repositories.
    /// </summary>
    public class DestroyCardAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IEventBus _eventBus;
        private readonly CardType _targetCardType;

        public AbilityName Name { get; }
        public string Description { get; }

        /// <summary>
        /// Creates a new destroy card ability.
        /// </summary>
        /// <param name="abilityName">The specific ability name</param>
        /// <param name="targetCardType">Type of card that can be destroyed</param>
        /// <param name="playerRepository">Repository for accessing player data</param>
        /// <param name="eventBus">Event bus for publishing card destroyed events</param>
        public DestroyCardAbility(
            AbilityName abilityName,
            CardType targetCardType,
            IPlayerRepository playerRepository,
            IEventBus eventBus)
        {
            Name = abilityName;
            _targetCardType = targetCardType;
            _playerRepository = playerRepository;
            _eventBus = eventBus;
            Description = $"Destroy an opponent's {targetCardType} card";
        }

        public bool CanActivate(AbilityContext context)
        {
            // Check if opponent has any cards of the target type on the field
            var opponent = _playerRepository.GetPlayer(context.OpponentPlayerId);
            return opponent != null && opponent.Field.Any(card => card.Type == _targetCardType);
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var opponent = _playerRepository.GetPlayer(context.OpponentPlayerId);

            if (opponent == null)
            {
                return AbilityResult.Failure("Opponent not found");
            }

            // Find first card of target type on opponent's field
            var targetCard = opponent.Field.FirstOrDefault(card => card.Type == _targetCardType);

            if (targetCard == null)
            {
                return AbilityResult.Failure($"No {_targetCardType} cards on opponent's field");
            }

            // NOTE: In a full implementation, you would need a use case for destroying cards
            // For now, this demonstrates the pattern - the ability determines WHAT to do,
            // and a use case would handle HOW to do it

            // This would be: var result = _destroyCardUseCase.Execute(opponentId, targetCard.Id);
            // For demonstration, we'll just show the pattern:

            // Move card from field to graveyard (simplified - should be in a use case)
            bool removed = opponent.RemoveCardFromField(targetCard);

            if (removed)
            {
                opponent.AddCardToGraveyard(targetCard);
                _playerRepository.SavePlayer(opponent);

                // Publish event
                _eventBus.Publish(new CardDestroyedEvent
                {
                    CardId = targetCard.Id.ToGuid(),
                    CardTitle = targetCard.Title,
                    Owner = context.OpponentPlayerId.ToCardOwner(),
                    DestroyedBy = context.CurrentPlayerId.ToCardOwner()
                });

                return AbilityResult.Success($"Destroyed {targetCard.Title}");
            }

            return AbilityResult.Failure("Failed to destroy card");
        }
    }

    /// <summary>
    /// Factory for creating destroy card abilities with dependency injection.
    /// </summary>
    public class DestroyCardAbilityFactory
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IEventBus _eventBus;

        public DestroyCardAbilityFactory(
            IPlayerRepository playerRepository,
            IEventBus eventBus)
        {
            _playerRepository = playerRepository;
            _eventBus = eventBus;
        }

        /// <summary>
        /// Creates a "Kill Opponent Invocation" ability.
        /// </summary>
        public DestroyCardAbility CreateKillOpponentInvocation()
        {
            return new DestroyCardAbility(
                AbilityName.KillOpponentInvocation,
                CardType.Invocation,
                _playerRepository,
                _eventBus
            );
        }

        /// <summary>
        /// Creates a "Destroy Field" ability (attack or defense version).
        /// </summary>
        public DestroyCardAbility CreateDestroyField()
        {
            return new DestroyCardAbility(
                AbilityName.DestroyFieldATK,  // Or DestroyFieldDEF
                CardType.Field,
                _playerRepository,
                _eventBus
            );
        }
    }
}
