using JDG.Application.Services;using System.Collections.Generic;
using JDG.Application.Abilities;
using JDG.Application.Abilities.Implementations;
using JDG.Application.Repositories;
using DomainFieldAbilityName = JDG.Domain.Enums.FieldAbilityName;
using DomainCardFamily = JDG.Domain.Enums.CardFamily;

/// <summary>
/// Provides field abilities for field card effects during gameplay.
/// Phase 48: Originally wrapped legacy singleton for DI-compatible access.
/// Phase 84: Owned the legacy ability dictionary directly.
/// Phase 104: Added modern IAbility support via GetModernAbility.
/// Phase 116: Removed legacy ability dictionary - now uses only modern IAbility.
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
public class FieldAbilityProviderService : IFieldAbilityProvider
{
    private Dictionary<DomainFieldAbilityName, IAbility> _modernAbilityDictionary;
    private readonly FieldAbilityFactory _factory;

    /// <summary>
    /// Initializes the field ability provider with modern abilities only.
    /// Phase 116: Removed legacy ability initialization.
    /// Phase 153: Added warning log when playerRepository is null.
    /// </summary>
    public FieldAbilityProviderService(IPlayerRepository playerRepository = null)
    {
        // Initialize modern factory if repository provided
        if (playerRepository != null)
        {
            _factory = new FieldAbilityFactory(playerRepository);
            InitializeModernAbilities();
        }
        else
        {
            // Phase 153: Log warning to help debug ability issues
            UnityEngine.Debug.LogWarning("[FieldAbilityProviderService] Created with null playerRepository - abilities will be empty. " +
                "This is expected during deck building but may cause issues during gameplay.");
            _modernAbilityDictionary = new Dictionary<DomainFieldAbilityName, IAbility>();
        }
    }

    /// <summary>
    /// Gets a modern IAbility implementation by field ability name.
    /// Phase 116: Now the primary (and only) lookup method.
    /// Phase 156: Returns DefaultAbility instead of null for consistency with AbilityProviderService.
    /// </summary>
    /// <param name="abilityName">The domain ability name to look up.</param>
    /// <returns>The modern ability, or DefaultAbility if not found.</returns>
    public IAbility GetModernAbility(DomainFieldAbilityName abilityName)
    {
        if (_modernAbilityDictionary.TryGetValue(abilityName, out var ability))
        {
            return ability;
        }

        // Phase 156: Return DefaultAbility instead of null for consistent behavior
        // Phase 158: Changed from .None to .Default
        if (abilityName != DomainFieldAbilityName.Default)
        {
            UnityEngine.Debug.LogWarning($"[FieldAbilityProviderService] Ability '{abilityName}' not found in registry. " +
                "Returning DefaultAbility. This may indicate a missing ability registration.");
        }
        return new DefaultAbility();
    }

    /// <summary>
    /// Checks if an ability exists for the given name.
    /// </summary>
    /// <param name="abilityName">The ability name to check.</param>
    /// <returns>True if the ability exists.</returns>
    public bool HasAbility(DomainFieldAbilityName abilityName)
    {
        return _modernAbilityDictionary.ContainsKey(abilityName);
    }

    /// <summary>
    /// Initializes the modern ability dictionary with all field abilities.
    /// Maps DomainFieldAbilityName to IAbility implementations.
    /// </summary>
    private void InitializeModernAbilities()
    {
        _modernAbilityDictionary = new Dictionary<DomainFieldAbilityName, IAbility>
        {
            // Family boost abilities
            [DomainFieldAbilityName.Earn1DEFForSpatialFamily] = _factory.CreateFamilyBoost(
                DomainCardFamily.Spatial, 0f, 1f),
            [DomainFieldAbilityName.Earn1HalfDEFAndMinusHalfATKForDevFamily] = _factory.CreateFamilyBoost(
                DomainCardFamily.Developer, -0.5f, 1.5f),
            [DomainFieldAbilityName.Earn2DEFAndMinusOneATKForIncarnationFamily] = _factory.CreateFamilyBoost(
                DomainCardFamily.Incarnation, -1f, 2f),
            [DomainFieldAbilityName.Earn1ATKForJapanFamily] = _factory.CreateFamilyBoost(
                DomainCardFamily.Japan, 1f, 0f),
            [DomainFieldAbilityName.Earn1HalfATKAndMinusHalfDEFForHCFamily] = _factory.CreateFamilyBoost(
                DomainCardFamily.HardCorner, 1.5f, -0.5f),
            [DomainFieldAbilityName.EarnHalfATKAndDefForRpgFamily] = _factory.CreateFamilyBoost(
                DomainCardFamily.Rpg, 0.5f, 0.5f),
            [DomainFieldAbilityName.Earn2ATKAndMinus1DEFForComicsFamily] = _factory.CreateFamilyBoost(
                DomainCardFamily.Comics, 2f, -1f),

            // Card-specific family changes
            [DomainFieldAbilityName.ChangeJMBruitagesFamilyToDev] = _factory.CreateChangeByName(
                "Jean-Michel Bruitages", DomainCardFamily.Developer),
            [DomainFieldAbilityName.ChangePatronInfogramFamilyToDev] = _factory.CreateChangeByName(
                "Patron D'Infogrames", DomainCardFamily.Developer),

            // Heal per family ability
            [DomainFieldAbilityName.EarnHalfHPPerWizardInvocationEachTurn] = _factory.CreateHealPerFamily(
                DomainCardFamily.Wizard, 0.5f),

            // Draw bonus ability
            [DomainFieldAbilityName.DrawOneMoreCard] = _factory.CreateDrawBonus(1),

            // Skip draw for family card ability
            [DomainFieldAbilityName.SkipDrawToGetFistilandInvocation] = _factory.CreateSkipDrawForFamily(
                DomainCardFamily.Fistiland)
        };
    }
}
