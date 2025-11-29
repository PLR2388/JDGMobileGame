using JDG.Domain;
using JDG.Domain.Events;
using JDG.Application.Repositories;

namespace JDG.Application.Abilities
{
    /// <summary>
    /// Ability that deals damage to a player (reduces HP).
    /// Simple implementation for direct damage effects.
    /// </summary>
    public class DamagePlayerAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IEventBus _eventBus;
        private readonly int _damageAmount;
        private readonly bool _targetOpponent;

        public AbilityName Name { get; }
        public string Description { get; }

        /// <summary>
        /// Creates a damage player ability.
        /// </summary>
        /// <param name="abilityName">The specific ability name</param>
        /// <param name="damageAmount">Amount of damage to deal</param>
        /// <param name="targetOpponent">True to damage opponent, false to damage self</param>
        /// <param name="playerRepository">Repository for accessing player data</param>
        /// <param name="eventBus">Event bus for publishing events</param>
        public DamagePlayerAbility(
            AbilityName abilityName,
            int damageAmount,
            bool targetOpponent,
            IPlayerRepository playerRepository,
            IEventBus eventBus)
        {
            Name = abilityName;
            _damageAmount = damageAmount;
            _targetOpponent = targetOpponent;
            _playerRepository = playerRepository;
            _eventBus = eventBus;
            Description = $"Deal {damageAmount} damage to {(targetOpponent ? "opponent" : "self")}";
        }

        public bool CanActivate(AbilityContext context)
        {
            // Can always attempt to deal damage
            return true;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var targetPlayerId = _targetOpponent ? context.OpponentPlayerId : context.CurrentPlayerId;
            var player = _playerRepository.GetPlayer(targetPlayerId);

            if (player == null)
            {
                return AbilityResult.Failure("Player not found");
            }

            // Deal damage (shields absorb damage first, then HP)
            int healthDamage = player.TakeDamage(_damageAmount);
            _playerRepository.SavePlayer(player);

            // Publish event
            _eventBus.Publish(new PlayerDamagedEvent
            {
                PlayerId = targetPlayerId.ToCardOwner(),
                Damage = _damageAmount,
                HealthDamage = healthDamage,
                CurrentHealth = player.Health,
                IsDefeated = player.IsDefeated
            });

            return AbilityResult.Success($"Dealt {_damageAmount} damage");
        }
    }

    /// <summary>
    /// Factory for creating damage player abilities.
    /// </summary>
    public class DamagePlayerAbilityFactory
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IEventBus _eventBus;

        public DamagePlayerAbilityFactory(
            IPlayerRepository playerRepository,
            IEventBus eventBus)
        {
            _playerRepository = playerRepository;
            _eventBus = eventBus;
        }

        public DamagePlayerAbility CreateDamageOpponent(int amount)
        {
            return new DamagePlayerAbility(
                AbilityName.Default, // Would use specific ability name
                amount,
                targetOpponent: true,
                _playerRepository,
                _eventBus
            );
        }
    }
}
