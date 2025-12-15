using JDG.Domain;
using JDG.Domain.Enums;
using JDG.Application.Repositories;
using System.Linq;

namespace JDG.Application.Abilities.Implementations
{
    /// <summary>
    /// Effect ability that swaps ATK and DEF of cards.
    /// Migrated from SwitchAtkDefEffectAbility.
    /// </summary>
    public class SwapStatsEffectAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly bool _targetOpponent;

        public AbilityName Name { get; }
        public string Description { get; }

        public SwapStatsEffectAbility(
            IPlayerRepository playerRepository,
            bool targetOpponent = false)
        {
            Name = AbilityName.Default;
            _playerRepository = playerRepository;
            _targetOpponent = targetOpponent;
            Description = targetOpponent ? "Swap opponent's ATK/DEF" : "Swap your ATK/DEF";
        }

        public bool CanActivate(AbilityContext context)
        {
            var targetId = _targetOpponent ? context.OpponentPlayerId : context.CurrentPlayerId;
            var player = _playerRepository.GetPlayer(targetId);
            return player != null && player.Field.Any();
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var targetId = _targetOpponent ? context.OpponentPlayerId : context.CurrentPlayerId;
            var player = _playerRepository.GetPlayer(targetId);
            if (player == null)
                return AbilityResult.Failure("Player not found");

            foreach (var card in player.Field)
            {
                if (card.Stats.HasValue)
                {
                    var tempAtk = card.Stats.Value.Attack;
                    card.SetStats(card.Stats.Value.Defense, tempAtk);
                }
            }

            _playerRepository.SavePlayer(player);
            return AbilityResult.Success($"Swapped ATK/DEF for {player.Field.Count} cards");
        }
    }

    /// <summary>
    /// Effect ability that divides opponent's DEF.
    /// Migrated from DivideDEFOpponentEffectAbility.
    /// </summary>
    public class DivideDefenseEffectAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly int _divisor;

        public AbilityName Name { get; }
        public string Description { get; }

        public DivideDefenseEffectAbility(
            IPlayerRepository playerRepository,
            int divisor = 2)
        {
            Name = AbilityName.Default;
            _playerRepository = playerRepository;
            _divisor = divisor;
            Description = $"Divide opponent's DEF by {divisor}";
        }

        public bool CanActivate(AbilityContext context)
        {
            var opponent = _playerRepository.GetPlayer(context.OpponentPlayerId);
            return opponent != null && opponent.Field.Any();
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var opponent = _playerRepository.GetPlayer(context.OpponentPlayerId);
            if (opponent == null)
                return AbilityResult.Failure("Opponent not found");

            foreach (var card in opponent.Field)
            {
                if (card.Stats.HasValue)
                {
                    int newDef = card.Stats.Value.Defense / _divisor;
                    card.SetStats(card.Stats.Value.Attack, newDef);
                }
            }

            _playerRepository.SavePlayer(opponent);
            return AbilityResult.Success($"Divided DEF for {opponent.Field.Count} cards");
        }
    }

    /// <summary>
    /// Effect ability that allows controlling opponent's invocation.
    /// Migrated from ControlOpponentInvocationCardEffectAbility.
    /// </summary>
    public class ControlCardEffectAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;

        public AbilityName Name { get; }
        public string Description { get; }

        public ControlCardEffectAbility(IPlayerRepository playerRepository)
        {
            Name = AbilityName.Default;
            _playerRepository = playerRepository;
            Description = "Take control of opponent's invocation";
        }

        public bool CanActivate(AbilityContext context)
        {
            var opponent = _playerRepository.GetPlayer(context.OpponentPlayerId);
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return opponent != null && opponent.Field.Any() &&
                   player != null && player.Field.Count < 4;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            // Requires user input to select which card to control
            return AbilityResult.NeedsUserInput("Select opponent's card to control");
        }
    }

    /// <summary>
    /// Effect ability that changes field card.
    /// Migrated from ChangeFieldCardEffectAbility.
    /// </summary>
    public class ChangeFieldEffectAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly bool _targetOpponent;

        public AbilityName Name { get; }
        public string Description { get; }

        public ChangeFieldEffectAbility(
            IPlayerRepository playerRepository,
            bool targetOpponent = false)
        {
            Name = AbilityName.Default;
            _playerRepository = playerRepository;
            _targetOpponent = targetOpponent;
            Description = targetOpponent ? "Change opponent's field card" : "Change your field card";
        }

        public bool CanActivate(AbilityContext context)
        {
            var targetId = _targetOpponent ? context.OpponentPlayerId : context.CurrentPlayerId;
            var player = _playerRepository.GetPlayer(targetId);
            return player != null && player.Field.Any(c => c.Type == CardType.Field);
        }

        public AbilityResult Execute(AbilityContext context)
        {
            // Requires user to select new field card from deck
            return AbilityResult.NeedsUserInput("Select field card from deck");
        }
    }

    /// <summary>
    /// Effect ability that destroys multiple cards.
    /// Migrated from DestroyCardsEffectAbility.
    /// </summary>
    public class DestroyMultipleCardsEffectAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly int _count;

        public AbilityName Name { get; }
        public string Description { get; }

        public DestroyMultipleCardsEffectAbility(
            IPlayerRepository playerRepository,
            int count)
        {
            Name = AbilityName.Default;
            _playerRepository = playerRepository;
            _count = count;
            Description = $"Destroy {count} opponent card(s)";
        }

        public bool CanActivate(AbilityContext context)
        {
            var opponent = _playerRepository.GetPlayer(context.OpponentPlayerId);
            return opponent != null && opponent.Field.Count >= _count;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            return AbilityResult.NeedsUserInput($"Select {_count} cards to destroy");
        }
    }

    /// <summary>
    /// Effect ability that limits hand size.
    /// Migrated from LimitHandCardsEffectAbility.
    /// </summary>
    public class LimitHandEffectAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly int _maxHandSize;

        public AbilityName Name { get; }
        public string Description { get; }

        public LimitHandEffectAbility(
            IPlayerRepository playerRepository,
            int maxHandSize = 5)
        {
            Name = AbilityName.Default;
            _playerRepository = playerRepository;
            _maxHandSize = maxHandSize;
            Description = $"Opponent's hand limited to {maxHandSize} cards";
        }

        public bool CanActivate(AbilityContext context)
        {
            return true;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var opponent = _playerRepository.GetPlayer(context.OpponentPlayerId);
            if (opponent == null)
                return AbilityResult.Failure("Opponent not found");

            if (opponent.Hand.Count > _maxHandSize)
            {
                int cardsToDiscard = opponent.Hand.Count - _maxHandSize;
                return AbilityResult.NeedsUserInput($"Opponent must discard {cardsToDiscard} cards");
            }

            return AbilityResult.Success("Hand limit set");
        }
    }

    /// <summary>
    /// Effect ability that allows looking at opponent's hand.
    /// Migrated from LookHandCardsEffectAbility.
    /// </summary>
    public class LookHandEffectAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;

        public AbilityName Name { get; }
        public string Description { get; }

        public LookHandEffectAbility(IPlayerRepository playerRepository)
        {
            Name = AbilityName.Default;
            _playerRepository = playerRepository;
            Description = "Look at opponent's hand";
        }

        public bool CanActivate(AbilityContext context)
        {
            var opponent = _playerRepository.GetPlayer(context.OpponentPlayerId);
            return opponent != null && opponent.Hand.Any();
        }

        public AbilityResult Execute(AbilityContext context)
        {
            return AbilityResult.NeedsUserInput("View opponent's hand");
        }
    }

    /// <summary>
    /// Effect ability that allows looking at top cards of deck.
    /// Migrated from LookDeckCardsEffectAbility.
    /// </summary>
    public class LookDeckEffectAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly int _cardCount;

        public AbilityName Name { get; }
        public string Description { get; }

        public LookDeckEffectAbility(
            IPlayerRepository playerRepository,
            int cardCount = 3)
        {
            Name = AbilityName.Default;
            _playerRepository = playerRepository;
            _cardCount = cardCount;
            Description = $"Look at top {cardCount} cards of deck";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return player != null && player.Deck.Count >= _cardCount;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            return AbilityResult.NeedsUserInput($"View top {_cardCount} cards");
        }
    }

    /// <summary>
    /// Effect ability that invokes card from deck to field.
    /// Migrated from InvokeCardFromDeckYellowEffectAbility.
    /// </summary>
    public class InvokFromDeckEffectAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;

        public AbilityName Name { get; }
        public string Description { get; }

        public InvokFromDeckEffectAbility(IPlayerRepository playerRepository)
        {
            Name = AbilityName.Default;
            _playerRepository = playerRepository;
            Description = "Invoke card from deck to field";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return player != null &&
                   player.Field.Count < 4 &&
                   player.Deck.Any(c => c.Type == CardType.Invocation);
        }

        public AbilityResult Execute(AbilityContext context)
        {
            return AbilityResult.NeedsUserInput("Select card to invoke from deck");
        }
    }

    /// <summary>
    /// Factory for creating effect abilities.
    /// </summary>
    public class EffectAbilityFactory
    {
        private readonly IPlayerRepository _playerRepository;

        public EffectAbilityFactory(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public SwapStatsEffectAbility CreateSwapStats(bool targetOpponent = false)
        {
            return new SwapStatsEffectAbility(_playerRepository, targetOpponent);
        }

        public DivideDefenseEffectAbility CreateDivideDefense(int divisor = 2)
        {
            return new DivideDefenseEffectAbility(_playerRepository, divisor);
        }

        public ControlCardEffectAbility CreateControlCard()
        {
            return new ControlCardEffectAbility(_playerRepository);
        }

        public ChangeFieldEffectAbility CreateChangeField(bool targetOpponent = false)
        {
            return new ChangeFieldEffectAbility(_playerRepository, targetOpponent);
        }

        public DestroyMultipleCardsEffectAbility CreateDestroyMultiple(int count)
        {
            return new DestroyMultipleCardsEffectAbility(_playerRepository, count);
        }

        public LimitHandEffectAbility CreateLimitHand(int maxSize = 5)
        {
            return new LimitHandEffectAbility(_playerRepository, maxSize);
        }

        public LookHandEffectAbility CreateLookHand()
        {
            return new LookHandEffectAbility(_playerRepository);
        }

        public LookDeckEffectAbility CreateLookDeck(int cardCount = 3)
        {
            return new LookDeckEffectAbility(_playerRepository, cardCount);
        }

        public InvokFromDeckEffectAbility CreateInvokeFromDeck()
        {
            return new InvokFromDeckEffectAbility(_playerRepository);
        }
    }
}
