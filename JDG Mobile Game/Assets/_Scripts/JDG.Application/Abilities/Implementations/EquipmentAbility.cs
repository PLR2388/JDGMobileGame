using JDG.Domain;
using JDG.Application.Repositories;

namespace JDG.Application.Abilities.Implementations
{
    /// <summary>
    /// Base class for equipment abilities with common IEquipmentAbility implementation.
    /// Phase 117: Added to provide default implementation for equipment abilities.
    /// </summary>
    public abstract class BaseEquipmentAbility : IEquipmentAbility, IPassiveAbility
    {
        public abstract AbilityName Name { get; }
        public abstract string Description { get; }
        public virtual bool CanAlwaysBePlaced => false;
        public virtual AbilityTrigger Trigger => AbilityTrigger.OnEquip;

        public abstract bool CanActivate(AbilityContext context);
        public abstract AbilityResult Execute(AbilityContext context);

        /// <summary>
        /// Default implementation allows destruction.
        /// Override to prevent destruction (e.g., protection abilities).
        /// </summary>
        public virtual bool OnPreDestroy(AbilityContext context) => true;
    }

    /// <summary>
    /// Equipment ability that sets specific ATK/DEF values.
    /// Migrated from SetAtkDefAbility.
    /// Phase 117: Updated to extend BaseEquipmentAbility.
    /// </summary>
    public class SetStatsEquipmentAbility : BaseEquipmentAbility
    {
        private readonly int _attack;
        private readonly int _defense;

        public override AbilityName Name { get; }
        public override string Description { get; }

        public SetStatsEquipmentAbility(int attack, int defense)
        {
            Name = AbilityName.Default;
            _attack = attack;
            _defense = defense;
            Description = $"Set ATK/DEF to {attack}/{defense}";
        }

        public override bool CanActivate(AbilityContext context)
        {
            return context.TargetCard != null;
        }

        public override AbilityResult Execute(AbilityContext context)
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
    /// Phase 117: Updated to extend BaseEquipmentAbility.
    /// </summary>
    public class BonusStatsEquipmentAbility : BaseEquipmentAbility
    {
        private readonly int _attackBonus;
        private readonly int _defenseBonus;

        public override AbilityName Name { get; }
        public override string Description { get; }

        public BonusStatsEquipmentAbility(int attackBonus, int defenseBonus)
        {
            Name = AbilityName.Default;
            _attackBonus = attackBonus;
            _defenseBonus = defenseBonus;
            Description = $"+{attackBonus} ATK / +{defenseBonus} DEF";
        }

        public override bool CanActivate(AbilityContext context)
        {
            return context.TargetCard != null;
        }

        public override AbilityResult Execute(AbilityContext context)
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
    /// Phase 117: Updated to extend BaseEquipmentAbility.
    /// </summary>
    public class MultiplyStatsEquipmentAbility : BaseEquipmentAbility
    {
        private readonly float _attackMultiplier;
        private readonly float _defenseMultiplier;

        public override AbilityName Name { get; }
        public override string Description { get; }

        public MultiplyStatsEquipmentAbility(float attackMultiplier, float defenseMultiplier)
        {
            Name = AbilityName.Default;
            _attackMultiplier = attackMultiplier;
            _defenseMultiplier = defenseMultiplier;
            Description = $"Multiply ATK by {attackMultiplier}x, DEF by {defenseMultiplier}x";
        }

        public override bool CanActivate(AbilityContext context)
        {
            return context.TargetCard != null;
        }

        public override AbilityResult Execute(AbilityContext context)
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
    /// Phase 117: Updated to extend BaseEquipmentAbility with OnPreDestroy override.
    /// </summary>
    public class ProtectFromDestructionEquipmentAbility : BaseEquipmentAbility
    {
        public override AbilityName Name { get; }
        public override string Description { get; }

        public ProtectFromDestructionEquipmentAbility()
        {
            Name = AbilityName.Default;
            Description = "Protects from destruction";
        }

        public override bool CanActivate(AbilityContext context)
        {
            return context.TargetCard != null;
        }

        public override AbilityResult Execute(AbilityContext context)
        {
            // This is a passive protection - marks card as protected
            return AbilityResult.Success("Card protected from destruction");
        }

        /// <summary>
        /// Prevents destruction by returning false.
        /// Equipment will be destroyed instead.
        /// </summary>
        public override bool OnPreDestroy(AbilityContext context) => false;
    }

