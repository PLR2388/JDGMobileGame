using System.Collections.Generic;
using System.Linq;
using JDG.Application.Abilities;
using JDG.Application.Abilities.Implementations;
using JDG.Application.Repositories;
using DomainEquipmentAbilityName = JDG.Domain.Enums.EquipmentAbilityName;

/// <summary>
/// Provides equipment abilities.
/// Phase 48: Originally wrapped legacy singleton for DI-compatible access.
/// Phase 84: Now owns the ability dictionary directly (EquipmentAbilityLibrary deleted).
/// Phase 103: Added modern IAbility support via GetModernAbility.
/// </summary>
#pragma warning disable CS0618 // Suppress obsolete warnings for legacy ability types
public class EquipmentAbilityProviderService : IEquipmentAbilityProvider
{
    private readonly Dictionary<EquipmentAbilityName, EquipmentAbility> _abilityDictionary;
    private Dictionary<DomainEquipmentAbilityName, IAbility> _modernAbilityDictionary;
    private readonly EquipmentAbilityFactory _factory;

    /// <summary>
    /// Initializes the equipment ability provider with all abilities.
    /// Phase 84: Moved from EquipmentAbilityLibrary.
    /// Phase 103: Added modern ability factory initialization.
    /// </summary>
    public EquipmentAbilityProviderService(IPlayerRepository playerRepository = null)
    {
        // Initialize modern factory if repository provided
        if (playerRepository != null)
        {
            _factory = new EquipmentAbilityFactory(playerRepository);
            InitializeModernAbilities();
        }
        else
        {
            _modernAbilityDictionary = new Dictionary<DomainEquipmentAbilityName, IAbility>();
        }

        var abilities = new List<EquipmentAbility>
        {
            new MultiplyAtkDefAbility(
                EquipmentAbilityName.MultiplyDefBy2ButPreventAttack,
                "Multiply DEF by 2 but the invocation cannot attack",
                defenseFactor: 2f,
                shouldPreventAttack: true
            ),
            new EarnAtkDefAbility(
                EquipmentAbilityName.Earn1ATKAndMinus1DEF,
                "The invocation earns 1 ATK and -1 DEF",
                1,
                -1
            ),
            new DirectAttackAbility(
                EquipmentAbilityName.DirectAttack,
                "Invocation card can directly attack player"
            ),
            new EarnAtkDefAbility(
                EquipmentAbilityName.EarnOneQuarterATKPerHandCards,
                "Invocation earns 0.25 ATK per hands in his hand",
                0.25f,
                0f,
                true
            ),
            new PreventAttackNewOpponentInvocationAbility(
                EquipmentAbilityName.PreventNewOpponentToAttack,
                "Prevent a freshly opponent invoke invocation to attack"
            ),
            new EarnAtkDefAbility(
                EquipmentAbilityName.Remove1ATKAnd1DEF,
                "Invocation looses 1 ATK and 1 DEF",
                -1,
                -1
            ),
            new SetAtkDefAbility(
                EquipmentAbilityName.SetATKToOne,
                "Invocation now has an ATK of 1",
                1f
            ),
            new CantBeAttackDestroyByInvocationAbility(
                EquipmentAbilityName.CantBeAttackByOtherInvocations,
                "Invocation can't be attacked or destroyed by another invocation"
            ),
            new MultiplyAtkDefAbility(
                EquipmentAbilityName.MultiplyAtkBy3,
                "Multiply ATK by 3",
                3
            ),
            new SetAtkDefAbility(
                EquipmentAbilityName.SetDefToZero,
                "Invocation now has a DEF of 0",
                def: 0
            ),
            new EarnAtkDefAbility(
                EquipmentAbilityName.Earn2ATK,
                "Invocation earns 2 ATK",
                2,
                0
            ),
            new EarnAtkDefAbility(
                EquipmentAbilityName.Earn3ATKAndMinus1DEF,
                "Invocation ears 3 ATK and -1 DEF",
                3,
                -1
            ),
            new EarnAtkDefAbility(
                EquipmentAbilityName.Earn1ATKAnd1DEF,
                "Invocation earns 1 ATK and 1 DEF",
                1,
                1
            ),
            new MultiplyAtkDefAbility(
                EquipmentAbilityName.MultiplyAtkBy2AndDefByHalf,
                "Multiply ATK by 2 and DEF by 1/2",
                2,
                0.5f
            ),
            new EarnAtkDefAbility(
                EquipmentAbilityName.EarnOneQuarterDEFPerHandCards,
                "Invocation earns 0.25 DEF per hands in his hand",
                0,
                0.25f,
                true
            ),
            new SwitchEquipmentCardAbility(
                EquipmentAbilityName.SwitchEquipmentCard,
                "Player can replace an equipment card by this one and add the previous to the yellow trash"
            ),
            new EarnAtkDefAbility(
                EquipmentAbilityName.Loose2ATK,
                "Invocation looses 2 ATK",
                -2,
                0
            ),
            new ProtectFromDestructionAbility(
                EquipmentAbilityName.ProtectOneTimeFromDestruction,
                "Equipment is destroyed instead of the invocation if the invocationCard should be destroyed"
            ),
            new CancelInvocationAbility(
                EquipmentAbilityName.CancelInvocationAbility,
                "Invocation whose has this equipment card loose its abilities"
            )
        };

        _abilityDictionary = abilities.ToDictionary(ability => ability.Name, ability => ability);
    }

