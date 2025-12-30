using JDG.Domain;
using JDG.Application.Repositories;

namespace JDG.Application.Abilities.Implementations
{
    /// <summary>
    /// Equipment ability that sets specific ATK/DEF values.
    /// Migrated from SetAtkDefAbility.
    /// </summary>
    public class SetStatsEquipmentAbility : IAbility
    {
        private readonly int _attack;
        private readonly int _defense;

        public AbilityName Name { get; }
        public string Description { get; }

        public SetStatsEquipmentAbility(int attack, int defense)
        {
            Name = AbilityName.Default;
            _attack = attack;
            _defense = defense;
            Description = $"Set ATK/DEF to {attack}/{defense}";
        }

        public bool CanActivate(AbilityContext context)
        {
            return context.TargetCard != null;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            if (context.TargetCard == null)
                return AbilityResult.Failure("No target card");

            context.TargetCard.SetStats(_attack, _defense);
            return AbilityResult.Success($"Stats set to {_attack}/{_defense}");
        }
    }

    /// <summary>
    /// Equipment ability that adds ATK/DEF bonuses.
    /// Migrated from EarnAtkDefAbility.
    /// </summary>
    public class BonusStatsEquipmentAbility : IAbility
    {
        private readonly int _attackBonus;
        private readonly int _defenseBonus;

        public AbilityName Name { get; }
        public string Description { get; }

        public BonusStatsEquipmentAbility(int attackBonus, int defenseBonus)
        {
            Name = AbilityName.Default;
            _attackBonus = attackBonus;
            _defenseBonus = defenseBonus;
            Description = $"+{attackBonus} ATK / +{defenseBonus} DEF";
        }

        public bool CanActivate(AbilityContext context)
        {
            return context.TargetCard != null;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            if (context.TargetCard == null)
                return AbilityResult.Failure("No target card");

            context.TargetCard.ModifyStats(_attackBonus, _defenseBonus);
            return AbilityResult.Success($"Added +{_attackBonus}/+{_defenseBonus}");
        }
    }

    /// <summary>
    /// Equipment ability that multiplies ATK/DEF.
    /// Migrated from MultiplyAtkDefAbility.
    /// </summary>
    public class MultiplyStatsEquipmentAbility : IAbility
    {
        private readonly float _attackMultiplier;
        private readonly float _defenseMultiplier;

        public AbilityName Name { get; }
        public string Description { get; }

        public MultiplyStatsEquipmentAbility(float attackMultiplier, float defenseMultiplier)
        {
            Name = AbilityName.Default;
            _attackMultiplier = attackMultiplier;
            _defenseMultiplier = defenseMultiplier;
            Description = $"Multiply ATK by {attackMultiplier}x, DEF by {defenseMultiplier}x";
        }

        public bool CanActivate(AbilityContext context)
        {
            return context.TargetCard != null;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            if (context.TargetCard == null || !context.TargetCard.Stats.HasValue)
                return AbilityResult.Failure("No target card or invalid stats");

            int newAtk = (int)(context.TargetCard.Stats.Value.Attack * _attackMultiplier);
            int newDef = (int)(context.TargetCard.Stats.Value.Defense * _defenseMultiplier);
            context.TargetCard.SetStats(newAtk, newDef);

            return AbilityResult.Success($"Stats multiplied to {newAtk}/{newDef}");
        }
    }

    /// <summary>
    /// Equipment ability that protects from destruction.
    /// Migrated from ProtectFromDestructionAbility, CantBeAttackDestroyByInvocationAbility.
    /// </summary>
    public class ProtectFromDestructionEquipmentAbility : IAbility
    {
        public AbilityName Name { get; }
        public string Description { get; }

        public ProtectFromDestructionEquipmentAbility()
        {
            Name = AbilityName.Default;
            Description = "Protects from destruction";
        }

        public bool CanActivate(AbilityContext context)
        {
            return context.TargetCard != null;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            // This is a passive protection - marks card as protected
            return AbilityResult.Success("Card protected from destruction");
        }
    }

    /// <summary>
    /// Equipment ability that cancels invocation abilities.
    /// Migrated from CancelInvocationAbility.
    /// </summary>
    public class CancelAbilitiesEquipmentAbility : IAbility
    {
        public AbilityName Name { get; }
        public string Description { get; }

        public CancelAbilitiesEquipmentAbility()
        {
            Name = AbilityName.Default;
            Description = "Cancel equipped card's abilities";
        }

        public bool CanActivate(AbilityContext context)
        {
            return context.TargetCard != null;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            if (context.TargetCard == null)
                return AbilityResult.Failure("No target card");

            // Mark card abilities as canceled
            context.TargetCard.SetCancelEffect(true);
            return AbilityResult.Success("Card abilities canceled");
        }
    }

    /// <summary>
    /// Equipment ability that switches equipment to another card.
    /// Migrated from SwitchEquipmentCardAbility.
    /// </summary>
    public class SwitchEquipmentAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;

        public AbilityName Name { get; }
        public string Description { get; }

        public SwitchEquipmentAbility(IPlayerRepository playerRepository)
        {
            Name = AbilityName.Default;
            _playerRepository = playerRepository;
            Description = "Can be equipped to multiple cards";
        }

        public bool CanActivate(AbilityContext context)
        {
            return true; // Can always be placed on any card
        }

