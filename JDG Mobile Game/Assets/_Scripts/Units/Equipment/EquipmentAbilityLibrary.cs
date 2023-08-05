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
        )
    };

    public Dictionary<EquipmentAbilityName, EquipmentAbility> equipmentAbilityDictionary;

    protected override void Awake()
    {
        base.Awake();
        equipmentAbilityDictionary = equipmentAbilities.ToDictionary(ability => ability.Name, ability => ability);
    }
}