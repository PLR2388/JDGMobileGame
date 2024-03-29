using System.Collections.Generic;
using System.Linq;
using Cards;

/// <summary>
/// Represents a library that manages and provides access to various field abilities.
/// </summary>
public class FieldAbilityLibrary : StaticInstance<FieldAbilityLibrary>
{
    /// <summary>
    /// A collection of all field abilities defined in the library.
    /// </summary>
    private readonly List<FieldAbility> fieldAbilities = new List<FieldAbility>
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

    /// <summary>
    /// Dictionary for quick access to field abilities based on their name.
    /// </summary>
    public Dictionary<FieldAbilityName, FieldAbility> FieldAbilityDictionary;

    /// <summary>
    /// Initializes the ability library by populating the dictionary.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        FieldAbilityDictionary = fieldAbilities.ToDictionary(ability => ability.Name, ability => ability);
    }
}