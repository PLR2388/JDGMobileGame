using System.Collections.Generic;
using JDG.Application.Abilities;
using JDG.Application.Abilities.Implementations;
using JDG.Application.Repositories;
using DomainEffectAbilityName = JDG.Domain.Enums.EffectAbilityName;

/// <summary>
/// Provides effect abilities for effect card effects during gameplay.
/// Phase 48: Originally wrapped legacy singleton for DI-compatible access.
/// Phase 84: Owned the legacy ability dictionary directly.
/// Phase 102: Added modern IAbility support via GetModernAbility.
/// Phase 115: Removed legacy ability dictionary - now uses only modern IAbility.
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
public class EffectAbilityProviderService : IEffectAbilityProvider
{
    private Dictionary<DomainEffectAbilityName, IAbility> _modernAbilityDictionary;
    private readonly EffectAbilityFactory _factory;

    /// <summary>
    /// Initializes the effect ability provider with modern abilities only.
    /// Phase 115: Removed legacy ability initialization.
    /// Phase 153: Added warning log when playerRepository is null.
    /// </summary>
    public EffectAbilityProviderService(IPlayerRepository playerRepository = null)
    {
        // Initialize modern factory if repository provided
        if (playerRepository != null)
        {
            _factory = new EffectAbilityFactory(playerRepository);
            InitializeModernAbilities();
        }
        else
        {
            // Phase 153: Log warning to help debug ability issues
            UnityEngine.Debug.LogWarning("[EffectAbilityProviderService] Created with null playerRepository - abilities will be empty. " +
                "This is expected during deck building but may cause issues during gameplay.");
            _modernAbilityDictionary = new Dictionary<DomainEffectAbilityName, IAbility>();
        }
    }

    /// <summary>
    /// Gets a modern IAbility implementation by effect ability name.
    /// Phase 102: Primary method for ability lookup.
    /// Phase 115: Now the only lookup method (legacy GetAbility removed).
    /// </summary>
    /// <param name="abilityName">The domain ability name to look up.</param>
    /// <returns>The modern ability, or null if not found.</returns>
    public IAbility GetModernAbility(DomainEffectAbilityName abilityName)
    {
        _modernAbilityDictionary.TryGetValue(abilityName, out var ability);
        return ability;
    }

    /// <summary>
    /// Checks if an ability exists for the given name.
    /// </summary>
    /// <param name="abilityName">The ability name to check.</param>
    /// <returns>True if the ability exists.</returns>
    public bool HasAbility(DomainEffectAbilityName abilityName)
    {
        return _modernAbilityDictionary.ContainsKey(abilityName);
    }

    /// <summary>
    /// Initializes the modern ability dictionary with all effect abilities.
    /// Maps DomainEffectAbilityName to IAbility implementations.
    /// </summary>
    private void InitializeModernAbilities()
    {
        _modernAbilityDictionary = new Dictionary<DomainEffectAbilityName, IAbility>
        {
            // Hand manipulation
            [DomainEffectAbilityName.LimitHandCardTo5] = _factory.CreateLimitHand(5),

            // Damage abilities
            [DomainEffectAbilityName.Lose2Point5StarsByInvocations] = _factory.CreateDamageOpponent(
                DamageCalculationType.ByPlayerInvocationCount, 2.5f),
            [DomainEffectAbilityName.LooseHPBasedOnNumberInvocation] = _factory.CreateDamageOpponent(
                DamageCalculationType.ByOpponentInvocationCount, 2.5f),
            [DomainEffectAbilityName.Loose1HPPerOpponentHandCards] = _factory.CreateDamageOpponent(
                DamageCalculationType.ByOpponentHandCount, 1f),

            // Healing abilities
            [DomainEffectAbilityName.GetHPFor1Sacrifice3ATKDEFCondition] = _factory.CreateHealPlayer(15f, 3),
            [DomainEffectAbilityName.Get7HalfHPFor1Sacrifice] = _factory.CreateHealPlayer(7.5f),
            [DomainEffectAbilityName.GetBackAllHPBySacrifice5AtkDef] = _factory.CreateHealPlayer(0f, 5), // 0 = full heal

            // Attack modification abilities
            [DomainEffectAbilityName.DirectAttackIfUnder5HP] = _factory.CreateEnableDirectAttack(5f),
            [DomainEffectAbilityName.DoubleAttackPerTurn] = _factory.CreateDoubleAttacks(1),
            [DomainEffectAbilityName.ManiabilitePourrieSkipAttackForOpponent] = _factory.CreateSkipAttackPhase(),

            // Card destruction abilities
            [DomainEffectAbilityName.DestroyAllCardsUnderManyConditions] = _factory.CreateDestroyMultiple(0), // All
            [DomainEffectAbilityName.DestroyOneCardByRemovingOneHandCard] = _factory.CreateDestroyMultiple(1),
            [DomainEffectAbilityName.DestroyEquipmentCard] = _factory.CreateDestroyMultiple(1),
            [DomainEffectAbilityName.DestroyOpponentInvocationCard] = _factory.CreateDestroyMultiple(1),
            [DomainEffectAbilityName.DestroyFieldFor7HalfCost] = _factory.CreateDestroyFieldCard(7.5f),

            // Card manipulation abilities
            [DomainEffectAbilityName.ChangeFieldCardFromDeck] = _factory.CreateChangeField(),
            [DomainEffectAbilityName.GetCardFromYellowDeck] = _factory.CreateDrawFromGraveyard(),
            [DomainEffectAbilityName.InvokeCardFromYellowTrash] = _factory.CreateInvokeFromDeck(),

            // View abilities
            [DomainEffectAbilityName.LookAndOrderDeckCards] = _factory.CreateLookDeck(5),
            [DomainEffectAbilityName.LookOpponentHandCardsAndChangeIt] = _factory.CreateLookHand(),

            // Stat modification abilities
            [DomainEffectAbilityName.SwitchAtkDef] = _factory.CreateSwapStats(),
            [DomainEffectAbilityName.DivideDEFOpponentBy2] = _factory.CreateDivideDefense(2),

            // Shield abilities
            [DomainEffectAbilityName.Add3ShieldsForUser] = _factory.CreateAddShields(3),

            // Control abilities
            [DomainEffectAbilityName.Control1OpponentInvocationCard] = _factory.CreateControlCard(),

            // Field effect abilities
            [DomainEffectAbilityName.ApplyFamilyFieldToInvocations] = _factory.CreateApplyFamilyField(0.5f)
        };
    }
}
