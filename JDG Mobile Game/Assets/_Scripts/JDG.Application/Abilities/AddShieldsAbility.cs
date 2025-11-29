using JDG.Domain;
using JDG.Domain.Events;
using JDG.Application.Repositories;

namespace JDG.Application.Abilities
{
    /// <summary>
    /// Ability that adds shields to a player.
    /// Shields absorb damage before HP is affected.
    /// </summary>
    public class AddShieldsAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IEventBus _eventBus;
        private readonly int _shieldAmount;

        public AbilityName Name { get; }
        public string Description { get; }

        /// <summary>
        /// Creates an add shields ability.
        /// </summary>
        /// <param name="abilityName">The specific ability name</param>
        /// <param name="shieldAmount">Number of shields to add</param>
        /// <param name="playerRepository">Repository for accessing player data</param>
        /// <param name="eventBus">Event bus for publishing events</param>
        public AddShieldsAbility(
            AbilityName abilityName,
            int shieldAmount,
            IPlayerRepository playerRepository,
            IEventBus eventBus)
        {
            Name = abilityName;
            _shieldAmount = shieldAmount;
            _playerRepository = playerRepository;
            _eventBus = eventBus;
            Description = $"Add {shieldAmount} shield(s)";
        }

        public bool CanActivate(AbilityContext context)
        {
            // Can always add shields
            return true;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);

            if (player == null)
            {
                return AbilityResult.Failure("Player not found");
            }

            player.AddShields(_shieldAmount);
            _playerRepository.SavePlayer(player);

            // Publish event
            _eventBus.Publish(new ShieldsAddedEvent
            {
                PlayerId = context.CurrentPlayerId.ToCardOwner(),
                ShieldsAdded = _shieldAmount,
                NewShields = player.Shields
            });

            return AbilityResult.Success($"Added {_shieldAmount} shield(s)");
        }
    }

    /// <summary>
    /// Factory for creating add shields abilities.
    /// </summary>
    public class AddShieldsAbilityFactory
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IEventBus _eventBus;

        public AddShieldsAbilityFactory(
            IPlayerRepository playerRepository,
            IEventBus eventBus)
        {
            _playerRepository = playerRepository;
            _eventBus = eventBus;
        }

        public AddShieldsAbility CreateAddShields(int amount)
        {
            return new AddShieldsAbility(
                AbilityName.Default,
                amount,
                _playerRepository,
                _eventBus
            );
        }
    }
}
