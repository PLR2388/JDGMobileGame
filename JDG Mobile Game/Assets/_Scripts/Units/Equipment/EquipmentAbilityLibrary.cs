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
        ),
        new MultiplyAtkDefAbility(
            EquipmentAbilityName.MultiplyAtkBy3,
            "Multiply ATK by 3",
            3
        ),
        new SetAtkDefAbility(
            EquipmentAbilityName.SetDefToZero,
            "Invocation now has a DEF of 0",
            def: 0
        ),
        new EarnAtkDefAbility(
            EquipmentAbilityName.Earn2ATK,
            "Invocation earns 2 ATK",
            2,
            0
        ),
        new EarnAtkDefAbility(
            EquipmentAbilityName.Earn3ATKAndMinus1DEF,
            "Invocation ears 3 ATK and -1 DEF",
            3,
            -1
        ),
        new EarnAtkDefAbility(
            EquipmentAbilityName.Earn1ATKAnd1DEF,
            "Invocation earns 1 ATK and 1 DEF",
            1,
            1
        ),
        new MultiplyAtkDefAbility(
            EquipmentAbilityName.MultiplyAtkBy2AndDefByHalf,
            "Multiply ATK by 2 and DEF by 1/2",
            2,
            0.5f
        ),
        new EarnAtkDefAbility(
            EquipmentAbilityName.EarnOneQuarterDEFPerHandCards,
            "Invocation earns 0.25 DEF per hands in his hand",
            0,
            0.25f,
            true
        ),
        new SwitchEquipmentCardAbility(
            EquipmentAbilityName.SwitchEquipmentCard,
            "Player can replace an equipment card by this one and add the previous to the yellow trash"
        ),
        new EarnAtkDefAbility(
            EquipmentAbilityName.Loose2ATK,
            "Invocation looses 2 ATK",
            -2,
            0
        ),
        new ProtectFromDestructionAbility(
            EquipmentAbilityName.ProtectOneTimeFromDestruction,
            "Equipment is destroyed instead of the invocation if the invocationCard should be destroyed"
        )
    };

    public Dictionary<EquipmentAbilityName, EquipmentAbility> equipmentAbilityDictionary;

    protected override void Awake()
    {
        base.Awake();
        equipmentAbilityDictionary = equipmentAbilities.ToDictionary(ability => ability.Name, ability => ability);
    }
}