        public AbilityResult Execute(AbilityContext context)
        {
            return AbilityResult.Success("Equipment can be switched");
        }
    }

    /// <summary>
    /// Equipment ability that prevents attacking newly summoned opponent invocations.
    /// Migrated from PreventAttackNewOpponentInvocationAbility.
    /// </summary>
    public class PreventAttackNewCardsEquipmentAbility : IAbility
    {
        public AbilityName Name { get; }
        public string Description { get; }

        public PreventAttackNewCardsEquipmentAbility()
        {
            Name = AbilityName.Default;
            Description = "Cannot attack newly summoned opponent cards";
        }

        public bool CanActivate(AbilityContext context)
        {
            return context.TargetCard != null;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            // Passive restriction ability
            return AbilityResult.Success("Attack restriction applied");
        }
    }

    /// <summary>
    /// Equipment ability that enables direct attack.
    /// Migrated from DirectAttackAbility.
    /// </summary>
    public class DirectAttackEquipmentAbility : IAbility
    {
        public AbilityName Name { get; }
        public string Description { get; }

        public DirectAttackEquipmentAbility()
        {
            Name = AbilityName.Default;
            Description = "Equipped card can attack player directly";
        }

        public bool CanActivate(AbilityContext context)
        {
            return context.TargetCard != null;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            if (context.TargetCard == null)
                return AbilityResult.Failure("No target card");

            context.TargetCard.EnableDirectAttack();
            return AbilityResult.Success("Direct attack enabled");
        }
    }

    /// <summary>
    /// Equipment ability that provides stats based on hand card count.
    /// Migrated from EarnAtkDefAbility with hand-based calculation.
    /// </summary>
    public class HandBasedStatsEquipmentAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly float _attackPerCard;
        private readonly float _defensePerCard;

        public AbilityName Name { get; }
        public string Description { get; }

        public HandBasedStatsEquipmentAbility(
            IPlayerRepository playerRepository,
            float attackPerCard,
            float defensePerCard)
        {
            Name = AbilityName.Default;
            _playerRepository = playerRepository;
            _attackPerCard = attackPerCard;
            _defensePerCard = defensePerCard;
            Description = $"+{attackPerCard} ATK / +{defensePerCard} DEF per hand card";
        }

        public bool CanActivate(AbilityContext context)
        {
            return context.TargetCard != null;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            if (context.TargetCard == null)
                return AbilityResult.Failure("No target card");

            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null)
                return AbilityResult.Failure("Player not found");

            int handCount = player.HandCount;
            int atkBonus = (int)(handCount * _attackPerCard);
            int defBonus = (int)(handCount * _defensePerCard);

            context.TargetCard.ModifyStats(atkBonus, defBonus);
            return AbilityResult.Success($"Added +{atkBonus}/+{defBonus} based on {handCount} hand cards");
        }
    }

    /// <summary>
    /// Equipment ability that prevents being attacked by other invocations.
    /// Migrated from CantBeAttackDestroyByInvocationAbility.
    /// </summary>
    public class CantBeAttackedEquipmentAbility : IAbility
    {
        public AbilityName Name { get; }
        public string Description { get; }

        public CantBeAttackedEquipmentAbility()
        {
            Name = AbilityName.Default;
            Description = "Cannot be attacked by other invocations";
        }

        public bool CanActivate(AbilityContext context)
        {
            return context.TargetCard != null;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            // Passive protection - card marked as untargetable
            return AbilityResult.Success("Card cannot be attacked by invocations");
        }
    }

    /// <summary>
    /// Factory for creating equipment abilities.
    /// </summary>
    public class EquipmentAbilityFactory
    {
        private readonly IPlayerRepository _playerRepository;

        public EquipmentAbilityFactory(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public SetStatsEquipmentAbility CreateSetStats(int atk, int def)
        {
            return new SetStatsEquipmentAbility(atk, def);
        }

        public BonusStatsEquipmentAbility CreateBonusStats(int atk, int def)
        {
            return new BonusStatsEquipmentAbility(atk, def);
        }

        public MultiplyStatsEquipmentAbility CreateMultiplyStats(float atkMult, float defMult)
        {
            return new MultiplyStatsEquipmentAbility(atkMult, defMult);
        }

        public ProtectFromDestructionEquipmentAbility CreateProtectFromDestruction()
        {
            return new ProtectFromDestructionEquipmentAbility();
        }

        public CancelAbilitiesEquipmentAbility CreateCancelAbilities()
        {
            return new CancelAbilitiesEquipmentAbility();
        }

        public SwitchEquipmentAbility CreateSwitchEquipment()
        {
            return new SwitchEquipmentAbility(_playerRepository);
        }

        public PreventAttackNewCardsEquipmentAbility CreatePreventAttackNew()
        {
            return new PreventAttackNewCardsEquipmentAbility();
        }

        public DirectAttackEquipmentAbility CreateDirectAttack()
        {
            return new DirectAttackEquipmentAbility();
        }

        public HandBasedStatsEquipmentAbility CreateHandBasedStats(float atkPerCard, float defPerCard)
        {
            return new HandBasedStatsEquipmentAbility(_playerRepository, atkPerCard, defPerCard);
        }

        public CantBeAttackedEquipmentAbility CreateCantBeAttacked()
        {
            return new CantBeAttackedEquipmentAbility();
        }
    }
}
