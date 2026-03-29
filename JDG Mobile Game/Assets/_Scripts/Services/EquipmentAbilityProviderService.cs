using JDG.Application.Services;using System.Collections.Generic;
using JDG.Application.Abilities;
using JDG.Application.Abilities.Implementations;
using JDG.Application.Repositories;
using DomainEquipmentAbilityName = JDG.Domain.Enums.EquipmentAbilityName;

/// <summary>
/// Provides equipment abilities for equipment card effects during gameplay.
/// Phase 48: Originally wrapped legacy singleton for DI-compatible access.
/// Phase 84: Owned the legacy ability dictionary directly.
/// Phase 103: Added modern IAbility support via GetModernAbility.
/// Phase 117: Removed legacy ability dictionary - now uses only modern IAbility.
///
/// IMPORTANT: Scope Registration Behavior
/// - SharedServicesScope: Created with null IPlayerRepository (abilities empty)
/// - GameSceneScope: Created with IPlayerRepository (abilities available)
///
/// This design is intentional:
/// - During deck building (MainScreen), abilities are not needed
/// - During gameplay (Game scene), abilities are fully functional
/// - GetModernAbility() returns null for missing abilities - callers must handle this
/// </summary>
public class EquipmentAbilityProviderService : IEquipmentAbilityProvider
{
    private Dictionary<DomainEquipmentAbilityName, IAbility> _modernAbilityDictionary;
    private readonly EquipmentAbilityFactory _factory;

    /// <summary>
    /// Initializes the equipment ability provider with modern abilities only.
    /// Phase 117: Removed legacy ability initialization.
    /// Phase 153: Added warning log when playerRepository is null.
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
            // Phase 153: Log warning to help debug ability issues
            UnityEngine.Debug.LogWarning("[EquipmentAbilityProviderService] Created with null playerRepository - abilities will be empty. " +
                "This is expected during deck building but may cause issues during gameplay.");
            _modernAbilityDictionary = new Dictionary<DomainEquipmentAbilityName, IAbility>();
        }
    }

    /// <summary>
    /// Gets a modern IAbility implementation by equipment ability name.
    /// Phase 117: Now the primary (and only) lookup method.
    /// Phase 156: Returns DefaultAbility instead of null for consistency with AbilityProviderService.
    /// </summary>
    /// <param name="abilityName">The domain ability name to look up.</param>
    /// <returns>The modern ability, or DefaultAbility if not found.</returns>
    public IAbility GetModernAbility(DomainEquipmentAbilityName abilityName)
    {
        if (_modernAbilityDictionary.TryGetValue(abilityName, out var ability))
        {
            return ability;
        }

        // Phase 156: Return DefaultAbility instead of null for consistent behavior
        // Phase 158: Changed from .None to .Default
        if (abilityName != DomainEquipmentAbilityName.Default)
        {
            UnityEngine.Debug.LogWarning($"[EquipmentAbilityProviderService] Ability '{abilityName}' not found in registry. " +
                "Returning DefaultAbility. This may indicate a missing ability registration.");
        }
        return new DefaultAbility();
    }

    /// <summary>
    /// Checks if an ability exists for the given name.
    /// </summary>
    /// <param name="abilityName">The ability name to check.</param>
    /// <returns>True if the ability exists.</returns>
    public bool HasAbility(DomainEquipmentAbilityName abilityName)
    {
        return _modernAbilityDictionary.ContainsKey(abilityName);
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