    /// <summary>
    /// Gets an equipment ability by its name.
    /// </summary>
    /// <param name="abilityName">The ability name to look up.</param>
    /// <returns>The equipment ability, or null if not found.</returns>
    public EquipmentAbility GetAbility(EquipmentAbilityName abilityName)
    {
        _abilityDictionary.TryGetValue(abilityName, out var ability);
        return ability;
    }

    /// <summary>
    /// Checks if an ability exists for the given name.
    /// </summary>
    /// <param name="abilityName">The ability name to check.</param>
    /// <returns>True if the ability exists.</returns>
    public bool HasAbility(EquipmentAbilityName abilityName)
    {
        return _abilityDictionary.ContainsKey(abilityName);
    }

    /// <summary>
    /// Gets a modern IAbility implementation by equipment ability name.
    /// Phase 103: New method for clean architecture migration.
    /// </summary>
    /// <param name="abilityName">The domain ability name to look up.</param>
    /// <returns>The modern ability, or null if not found.</returns>
    public IAbility GetModernAbility(DomainEquipmentAbilityName abilityName)
    {
        _modernAbilityDictionary.TryGetValue(abilityName, out var ability);
        return ability;
    }

    /// <summary>
    /// Initializes the modern ability dictionary with all equipment abilities.
    /// Maps DomainEquipmentAbilityName to IAbility implementations.
    /// </summary>
    private void InitializeModernAbilities()
    {
        _modernAbilityDictionary = new Dictionary<DomainEquipmentAbilityName, IAbility>
        {
            // Multiply stats abilities
            [DomainEquipmentAbilityName.MultiplyDefBy2ButPreventAttack] = _factory.CreateMultiplyStats(1f, 2f),
            [DomainEquipmentAbilityName.MultiplyAtkBy3] = _factory.CreateMultiplyStats(3f, 1f),
            [DomainEquipmentAbilityName.MultiplyAtkBy2AndDefByHalf] = _factory.CreateMultiplyStats(2f, 0.5f),

            // Set stats abilities
            [DomainEquipmentAbilityName.SetATKToOne] = _factory.CreateSetStats(1, -1), // -1 means no change
            [DomainEquipmentAbilityName.SetDefToZero] = _factory.CreateSetStats(-1, 0), // -1 means no change

            // Bonus stats abilities
            [DomainEquipmentAbilityName.Earn1ATKAndMinus1DEF] = _factory.CreateBonusStats(1, -1),
            [DomainEquipmentAbilityName.Remove1ATKAnd1DEF] = _factory.CreateBonusStats(-1, -1),
            [DomainEquipmentAbilityName.Earn2ATK] = _factory.CreateBonusStats(2, 0),
            [DomainEquipmentAbilityName.Earn3ATKAndMinus1DEF] = _factory.CreateBonusStats(3, -1),
            [DomainEquipmentAbilityName.Earn1ATKAnd1DEF] = _factory.CreateBonusStats(1, 1),
            [DomainEquipmentAbilityName.Loose2ATK] = _factory.CreateBonusStats(-2, 0),

            // Hand-based stats abilities
            [DomainEquipmentAbilityName.EarnOneQuarterATKPerHandCards] = _factory.CreateHandBasedStats(0.25f, 0f),
            [DomainEquipmentAbilityName.EarnOneQuarterDEFPerHandCards] = _factory.CreateHandBasedStats(0f, 0.25f),

            // Direct attack ability
            [DomainEquipmentAbilityName.DirectAttack] = _factory.CreateDirectAttack(),

            // Prevention abilities
            [DomainEquipmentAbilityName.PreventNewOpponentToAttack] = _factory.CreatePreventAttackNew(),
            [DomainEquipmentAbilityName.CantBeAttackByOtherInvocations] = _factory.CreateCantBeAttacked(),

            // Protection abilities
            [DomainEquipmentAbilityName.ProtectOneTimeFromDestruction] = _factory.CreateProtectFromDestruction(),

            // Switch equipment ability
            [DomainEquipmentAbilityName.SwitchEquipmentCard] = _factory.CreateSwitchEquipment(),

            // Cancel abilities
            [DomainEquipmentAbilityName.CancelInvocationAbility] = _factory.CreateCancelAbilities()
        };
    }
}
#pragma warning restore CS0618
