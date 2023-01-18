using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation.Condition;

public class ConditionLibrary : StaticInstance<ConditionLibrary>
{
    private List<Condition> conditions = new List<Condition>
    {
        new InvocationCardOnFieldCondition(
            ConditionName.InvocationCardOnFieldAlphaManCondition,
            "Check if Benzaie jeune or Benzaie are on the field",
            new List<string>
            {
                "Benzaie jeune",
                "Benzaie"
            }
        ),
        new FieldCardOnFieldCondition(
            ConditionName.FieldCardOnFieldAlphaVCondition,
            "Check if the current field card on field is Zozan Kebab",
            "Zozan Kebab"
        ),
        new InvocationCardOnFieldCondition(
            ConditionName.InvocationCardOnFieldAlphaVCondition,
            "Check if Archibald Von Grenier is on the field",
            new List<string>
            {
                "Archibald Von Grenier"
            }
        ),
        new EquipmentCardOnCardCondition(
            ConditionName.EquipmentCardOnCardBenzaieCondition,
            "Check if Benzaie jeune has a Cassette VHS",
            "Cassette VHS",
            "Benzaie jeune"),
        new EquipmentCardOnCardCondition(
            ConditionName.EquipmentCardOnCardCanardmanCondition,
            "Check if Joueur du Grenier has Canarang",
            "Canarang",
            "Joueur Du Grenier"
            )
    };

    public Dictionary<ConditionName, Condition> conditionDictionary;

    protected override void Awake()
    {
        base.Awake();
        conditionDictionary = conditions.ToDictionary(condition => condition.Name, condition => condition);
    }
}