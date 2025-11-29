using JDG.Domain;
using JDG.Domain.Events;
using JDG.Application.Repositories;

namespace JDG.Application.Abilities
{
    /// <summary>
    /// Ability that restores HP to a player.
    /// Simple implementation for healing effects.
    /// </summary>
    public class HealPlayerAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IEventBus _eventBus;
        private readonly int _healAmount;

        public AbilityName Name { get; }
        public string Description { get; }

        /// <summary>
        /// Creates a heal player ability.
        /// </summary>
        /// <param name="abilityName">The specific ability name</param>
        /// <param name="healAmount">Amount of HP to restore</param>
        /// <param name="playerRepository">Repository for accessing player data</param>
        /// <param name="eventBus">Event bus for publishing events</param>
        public HealPlayerAbility(
            AbilityName abilityName,
            int healAmount,
            IPlayerRepository playerRepository,
            IEventBus eventBus)
        {
            Name = abilityName;
            _healAmount = healAmount;
            _playerRepository = playerRepository;
            _eventBus = eventBus;
            Description = $"Restore {healAmount} HP";
        }

        public bool CanActivate(AbilityContext context)
        {
            // Can heal if player is not at max HP
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return player != null && player.HP < player.MaxHP;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);

            if (player == null)
            {
                return AbilityResult.Failure("Player not found");
            }

            int oldHP = player.HP;
            player.Heal(_healAmount);
            _playerRepository.SavePlayer(player);

            int actualHealed = player.HP - oldHP;

            // Publish event
            _eventBus.Publish(new PlayerHealedEvent
            {
                PlayerId = context.CurrentPlayerId.ToCardOwner(),
                HealAmount = actualHealed,
                NewHP = player.HP
            });

            return AbilityResult.Success($"Restored {actualHealed} HP");
        }
    }

    /// <summary>
    /// Factory for creating heal player abilities.
    /// </summary>
    public class HealPlayerAbilityFactory
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IEventBus _eventBus;

        public HealPlayerAbilityFactory(
            IPlayerRepository playerRepository,
            IEventBus eventBus)
        {
            _playerRepository = playerRepository;
            _eventBus = eventBus;
        }

        public HealPlayerAbility CreateHeal(int amount)
        {
            return new HealPlayerAbility(
                AbilityName.Default,
                amount,
                _playerRepository,
                _eventBus
            );
        }
    }
}
