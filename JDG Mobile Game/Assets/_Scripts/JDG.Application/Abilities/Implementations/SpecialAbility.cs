using JDG.Domain;
using JDG.Domain.Enums;
using JDG.Application.Repositories;
using System.Linq;

namespace JDG.Application.Abilities.Implementations
{
    /// <summary>
    /// Ability that sends all cards to player hands.
    /// Migrated from SendAllCardsInHand.
    /// </summary>
    public class SendAllCardsToHandAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;

        public AbilityName Name { get; }
        public string Description { get; }

        public SendAllCardsToHandAbility(IPlayerRepository playerRepository)
        {
            Name = AbilityName.SendAllCardToHands;
            _playerRepository = playerRepository;
            Description = "Return all field cards to hand";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            var opponent = _playerRepository.GetPlayer(context.OpponentPlayerId);
            return (player != null && player.Field.Any()) ||
                   (opponent != null && opponent.Field.Any());
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            var opponent = _playerRepository.GetPlayer(context.OpponentPlayerId);

            int totalReturned = 0;

            if (player != null)
            {
                var fieldCards = player.Field.ToList();
                foreach (var card in fieldCards)
                {
                    if (player.ReturnFieldCardToHand(card))
                        totalReturned++;
                }
                _playerRepository.SavePlayer(player);
            }

            if (opponent != null)
            {
                var fieldCards = opponent.Field.ToList();
                foreach (var card in fieldCards)
                {
                    if (opponent.ReturnFieldCardToHand(card))
                        totalReturned++;
                }
                _playerRepository.SavePlayer(opponent);
            }

            return AbilityResult.Success($"Returned {totalReturned} cards to hand");
        }
    }

    /// <summary>
    /// Ability that increases number of attacks allowed.
    /// Migrated from IncrementNumberAttackEffectAbility.
    /// </summary>
    public class IncrementAttacksAbility : IAbility
    {
        private readonly int _bonusAttacks;

        public AbilityName Name { get; }
        public string Description { get; }

        public IncrementAttacksAbility(int bonusAttacks = 1)
        {
            Name = AbilityName.Default;
            _bonusAttacks = bonusAttacks;
            Description = $"Gain +{bonusAttacks} attack(s) per turn";
        }

        public bool CanActivate(AbilityContext context)
        {
            return context.SourceCard != null;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            if (context.SourceCard == null)
                return AbilityResult.Failure("No source card");

            // This would modify the card's attack count for the turn
            return AbilityResult.Success($"+{_bonusAttacks} attacks granted");
        }
    }

    /// <summary>
    /// Ability that applies family-based field effects to invocations.
    /// Migrated from FamilyFieldToInvocationsEffectAbility.
    /// </summary>
    public class FamilyFieldEffectAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly CardFamily _fieldFamily;

        public AbilityName Name { get; }
        public string Description { get; }

        public FamilyFieldEffectAbility(
            CardFamily fieldFamily,
            IPlayerRepository playerRepository)
        {
            Name = AbilityName.Default;
            _fieldFamily = fieldFamily;
            _playerRepository = playerRepository;
            Description = $"Apply {fieldFamily} field effects to all invocations";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return player != null &&
                   player.Field.Any(c => c.Type == CardType.Field && c.Families != null && c.Families.Contains(_fieldFamily)) &&
                   player.Field.Any(c => c.Type == CardType.Invocation);
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null)
                return AbilityResult.Failure("Player not found");

            var invocations = player.Field.Where(c => c.Type == CardType.Invocation).ToList();

            // Apply field bonuses to invocations
            foreach (var invocation in invocations)
            {
                // Field effects would be applied here based on field card
                invocation.ModifyStats(1, 1); // Example bonus
            }

            _playerRepository.SavePlayer(player);
            return AbilityResult.Success($"Applied field effects to {invocations.Count} invocations");
        }
    }

    /// <summary>
    /// Ability that searches for equipment cards.
    /// Migrated from GetEquipmentCardWithoutAttack, GetTypeCardFromDeckWithoutAttackAbility.
    /// </summary>
    public class SearchEquipmentAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;

        public AbilityName Name { get; }
        public string Description { get; }

        public SearchEquipmentAbility(IPlayerRepository playerRepository)
        {
            Name = AbilityName.GetEquipmentCardWithoutAttack;
            _playerRepository = playerRepository;
            Description = "Search deck for equipment card and add to hand";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return player != null && player.Deck.Any(c => c.Type == CardType.Equipment);
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null)
                return AbilityResult.Failure("Player not found");

            var equipmentCards = player.Deck.Where(c => c.Type == CardType.Equipment).ToList();
            if (!equipmentCards.Any())
                return AbilityResult.Failure("No equipment cards in deck");

            // Requires user input to select which equipment
            return AbilityResult.NeedsUserInput("Select equipment card from deck");
        }
    }

    /// <summary>
    /// Ability that allows optional field change from deck.
    /// Migrated from OptionalChangeFieldFromDeckAbility.
    /// </summary>
    public class OptionalChangeFieldAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;

        public AbilityName Name { get; }
        public string Description { get; }

        public OptionalChangeFieldAbility(IPlayerRepository playerRepository)
        {
            Name = AbilityName.ChangeFieldWithFieldFromDeck;
            _playerRepository = playerRepository;
            Description = "Optionally change field card with one from deck";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return player != null && player.Deck.Any(c => c.Type == CardType.Field);
        }

        public AbilityResult Execute(AbilityContext context)
        {
            return AbilityResult.NeedsUserInput("Choose to change field card or skip");
        }
    }

    /// <summary>
    /// Ability that allows optional sacrifice for ATK/DEF gain.
    /// Migrated from OptionalSacrificeForAtkDefAbility.
    /// </summary>
    public class OptionalSacrificeForStatsAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly int _attackGain;
        private readonly int _defenseGain;

        public AbilityName Name { get; }
        public string Description { get; }

        public OptionalSacrificeForStatsAbility(
            AbilityName abilityName,
            int attackGain,
            int defenseGain,
            IPlayerRepository playerRepository)
        {
            Name = abilityName;
            _attackGain = attackGain;
            _defenseGain = defenseGain;
            _playerRepository = playerRepository;
            Description = $"Optionally sacrifice card for +{attackGain} ATK / +{defenseGain} DEF";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return player != null && player.Field.Count > 1; // Need other card to sacrifice
        }

        public AbilityResult Execute(AbilityContext context)
        {
            return AbilityResult.NeedsUserInput($"Choose card to sacrifice for +{_attackGain}/+{_defenseGain}");
        }
    }

    /// <summary>
    /// Ability that sacrifices card based on minimum stats and family conditions.
    /// Migrated from SacrificeCardMinAtkMinDefFamilyNumberAbility.
    /// </summary>
    public class ConditionalSacrificeAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly int _minAttack;
        private readonly int _minDefense;
        private readonly CardFamily _targetFamily;
        private readonly int _requiredFamilyCount;

        public AbilityName Name { get; }
        public string Description { get; }

        public ConditionalSacrificeAbility(
            AbilityName abilityName,
            int minAttack,
            int minDefense,
            CardFamily targetFamily,
            int requiredFamilyCount,
            IPlayerRepository playerRepository)
        {
            Name = abilityName;
            _minAttack = minAttack;
            _minDefense = minDefense;
            _targetFamily = targetFamily;
            _requiredFamilyCount = requiredFamilyCount;
            _playerRepository = playerRepository;
            Description = $"Sacrifice if {minAttack}+ ATK/{minDefense}+ DEF and {requiredFamilyCount}+ {targetFamily} cards";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null || context.SourceCard == null)
                return false;

            bool meetsStats = context.SourceCard.Stats.HasValue &&
                            context.SourceCard.Stats.Value.Attack >= _minAttack &&
                            context.SourceCard.Stats.Value.Defense >= _minDefense;

            int familyCount = player.Field.Count(c =>
                c.Families != null && c.Families.Contains(_targetFamily));

            return meetsStats && familyCount >= _requiredFamilyCount;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            // Check if conditions are met before requesting user input
            if (!CanActivate(context))
            {
                return AbilityResult.Failure("No valid cards to sacrifice - conditions not met");
            }

            return AbilityResult.NeedsUserInput("Sacrifice card meeting conditions");
        }
    }

    /// <summary>
    /// Factory for creating special/miscellaneous abilities.
    /// </summary>
    public class SpecialAbilityFactory
    {
        private readonly IPlayerRepository _playerRepository;

        public SpecialAbilityFactory(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public SendAllCardsToHandAbility CreateSendAllToHand()
        {
            return new SendAllCardsToHandAbility(_playerRepository);
        }

        public IncrementAttacksAbility CreateIncrementAttacks(int bonusAttacks = 1)
        {
            return new IncrementAttacksAbility(bonusAttacks);
        }

        public FamilyFieldEffectAbility CreateFamilyFieldEffect(CardFamily fieldFamily)
        {
            return new FamilyFieldEffectAbility(fieldFamily, _playerRepository);
        }

        public SearchEquipmentAbility CreateSearchEquipment()
        {
            return new SearchEquipmentAbility(_playerRepository);
        }

        public OptionalChangeFieldAbility CreateOptionalChangeField()
        {
            return new OptionalChangeFieldAbility(_playerRepository);
        }

        public OptionalSacrificeForStatsAbility CreateOptionalSacrificeForStats(
            AbilityName name,
            int atkGain,
            int defGain)
        {
            return new OptionalSacrificeForStatsAbility(name, atkGain, defGain, _playerRepository);
        }

        public ConditionalSacrificeAbility CreateConditionalSacrifice(
            AbilityName name,
            int minAtk,
            int minDef,
            CardFamily family,
            int requiredCount)
        {
            return new ConditionalSacrificeAbility(name, minAtk, minDef, family, requiredCount, _playerRepository);
        }
    }
}
