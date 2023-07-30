using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cards;
using UnityEngine;

public class FieldAbilityLibrary : StaticInstance<FieldAbilityLibrary>
{
    private List<FieldAbility> fieldAbilities = new List<FieldAbility>
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
        )
    };

    public Dictionary<FieldAbilityName, FieldAbility> fieldAbilityDictionary;

    protected override void Awake()
    {
        base.Awake();
        fieldAbilityDictionary = fieldAbilities.ToDictionary(ability => ability.Name, ability => ability);
    }
}