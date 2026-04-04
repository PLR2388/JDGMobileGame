using JDG.Domain;
using JDG.Domain.Enums;
using JDG.Application.Repositories;
using System.Linq;

namespace JDG.Application.Abilities.Implementations
{
    /// <summary>
    /// Ability that prevents the card from being attacked.
    /// Migrated from CantBeAttackAbility, CantBeAttackKill, CantBeAttackIfComics.
    /// </summary>
    public class CantBeAttackedAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly CardFamily? _conditionFamily; // null = always protected

        public AbilityName Name { get; }
        public string Description { get; }

        public CantBeAttackedAbility(
            AbilityName abilityName,
            IPlayerRepository playerRepository,
            CardFamily? conditionFamily = null)
        {
            Name = abilityName;
            _playerRepository = playerRepository;
            _conditionFamily = conditionFamily;
            Description = conditionFamily.HasValue
                ? $"Cannot be attacked if {conditionFamily.Value} is on field"
                : "Cannot be attacked";
        }

        public bool CanActivate(AbilityContext context)
        {
            if (!_conditionFamily.HasValue)
                return true; // Always active

            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return player != null && player.Field.Any(c =>
                c.Families != null && c.Families.Contains(_conditionFamily.Value));
        }

        public AbilityResult Execute(AbilityContext context)
        {
            // This is a passive ability - execution marks the card as protected
            return AbilityResult.Success("Card protected from attacks");
        }
    }

    /// <summary>
    /// Ability that protects cards behind this card.
    /// Migrated from ProtectBehindDuringAttackAbility, ProtectedBehindStarlightUnicorn, ProtectBehindGreaterDef.
    /// </summary>
    public class ProtectBehindAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly int? _minDefense; // null = protect all, value = protect if def greater

        public AbilityName Name { get; }
        public string Description { get; }

        public ProtectBehindAbility(
            AbilityName abilityName,
            IPlayerRepository playerRepository,
            int? minDefense = null)
        {
            Name = abilityName;
            _playerRepository = playerRepository;
            _minDefense = minDefense;
            Description = minDefense.HasValue
                ? $"Protect cards with less than {minDefense} DEF behind this card"
                : "Protect all cards behind this card";
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

            var protectedCards = player.Field.Where(c =>
                c != context.SourceCard &&
                (!_minDefense.HasValue || (c.Stats.HasValue && c.Stats.Value.Defense < _minDefense.Value)))
                .ToList();

            return AbilityResult.Success($"Protecting {protectedCards.Count} cards");
        }
    }

    /// <summary>
    /// Ability that restricts card to only attack itself.
    /// Migrated from CanOnlyAttackItselfAbility.
    /// </summary>
    public class CanOnlyAttackItselfAbility : IAbility
    {
        public AbilityName Name { get; }
        public string Description { get; }

        public CanOnlyAttackItselfAbility()
        {
            Name = AbilityName.CanOnlyAttackItself;
            Description = "Can only attack itself";
        }

        public bool CanActivate(AbilityContext context)
        {
            return true;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            // This is a passive restriction ability
            return AbilityResult.Success("Attack restricted to self");
        }
    }

    /// <summary>
    /// Ability that makes card survive one turn before being destroyed.
    /// Migrated from SurviveOneTurn, LimitTurnExistenceAbility.
    /// </summary>
    public class LimitedLifetimeAbility : IAbility
    {
        private readonly int _maxTurns;

        public AbilityName Name { get; }
        public string Description { get; }

        public LimitedLifetimeAbility(AbilityName abilityName, int maxTurns = 1)
        {
            Name = abilityName;
            _maxTurns = maxTurns;
            Description = $"Destroyed after {maxTurns} turn(s)";
        }

        public bool CanActivate(AbilityContext context)
        {
            return context.SourceCard != null;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            // This would track turns on the card and destroy when limit reached
            return AbilityResult.Success($"Card will be destroyed in {_maxTurns} turn(s)");
        }
    }

    /// <summary>
    /// Ability where card cannot exist without specific other cards.
    /// Migrated from CantLiveWithoutAbility (Benzaie, JDG, Comics, Human, Japon, Granolax variants).
    /// </summary>
    public class DependencyAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly string[] _requiredCardNames;

        public AbilityName Name { get; }
        public string Description { get; }

        public DependencyAbility(
            AbilityName abilityName,
            IPlayerRepository playerRepository,
            params string[] requiredCardNames)
        {
            Name = abilityName;
            _playerRepository = playerRepository;
            _requiredCardNames = requiredCardNames;
            Description = $"Cannot exist without: {string.Join(" or ", requiredCardNames)}";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null) return false;

            // Check if at least one required card is on field
            return _requiredCardNames.Any(name => player.Field.Any(c => c.Title == name));
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null || context.SourceCard == null)
                return AbilityResult.Failure("Invalid context");

            // Check if dependency is satisfied
            bool hasDependency = _requiredCardNames.Any(name =>
                player.Field.Any(c => c.Title == name));

            if (!hasDependency)
            {
                // Card must be destroyed - dependency check FAILED
                player.DestroyCardFromField(context.SourceCard);
                _playerRepository.SavePlayer(player);
                return AbilityResult.Failure("Card destroyed due to missing dependency");
            }

            return AbilityResult.Success("Dependency satisfied");
        }
    }

    /// <summary>
    /// Ability where card cannot exist without a specific card family on field.
    /// </summary>
    public class FamilyDependencyAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly CardFamily _requiredFamily;

        public AbilityName Name { get; }
        public string Description { get; }

        public FamilyDependencyAbility(
            CardFamily requiredFamily,
            IPlayerRepository playerRepository)
        {
            Name = AbilityName.Default;
            _playerRepository = playerRepository;
            _requiredFamily = requiredFamily;
            Description = $"Cannot exist without {requiredFamily} card on field";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null) return false;

            // Check if at least one card of required family is on field (excluding self)
            return player.Field.Any(c =>
                c != context.SourceCard &&
                c.Families != null &&
                c.Families.Contains(_requiredFamily));
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null || context.SourceCard == null)
                return AbilityResult.Failure("Invalid context");

            // Check if dependency is satisfied (excluding self)
            bool hasDependency = player.Field.Any(c =>
                c != context.SourceCard &&
                c.Families != null &&
                c.Families.Contains(_requiredFamily));

            if (!hasDependency)
            {
                // Card must be destroyed
                player.DestroyCardFromField(context.SourceCard);
                _playerRepository.SavePlayer(player);
                return AbilityResult.Failure("Card destroyed due to missing family dependency");
            }

            return AbilityResult.Success($"Dependency satisfied: {_requiredFamily} present on field");
        }
    }

    /// <summary>
    /// Factory for creating protection and dependency abilities.
    /// </summary>
    public class ProtectionAbilityFactory
    {
        private readonly IPlayerRepository _playerRepository;

        public ProtectionAbilityFactory(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public CantBeAttackedAbility CreateCantBeAttacked(AbilityName name, CardFamily? conditionFamily = null)
        {
            return new CantBeAttackedAbility(name, _playerRepository, conditionFamily);
        }

        public ProtectBehindAbility CreateProtectBehind(AbilityName name, int? minDefense = null)
        {
            return new ProtectBehindAbility(name, _playerRepository, minDefense);
        }

        public CanOnlyAttackItselfAbility CreateCanOnlyAttackItself()
        {
            return new CanOnlyAttackItselfAbility();
        }

        public LimitedLifetimeAbility CreateLimitedLifetime(AbilityName name, int maxTurns)
        {
            return new LimitedLifetimeAbility(name, maxTurns);
        }

        public DependencyAbility CreateDependency(AbilityName name, params string[] requiredCards)
        {
            return new DependencyAbility(name, _playerRepository, requiredCards);
        }

        /// <summary>
        /// Creates a dependency ability that requires any card from a specific family to be on field.
        /// </summary>
        public DependencyAbility CreateDependencyAbility(string[] requiredCardNames)
        {
            return new DependencyAbility(AbilityName.Default, _playerRepository, requiredCardNames);
        }

        /// <summary>
        /// Creates a family dependency ability where card needs at least one card from a specific family on field.
        /// </summary>
        public FamilyDependencyAbility CreateFamilyDependencyAbility(CardFamily requiredFamily)
        {
            return new FamilyDependencyAbility(requiredFamily, _playerRepository);
        }
    }
}
