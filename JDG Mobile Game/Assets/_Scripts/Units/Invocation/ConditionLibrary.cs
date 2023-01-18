
using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation.Condition;

public class ConditionLibrary : StaticInstance<ConditionLibrary>
{
    private List<Condition> conditions = new List<Condition>()
    {
        new InvocationCardOnFieldCondition(
            ConditionName.InvocationCardOnFieldCondition, 
            "Check if at least one invocation from the list is on the field",
            new List<string>
            {
                "Benzaie jeune",
                "Benzaie"
            }
        )
    };

    public Dictionary<ConditionName, Condition> conditionDictionary;

    protected override void Awake()
    {
        base.Awake();
        conditionDictionary = conditions.ToDictionary(condition => condition.Name, condition => condition);
    }
}