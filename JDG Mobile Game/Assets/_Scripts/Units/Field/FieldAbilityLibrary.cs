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
            )
    };

    public Dictionary<FieldAbilityName, FieldAbility> fieldAbilityDictionary;

    protected override void Awake()
    {
        base.Awake();
        fieldAbilityDictionary = fieldAbilities.ToDictionary(ability => ability.Name, ability => ability);
    }
}