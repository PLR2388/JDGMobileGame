using System.Collections.Generic;
using System.Linq;

public class EquipmentAbilityLibrary : StaticInstance<EquipmentAbilityLibrary>
{
    private List<EquipmentAbility> equipmentAbilities = new List<EquipmentAbility>
    {
        new MultiplyAtkDefAbility(
            EquipmentAbilityName.MultiplyDefBy2ButPreventAttack,
            "Multiply DEF by 2 but the invocation cannot attack",
            defenseFactor: 2f,
            preventAttack: true
        )
    };

    public Dictionary<EquipmentAbilityName, EquipmentAbility> equipmentAbilityDictionary;

    protected override void Awake()
    {
        base.Awake();
        equipmentAbilityDictionary = equipmentAbilities.ToDictionary(ability => ability.Name, ability => ability);
    }
}