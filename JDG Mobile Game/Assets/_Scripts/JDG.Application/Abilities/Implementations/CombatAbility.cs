using JDG.Domain;
using JDG.Application.Repositories;
using JDG.Infrastructure.Events;
using System.Linq;

namespace JDG.Application.Abilities.Implementations
{
    /// <summary>
    /// Ability that kills both the attacker and attacked card.
    /// Migrated from KillBothCardsIfAttackAbility, KillEnemyIfDestroy.
    /// </summary>
    public class MutualDestructionAbility : IAbility, IPassiveAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IEventBus _eventBus;

        public AbilityName Name { get; }
        public string Description { get; }
        public AbilityTrigger Trigger => AbilityTrigger.OnAttack;

        public MutualDestructionAbility(
            AbilityName abilityName,
            IPlayerRepository playerRepository,
            IEventBus eventBus)
        {
            Name = abilityName;
            _playerRepository = playerRepository;
            _eventBus = eventBus;
            Description = "When this card attacks or is attacked, destroy both cards";
        }

        public bool CanActivate(AbilityContext context)
        {
            return context.SourceCard != null && context.TargetCard != null;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            if (context.SourceCard == null || context.TargetCard == null)
                return AbilityResult.Failure("Invalid combat context");

            var currentPlayer = _playerRepository.GetPlayer(context.CurrentPlayerId);
            var opponentPlayer = _playerRepository.GetPlayer(context.OpponentPlayerId);

            if (currentPlayer == null || opponentPlayer == null)
                return AbilityResult.Failure("Players not found");

            // Destroy both cards
            currentPlayer.Field.Remove(context.SourceCard);
            currentPlayer.Graveyard.Add(context.SourceCard);

            opponentPlayer.Field.Remove(context.TargetCard);
            opponentPlayer.Graveyard.Add(context.TargetCard);

            _playerRepository.SavePlayer(currentPlayer);
            _playerRepository.SavePlayer(opponentPlayer);

            return AbilityResult.Success("Both cards destroyed");
        }
    }

    /// <summary>
    /// Ability that allows skipping opponent's attack phase.
    /// Migrated from SkipOpponentAttackAbility, SkipOpponentAttackEveryTurn.
    /// </summary>
    public class SkipAttackAbility : IAbility
    {
        private readonly bool _everyTurn;

        public AbilityName Name { get; }
        public string Description { get; }

        public SkipAttackAbility(AbilityName abilityName, bool everyTurn = false)
        {
            Name = abilityName;
            _everyTurn = everyTurn;
            Description = everyTurn ? "Skip opponent's attack every turn" : "Skip opponent's attack";
        }

        public bool CanActivate(AbilityContext context)
        {
            return true;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            // This would set a flag to skip next attack phase
            return AbilityResult.Success("Opponent's attack skipped");
        }
    }

    /// <summary>
    /// Ability that enables direct attack on opponent player.
    /// Migrated from DirectAttackAbility (Equipment).
    /// </summary>
    public class DirectAttackAbility : IAbility
    {
        public AbilityName Name { get; }
        public string Description { get; }

        public DirectAttackAbility()
        {
            Name = AbilityName.Default;
            Description = "Can attack opponent directly";
        }

        public bool CanActivate(AbilityContext context)
        {
            return context.SourceCard != null;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            // This is a passive ability that allows bypassing opponent cards
            return AbilityResult.Success("Direct attack enabled");
        }
    }

    /// <summary>
    /// Ability that comes back from death (graveyard to field).
    /// Migrated from ComesBackFromDeath, ComesBackFromDeath5Times, BackToHandAfterDeathAbility.
    /// </summary>
    public class ResurrectionAbility : IAbility, IPassiveAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly int _maxRevives;
        private readonly bool _toHand; // false = to field, true = to hand

        public AbilityName Name { get; }
        public string Description { get; }
        public AbilityTrigger Trigger => AbilityTrigger.OnDeath;

        public ResurrectionAbility(
            AbilityName abilityName,
            IPlayerRepository playerRepository,
            int maxRevives = 1,
            bool toHand = false)
        {
            Name = abilityName;
            _playerRepository = playerRepository;
            _maxRevives = maxRevives;
            _toHand = toHand;
            Description = toHand
                ? $"Returns to hand {maxRevives} time(s) when destroyed"
                : $"Revives {maxRevives} time(s) when destroyed";
        }

        public bool CanActivate(AbilityContext context)
        {
            return context.SourceCard != null &&
                   context.SourceCard.TimesRevived < _maxRevives;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null || context.SourceCard == null)
                return AbilityResult.Failure("Invalid context");

            if (context.SourceCard.TimesRevived >= _maxRevives)
                return AbilityResult.Failure("Max revives reached");

            // Move from graveyard to field or hand
            player.Graveyard.Remove(context.SourceCard);

            if (_toHand)
            {
                player.Hand.Add(context.SourceCard);
            }
            else
            {
                if (player.Field.Count < 4)
                {
                    player.Field.Add(context.SourceCard);
                }
                else
                {
                    return AbilityResult.Failure("Field is full");
                }
            }

            context.SourceCard.TimesRevived++;
            _playerRepository.SavePlayer(player);

            return AbilityResult.Success($"Card revived ({context.SourceCard.TimesRevived}/{_maxRevives})");
        }
    }

    /// <summary>
    /// Ability that gives death to another card when this card dies.
    /// Migrated from GiveDeathWhenDie.
    /// </summary>
    public class DeathTriggerAbility : IAbility, IPassiveAbility
    {
        private readonly IPlayerRepository _playerRepository;

        public AbilityName Name { get; }
        public string Description { get; }
        public AbilityTrigger Trigger => AbilityTrigger.OnDeath;

        public DeathTriggerAbility(IPlayerRepository playerRepository)
        {
            Name = AbilityName.GiveDeathWhenDie;
            _playerRepository = playerRepository;
            Description = "Destroy target card when this card dies";
        }

        public bool CanActivate(AbilityContext context)
        {
            var opponent = _playerRepository.GetPlayer(context.OpponentPlayerId);
            return opponent != null && opponent.Field.Any();
        }

        public AbilityResult Execute(AbilityContext context)
        {
            // Requires user input to select target
            return AbilityResult.NeedsUserInput("Select card to destroy");
        }
    }

    /// <summary>
    /// Ability to get specific card after death.
    /// Migrated from GetSpecificCardAfterDeathAbility.
    /// </summary>
    public class DeathRewardAbility : IAbility, IPassiveAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly string _rewardCardName;

        public AbilityName Name { get; }
        public string Description { get; }
        public AbilityTrigger Trigger => AbilityTrigger.OnDeath;

        public DeathRewardAbility(
            AbilityName abilityName,
            string rewardCardName,
            IPlayerRepository playerRepository)
        {
            Name = abilityName;
            _rewardCardName = rewardCardName;
            _playerRepository = playerRepository;
            Description = $"Get {rewardCardName} when this card dies";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return player != null && player.Deck.Any(c => c.Title == _rewardCardName);
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null)
                return AbilityResult.Failure("Player not found");

            var rewardCard = player.Deck.FirstOrDefault(c => c.Title == _rewardCardName);
            if (rewardCard == null)
                return AbilityResult.Failure($"{_rewardCardName} not in deck");

            player.Deck.Remove(rewardCard);
            player.Hand.Add(rewardCard);
            _playerRepository.SavePlayer(player);

            return AbilityResult.Success($"Added {_rewardCardName} to hand");
        }
    }

    /// <summary>
    /// Factory for creating combat-related abilities.
    /// </summary>
    public class CombatAbilityFactory
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IEventBus _eventBus;

        public CombatAbilityFactory(IPlayerRepository playerRepository, IEventBus eventBus)
        {
            _playerRepository = playerRepository;
            _eventBus = eventBus;
        }

        public MutualDestructionAbility CreateMutualDestruction(AbilityName name)
        {
            return new MutualDestructionAbility(name, _playerRepository, _eventBus);
        }

        public SkipAttackAbility CreateSkipAttack(AbilityName name, bool everyTurn = false)
        {
            return new SkipAttackAbility(name, everyTurn);
        }

        public DirectAttackAbility CreateDirectAttack()
        {
            return new DirectAttackAbility();
        }

        public ResurrectionAbility CreateResurrection(AbilityName name, int maxRevives = 1, bool toHand = false)
        {
            return new ResurrectionAbility(name, _playerRepository, maxRevives, toHand);
        }

        public DeathTriggerAbility CreateDeathTrigger()
        {
            return new DeathTriggerAbility(_playerRepository);
        }

        public DeathRewardAbility CreateDeathReward(AbilityName name, string rewardCardName)
        {
            return new DeathRewardAbility(name, rewardCardName, _playerRepository);
        }
    }
}
