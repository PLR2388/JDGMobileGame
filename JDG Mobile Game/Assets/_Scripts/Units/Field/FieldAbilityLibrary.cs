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
        )
    };

    public Dictionary<FieldAbilityName, FieldAbility> fieldAbilityDictionary;

    protected override void Awake()
    {
        base.Awake();
        fieldAbilityDictionary = fieldAbilities.ToDictionary(ability => ability.Name, ability => ability);
    }
}