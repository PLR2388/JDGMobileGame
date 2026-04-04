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
                    // Phase 159: Changed to float for half-star support
                    float newDef = card.Stats.Value.Defense / _divisor;
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

        public DamageOpponentEffectAbility CreateDamageOpponent(DamageCalculationType calcType, float damagePerUnit)
        {
            return new DamageOpponentEffectAbility(_playerRepository, calcType, damagePerUnit);
        }

        public HealPlayerEffectAbility CreateHealPlayer(float healAmount, int minStatRequired = 0)
        {
            return new HealPlayerEffectAbility(_playerRepository, healAmount, minStatRequired);
        }

        public EnableDirectAttackEffectAbility CreateEnableDirectAttack(float hpThreshold)
        {
            return new EnableDirectAttackEffectAbility(_playerRepository, hpThreshold);
        }

        public DestroyFieldCardEffectAbility CreateDestroyFieldCard(float hpCost)
        {
            return new DestroyFieldCardEffectAbility(_playerRepository, hpCost);
        }

        public DrawFromGraveyardEffectAbility CreateDrawFromGraveyard()
        {
            return new DrawFromGraveyardEffectAbility(_playerRepository);
        }

        public SkipAttackPhaseEffectAbility CreateSkipAttackPhase()
        {
            return new SkipAttackPhaseEffectAbility(_playerRepository);
        }

        public DoubleAttacksEffectAbility CreateDoubleAttacks(int bonusAttacks = 1)
        {
            return new DoubleAttacksEffectAbility(_playerRepository, bonusAttacks);
        }

        public AddShieldsEffectAbility CreateAddShields(int shieldCount)
        {
            return new AddShieldsEffectAbility(_playerRepository, shieldCount);
        }

        public ApplyFamilyFieldEffectAbility CreateApplyFamilyField(float hpCostPerTurn)
        {
            return new ApplyFamilyFieldEffectAbility(_playerRepository, hpCostPerTurn);
        }
    }

    /// <summary>
    /// Calculation type for damage-based effect abilities.
    /// </summary>
    public enum DamageCalculationType
    {
        ByPlayerInvocationCount,
        ByOpponentInvocationCount,
        ByOpponentHandCount
    }

    /// <summary>
    /// Effect ability that damages opponent based on card counts.
    /// Migrated from LoseHPOpponentEffectAbility.
    /// </summary>
    public class DamageOpponentEffectAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly DamageCalculationType _calculationType;
        private readonly float _damagePerUnit;

        public AbilityName Name { get; }
        public string Description { get; }

        public DamageOpponentEffectAbility(
            IPlayerRepository playerRepository,
            DamageCalculationType calculationType,
            float damagePerUnit)
        {
            Name = AbilityName.Default;
            _playerRepository = playerRepository;
            _calculationType = calculationType;
            _damagePerUnit = damagePerUnit;
            Description = $"Deal {damagePerUnit} damage per {calculationType}";
        }

        public bool CanActivate(AbilityContext context)
        {
            return true;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var opponent = _playerRepository.GetPlayer(context.OpponentPlayerId);
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (opponent == null || player == null)
                return AbilityResult.Failure("Player not found");

            int count = _calculationType switch
            {
                DamageCalculationType.ByPlayerInvocationCount => player.Field.Count,
                DamageCalculationType.ByOpponentInvocationCount => opponent.Field.Count,
                DamageCalculationType.ByOpponentHandCount => opponent.Hand.Count,
                _ => 0
            };

            float totalDamage = count * _damagePerUnit;
            opponent.TakeDamage(totalDamage);
            _playerRepository.SavePlayer(opponent);

            return AbilityResult.Success($"Dealt {totalDamage} damage to opponent");
        }
    }

    /// <summary>
    /// Effect ability that heals the player.
    /// Migrated from GetHPBackEffectAbility.
    /// </summary>
    public class HealPlayerEffectAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly float _healAmount;
        private readonly int _minStatRequired;

        public AbilityName Name { get; }
        public string Description { get; }

        public HealPlayerEffectAbility(
            IPlayerRepository playerRepository,
            float healAmount,
            int minStatRequired = 0)
        {
            Name = AbilityName.Default;
            _playerRepository = playerRepository;
            _healAmount = healAmount;
            _minStatRequired = minStatRequired;
            Description = healAmount > 0
                ? $"Heal {healAmount} HP"
                : "Restore all HP";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null) return false;

            // Check if sacrifice with minimum stat is required
            if (_minStatRequired > 0)
            {
                return player.Field.Any(c =>
                    c.Stats.HasValue &&
                    (c.Stats.Value.Attack >= _minStatRequired || c.Stats.Value.Defense >= _minStatRequired));
            }
            return player.Field.Any();
        }

        public AbilityResult Execute(AbilityContext context)
        {
            // Requires user to select card to sacrifice
            return AbilityResult.NeedsUserInput("Select invocation to sacrifice for healing");
        }
    }

    /// <summary>
    /// Effect ability that enables direct attack when opponent HP is low.
    /// Migrated from DirectAttackEffectAbility.
    /// </summary>
    public class EnableDirectAttackEffectAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly float _hpThreshold;

        public AbilityName Name { get; }
        public string Description { get; }

        public EnableDirectAttackEffectAbility(
            IPlayerRepository playerRepository,
            float hpThreshold)
        {
            Name = AbilityName.Default;
            _playerRepository = playerRepository;
            _hpThreshold = hpThreshold;
            Description = $"Enable direct attack when opponent HP <= {hpThreshold}";
        }

        public bool CanActivate(AbilityContext context)
        {
            var opponent = _playerRepository.GetPlayer(context.OpponentPlayerId);
            return opponent != null && opponent.Health <= _hpThreshold;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null)
                return AbilityResult.Failure("Player not found");

            // Enable direct attack for all invocations
            foreach (var card in player.Field.Where(c => c.Type == CardType.Invocation))
            {
                card.EnableDirectAttack();
            }

            _playerRepository.SavePlayer(player);
            return AbilityResult.Success("Direct attack enabled for all invocations");
        }
    }

    /// <summary>
    /// Effect ability that destroys field cards.
    /// Migrated from DestroyFieldCardAbility.
    /// </summary>
    public class DestroyFieldCardEffectAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly float _hpCost;

        public AbilityName Name { get; }
        public string Description { get; }

        public DestroyFieldCardEffectAbility(
            IPlayerRepository playerRepository,
            float hpCost)
        {
            Name = AbilityName.Default;
            _playerRepository = playerRepository;
            _hpCost = hpCost;
            Description = $"Destroy field card for {hpCost} HP";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            var opponent = _playerRepository.GetPlayer(context.OpponentPlayerId);

            bool hasEnoughHp = player != null && player.Health > _hpCost;
            bool opponentHasField = opponent != null && opponent.Field.Any(c => c.Type == CardType.Field);

            return hasEnoughHp && opponentHasField;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            var opponent = _playerRepository.GetPlayer(context.OpponentPlayerId);

            if (player == null || opponent == null)
                return AbilityResult.Failure("Player not found");

            // Pay HP cost
            player.TakeDamage(_hpCost);
            _playerRepository.SavePlayer(player);

            // Destroy opponent's field card
            var fieldCard = opponent.Field.FirstOrDefault(c => c.Type == CardType.Field);
            if (fieldCard != null)
            {
                opponent.DestroyCardFromField(fieldCard);
                _playerRepository.SavePlayer(opponent);
                return AbilityResult.Success($"Destroyed opponent's field card for {_hpCost} HP");
            }

            return AbilityResult.Failure("No field card to destroy");
        }
    }

    /// <summary>
    /// Effect ability that draws from graveyard or deck.
    /// Migrated from GetCardFromDeckYellowEffectAbility.
    /// </summary>
    public class DrawFromGraveyardEffectAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;

        public AbilityName Name { get; }
        public string Description { get; }

        public DrawFromGraveyardEffectAbility(IPlayerRepository playerRepository)
        {
            Name = AbilityName.Default;
            _playerRepository = playerRepository;
            Description = "Draw a card from graveyard or deck";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return player != null && (player.Graveyard.Any() || player.Deck.Any());
        }

        public AbilityResult Execute(AbilityContext context)
        {
            return AbilityResult.NeedsUserInput("Select card from graveyard or deck");
        }
    }

    /// <summary>
    /// Effect ability that skips opponent's attack phase.
    /// Migrated from SkipOpponentAttackEffectAbility.
    /// </summary>
    public class SkipAttackPhaseEffectAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;

        public AbilityName Name { get; }
        public string Description { get; }

        public SkipAttackPhaseEffectAbility(IPlayerRepository playerRepository)
        {
            Name = AbilityName.Default;
            _playerRepository = playerRepository;
            Description = "Skip opponent's attack phase";
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

            // Block all opponent invocations from attacking
            foreach (var card in opponent.Field.Where(c => c.Type == CardType.Invocation))
            {
                card.BlockAttack();
            }

            _playerRepository.SavePlayer(opponent);
            return AbilityResult.Success("Opponent's attack phase skipped");
        }
    }

    /// <summary>
    /// Effect ability that gives bonus attacks per turn.
    /// Migrated from IncrementNumberAttackEffectAbility.
    /// </summary>
    public class DoubleAttacksEffectAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly int _bonusAttacks;

        public AbilityName Name { get; }
        public string Description { get; }

        public DoubleAttacksEffectAbility(
            IPlayerRepository playerRepository,
            int bonusAttacks = 1)
        {
            Name = AbilityName.Default;
            _playerRepository = playerRepository;
            _bonusAttacks = bonusAttacks;
            Description = $"Gain {_bonusAttacks + 1} attacks this turn";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return player != null && player.Field.Any(c => c.Type == CardType.Invocation);
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null)
                return AbilityResult.Failure("Player not found");

            foreach (var card in player.Field.Where(c => c.Type == CardType.Invocation))
            {
                card.SetBonusAttacks(_bonusAttacks);
            }

            _playerRepository.SavePlayer(player);
            return AbilityResult.Success($"All invocations gain {_bonusAttacks} bonus attacks");
        }
    }

    /// <summary>
    /// Effect ability that adds damage shields to the player.
    /// Migrated from AddShieldsForUserEffectAbility.
    /// </summary>
    public class AddShieldsEffectAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly int _shieldCount;

        public AbilityName Name { get; }
        public string Description { get; }

        public AddShieldsEffectAbility(
            IPlayerRepository playerRepository,
            int shieldCount)
        {
            Name = AbilityName.Default;
            _playerRepository = playerRepository;
            _shieldCount = shieldCount;
            Description = $"Add {shieldCount} shields";
        }

        public bool CanActivate(AbilityContext context)
        {
            return true;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null)
                return AbilityResult.Failure("Player not found");

            player.AddShields(_shieldCount);
            _playerRepository.SavePlayer(player);
            return AbilityResult.Success($"Added {_shieldCount} shields");
        }
    }

    /// <summary>
    /// Effect ability that applies field card family to invocations.
    /// Migrated from FamilyFieldToInvocationsEffectAbility.
    /// </summary>
    public class ApplyFamilyFieldEffectAbility : IPassiveAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly float _hpCostPerTurn;

        public AbilityName Name { get; }
        public string Description { get; }
        public AbilityTrigger Trigger => AbilityTrigger.OnTurnStart;

        public ApplyFamilyFieldEffectAbility(
            IPlayerRepository playerRepository,
            float hpCostPerTurn)
        {
            Name = AbilityName.Default;
            _playerRepository = playerRepository;
            _hpCostPerTurn = hpCostPerTurn;
            Description = $"Apply field family to invocations for {hpCostPerTurn} HP/turn";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return player != null &&
                   player.Health > _hpCostPerTurn &&
                   player.Field.Any(c => c.Type == CardType.Field);
        }

        public AbilityResult Execute(AbilityContext context)
        {
            // User must confirm they want to pay the HP cost
            return AbilityResult.NeedsUserInput($"Pay {_hpCostPerTurn} HP to apply field family to invocations?");
        }
    }
}
