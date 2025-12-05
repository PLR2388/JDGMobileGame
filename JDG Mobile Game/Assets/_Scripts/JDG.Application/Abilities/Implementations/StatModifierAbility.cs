using JDG.Domain;
using JDG.Domain.Enums;
using JDG.Application.Repositories;
using System.Linq;

namespace JDG.Application.Abilities.Implementations
{
    /// <summary>
    /// Ability that gives ATK/DEF bonuses to cards of a specific family.
    /// Migrated from GiveAtkDefFamilyAbility, GiveAtkDefToComics, etc.
    /// </summary>
    public class GiveFamilyStatsAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly CardFamily _targetFamily;
        private readonly int _attackBonus;
        private readonly int _defenseBonus;

        public AbilityName Name { get; }
        public string Description { get; }

        public GiveFamilyStatsAbility(
            AbilityName abilityName,
            CardFamily targetFamily,
            int attackBonus,
            int defenseBonus,
            IPlayerRepository playerRepository)
        {
            Name = abilityName;
            _targetFamily = targetFamily;
            _attackBonus = attackBonus;
            _defenseBonus = defenseBonus;
            _playerRepository = playerRepository;
            Description = $"Give {targetFamily} cards +{attackBonus} ATK / +{defenseBonus} DEF";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return player != null && player.Field.Any(c =>
                c.Families != null && c.Families.Contains(_targetFamily));
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null)
                return AbilityResult.Failure("Player not found");

            var familyCards = player.Field.Where(c =>
                c.Families != null && c.Families.Contains(_targetFamily)).ToList();

            if (!familyCards.Any())
                return AbilityResult.Failure($"No {_targetFamily} cards on field");

            foreach (var card in familyCards)
            {
                card.ModifyStats(_attackBonus, _defenseBonus);
            }

            _playerRepository.SavePlayer(player);
            return AbilityResult.Success($"Gave +{_attackBonus}/+{_defenseBonus} to {familyCards.Count} {_targetFamily} cards");
        }
    }

    /// <summary>
    /// Ability that gives ATK/DEF bonuses based on conditions.
    /// Migrated from WinAtkDefFamilyAbility, Win1ATK1DefJaponWith2ATK2DEFCondition, etc.
    /// </summary>
    public class ConditionalStatsAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly CardFamily _conditionFamily;
        private readonly int _minAttack;
        private readonly int _minDefense;
        private readonly int _attackBonus;
        private readonly int _defenseBonus;

        public AbilityName Name { get; }
        public string Description { get; }

        public ConditionalStatsAbility(
            AbilityName abilityName,
            CardFamily conditionFamily,
            int minAttack,
            int minDefense,
            int attackBonus,
            int defenseBonus,
            IPlayerRepository playerRepository)
        {
            Name = abilityName;
            _conditionFamily = conditionFamily;
            _minAttack = minAttack;
            _minDefense = minDefense;
            _attackBonus = attackBonus;
            _defenseBonus = defenseBonus;
            _playerRepository = playerRepository;
            Description = $"If {conditionFamily} card has {minAttack}+ ATK / {minDefense}+ DEF, gain +{attackBonus}/+{defenseBonus}";
        }

        public bool CanActivate(AbilityContext context)
        {
            return context.SourceCard != null;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null || context.SourceCard == null)
                return AbilityResult.Failure("Invalid context");

            var sourceCard = context.SourceCard;

            // Check if card meets condition
            if (sourceCard.Families != null &&
                sourceCard.Families.Contains(_conditionFamily) &&
                sourceCard.Stats.HasValue &&
                sourceCard.Stats.Value.Attack >= _minAttack &&
                sourceCard.Stats.Value.Defense >= _minDefense)
            {
                sourceCard.ModifyStats(_attackBonus, _defenseBonus);
                _playerRepository.SavePlayer(player);
                return AbilityResult.Success($"Gained +{_attackBonus}/+{_defenseBonus}");
            }

            return AbilityResult.Failure("Condition not met");
        }
    }

    /// <summary>
    /// Ability that copies ATK/DEF from another card.
    /// Migrated from CopyAtkDefAbility, CopyBenzaieJeune.
    /// </summary>
    public class CopyStatsAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly string _targetCardName;

        public AbilityName Name { get; }
        public string Description { get; }

        public CopyStatsAbility(
            AbilityName abilityName,
            string targetCardName,
            IPlayerRepository playerRepository)
        {
            Name = abilityName;
            _targetCardName = targetCardName;
            _playerRepository = playerRepository;
            Description = $"Copy ATK/DEF from {targetCardName}";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return player != null &&
                   context.SourceCard != null &&
                   player.Field.Any(c => c.Title == _targetCardName);
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null || context.SourceCard == null)
                return AbilityResult.Failure("Invalid context");

            var targetCard = player.Field.FirstOrDefault(c => c.Title == _targetCardName);
            if (targetCard == null || !targetCard.Stats.HasValue)
                return AbilityResult.Failure($"{_targetCardName} not on field or has no stats");

            context.SourceCard.SetStats(targetCard.Stats.Value.Attack, targetCard.Stats.Value.Defense);
            _playerRepository.SavePlayer(player);

            return AbilityResult.Success($"Copied {targetCard.Stats.Value.Attack}/{targetCard.Stats.Value.Defense}");
        }
    }

    /// <summary>
    /// Factory for creating stat modifier abilities.
    /// </summary>
    public class StatModifierAbilityFactory
    {
        private readonly IPlayerRepository _playerRepository;

        public StatModifierAbilityFactory(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public GiveFamilyStatsAbility CreateGiveFamilyStats(
            AbilityName name,
            CardFamily family,
            int atk,
            int def)
        {
            return new GiveFamilyStatsAbility(name, family, atk, def, _playerRepository);
        }

        public ConditionalStatsAbility CreateConditionalStats(
            AbilityName name,
            CardFamily family,
            int minAtk,
            int minDef,
            int bonusAtk,
            int bonusDef)
        {
            return new ConditionalStatsAbility(name, family, minAtk, minDef, bonusAtk, bonusDef, _playerRepository);
        }

        public CopyStatsAbility CreateCopyStats(AbilityName name, string targetCardName)
        {
            return new CopyStatsAbility(name, targetCardName, _playerRepository);
        }
    }
}