    /// <summary>
    /// Equipment ability that cancels invocation abilities.
    /// Migrated from CancelInvocationAbility.
    /// Phase 117: Updated to extend BaseEquipmentAbility.
    /// </summary>
    public class CancelAbilitiesEquipmentAbility : BaseEquipmentAbility
    {
        public override AbilityName Name { get; }
        public override string Description { get; }

        public CancelAbilitiesEquipmentAbility()
        {
            Name = AbilityName.Default;
            Description = "Cancel equipped card's abilities";
        }

        public override bool CanActivate(AbilityContext context)
        {
            return context.TargetCard != null;
        }

        public override AbilityResult Execute(AbilityContext context)
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
    /// Phase 117: Updated to extend BaseEquipmentAbility with CanAlwaysBePlaced = true.
    /// </summary>
    public class SwitchEquipmentAbility : BaseEquipmentAbility
    {
        private readonly IPlayerRepository _playerRepository;

        public override AbilityName Name { get; }
        public override string Description { get; }
        public override bool CanAlwaysBePlaced => true;

        public SwitchEquipmentAbility(IPlayerRepository playerRepository)
        {
            Name = AbilityName.Default;
            _playerRepository = playerRepository;
            Description = "Can be equipped to multiple cards";
        }

        public override bool CanActivate(AbilityContext context)
        {
            return true; // Can always be placed on any card
        }

        public override AbilityResult Execute(AbilityContext context)
        {
            return AbilityResult.Success("Equipment can be switched");
        }
    }

    /// <summary>
    /// Equipment ability that prevents attacking newly summoned opponent invocations.
    /// Migrated from PreventAttackNewOpponentInvocationAbility.
    /// Phase 117: Updated to extend BaseEquipmentAbility with OnCardPlayed trigger.
    /// </summary>
    public class PreventAttackNewCardsEquipmentAbility : BaseEquipmentAbility
    {
        public override AbilityName Name { get; }
        public override string Description { get; }
        public override AbilityTrigger Trigger => AbilityTrigger.OnCardPlayed;

        public PreventAttackNewCardsEquipmentAbility()
        {
            Name = AbilityName.Default;
            Description = "Cannot attack newly summoned opponent cards";
        }

        public override bool CanActivate(AbilityContext context)
        {
            return context.TargetCard != null;
        }

        public override AbilityResult Execute(AbilityContext context)
        {
            // Passive restriction - block attack on newly summoned cards
            if (context.TargetCard != null)
            {
                context.TargetCard.BlockAttack();
            }
            return AbilityResult.Success("Attack restriction applied");
        }
    }

    /// <summary>
    /// Equipment ability that enables direct attack.
    /// Migrated from DirectAttackAbility.
    /// Phase 117: Updated to extend BaseEquipmentAbility.
    /// </summary>
    public class DirectAttackEquipmentAbility : BaseEquipmentAbility
    {
        public override AbilityName Name { get; }
        public override string Description { get; }

        public DirectAttackEquipmentAbility()
        {
            Name = AbilityName.Default;
            Description = "Equipped card can attack player directly";
        }

        public override bool CanActivate(AbilityContext context)
        {
            return context.TargetCard != null;
        }

        public override AbilityResult Execute(AbilityContext context)
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
    /// Phase 117: Updated to extend BaseEquipmentAbility with OnHandChange trigger.
    /// </summary>
    public class HandBasedStatsEquipmentAbility : BaseEquipmentAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly float _attackPerCard;
        private readonly float _defensePerCard;

        public override AbilityName Name { get; }
        public override string Description { get; }
        public override AbilityTrigger Trigger => AbilityTrigger.OnHandChange;

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

        public override bool CanActivate(AbilityContext context)
        {
            return context.TargetCard != null;
        }

        public override AbilityResult Execute(AbilityContext context)
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
    /// Phase 117: Updated to extend BaseEquipmentAbility with OnPreDestroy override.
    /// </summary>
    public class CantBeAttackedEquipmentAbility : BaseEquipmentAbility
    {
        public override AbilityName Name { get; }
        public override string Description { get; }

        public CantBeAttackedEquipmentAbility()
        {
            Name = AbilityName.Default;
            Description = "Cannot be attacked by other invocations";
        }

        public override bool CanActivate(AbilityContext context)
        {
            return context.TargetCard != null;
        }

        public override AbilityResult Execute(AbilityContext context)
        {
            // Mark card as untargetable by invocations
            if (context.TargetCard != null)
            {
                context.TargetCard.SetCantBeAttacked(true);
            }
            return AbilityResult.Success("Card cannot be attacked by invocations");
        }

        /// <summary>
        /// Prevents destruction by invocations.
        /// </summary>
        public override bool OnPreDestroy(AbilityContext context) => false;
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
