using System.Collections.Generic;
using System.Linq;
using Cards;
using JDG.Application.Abilities;
using JDG.Application.Abilities.Implementations;
using JDG.Application.Repositories;
using DomainFieldAbilityName = JDG.Domain.Enums.FieldAbilityName;
using DomainCardFamily = JDG.Domain.Enums.CardFamily;

/// <summary>
/// Provides field abilities.
/// Phase 48: Originally wrapped legacy singleton for DI-compatible access.
/// Phase 84: Now owns the ability dictionary directly (FieldAbilityLibrary deleted).
/// Phase 104: Added modern IAbility support via GetModernAbility.
/// </summary>
#pragma warning disable CS0618 // Suppress obsolete warnings for legacy ability types
public class FieldAbilityProviderService : IFieldAbilityProvider
{
    private readonly Dictionary<FieldAbilityName, FieldAbility> _abilityDictionary;
    private Dictionary<DomainFieldAbilityName, IAbility> _modernAbilityDictionary;
    private readonly FieldAbilityFactory _factory;

    /// <summary>
    /// Initializes the field ability provider with all abilities.
    /// Phase 84: Moved from FieldAbilityLibrary.
    /// Phase 104: Added modern ability factory initialization.
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
            _modernAbilityDictionary = new Dictionary<DomainFieldAbilityName, IAbility>();
        }

        var abilities = new List<FieldAbility>
        {
            new EarnATKDEFForFamilyAbility(
                FieldAbilityName.Earn1DEFForSpatialFamily,
                "Invocations whose family is Spatial earn 1 DEF",
                0,
                1,
                CardFamily.Spatial
            ),
            new EarnATKDEFForFamilyAbility(
                FieldAbilityName.Earn1HalfDEFAndMinusHalfATKForDevFamily,
                "Invocations whose family is Developper earn 1.5 DEF and -0.5 ATK",
                -0.5f,
                1.5f,
                CardFamily.Developer
            ),
            new ChangeInvocationFamilyAbility(
                FieldAbilityName.ChangeJMBruitagesFamilyToDev,
                "Jean-Michel Bruitages has the developer family if he is on field",
                "Jean-Michel Bruitages",
                CardFamily.Developer
            ),
            new ChangeInvocationFamilyAbility(
                FieldAbilityName.ChangePatronInfogramFamilyToDev,
                "Patron D'Infogrames has the developer family if he is on field",
                "Patron D'Infogrames",
                CardFamily.Developer
            ),
            new EarnATKDEFForFamilyAbility(
                FieldAbilityName.Earn2DEFAndMinusOneATKForIncarnationFamily,
                "Invocations whose family is Incarnation earn 2 DEF and -1 ATK",
                -1,
                2,
                CardFamily.Incarnation
            ),
            new EarnHPPerFamilyOnTurnStartAbility(
                FieldAbilityName.EarnHalfHPPerWizardInvocationEachTurn,
                "Player recover 0.5 HP per invocations whose family is Wizard at each turn for which it plays",
                0.5f,
                CardFamily.Wizard
            ),
            new EarnATKDEFForFamilyAbility(
                FieldAbilityName.Earn1ATKForJapanFamily,
                "Invocations whose family is Japan earn 1 ATK",
                1,
                0,
                CardFamily.Japan
            ),
            new EarnATKDEFForFamilyAbility(
                FieldAbilityName.Earn1HalfATKAndMinusHalfDEFForHCFamily,
                "Invocations whose family is Hard Corner earn 1.5 ATK and -0.5 DEF",
                1.5f,
                -0.5f,
                CardFamily.HardCorner
            ),
            new DrawMoreCardsAbility(
                FieldAbilityName.DrawOneMoreCard,
                "Player can draw 2 cards per turn (1 additional)",
                1
            ),
            new EarnATKDEFForFamilyAbility(
                FieldAbilityName.EarnHalfATKAndDefForRpgFamily,
                "Invocations whose family is Rpg earn 0.5 ATK and 0.5 DEF",
                0.5f,
                0.5f,
                CardFamily.Rpg
            ),
            new GetCardFromFamilyIfSkipDrawAbility(
                FieldAbilityName.SkipDrawToGetFistilandInvocation,
                "Skip draw phase to get a Fistiland invocation from deck or yellow trash",
                CardFamily.Fistiland
            ),
            new EarnATKDEFForFamilyAbility(
                FieldAbilityName.Earn2ATKAndMinus1DEFForComicsFamily,
                "Invocations whose family is Comics earn 2 ATK and -1 DEF",
                2,
                -1,
                CardFamily.Comics
            )
        };

        _abilityDictionary = abilities.ToDictionary(ability => ability.Name, ability => ability);
    }

    /// <summary>
    /// Gets a field ability by its name.
    /// </summary>
    /// <param name="abilityName">The ability name to look up.</param>
    /// <returns>The field ability, or null if not found.</returns>
    public FieldAbility GetAbility(FieldAbilityName abilityName)
    {
        _abilityDictionary.TryGetValue(abilityName, out var ability);
        return ability;
    }

    /// <summary>
    /// Checks if an ability exists for the given name.
    /// </summary>
    /// <param name="abilityName">The ability name to check.</param>
    /// <returns>True if the ability exists.</returns>
    public bool HasAbility(FieldAbilityName abilityName)
    {
        return _abilityDictionary.ContainsKey(abilityName);
    }

    /// <summary>
    /// Gets a modern IAbility implementation by field ability name.
    /// Phase 104: New method for clean architecture migration.
    /// </summary>
    /// <param name="abilityName">The domain ability name to look up.</param>
    /// <returns>The modern ability, or null if not found.</returns>
    public IAbility GetModernAbility(DomainFieldAbilityName abilityName)
    {
        _modernAbilityDictionary.TryGetValue(abilityName, out var ability);
        return ability;
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
#pragma warning restore CS0618
