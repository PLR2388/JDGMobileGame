using System.Collections.Generic;
using System.Linq;

public class EffectAbilityLibrary : StaticInstance<EffectAbilityLibrary>
{
    private List<EffectAbility> effectAbilities = new List<EffectAbility>
    {
        new LimitHandCards(
            EffectAbilityName.LimitHandCardTo5,
            "Limit the number of cards in the hands to 5",
            5
        )
    };

    public Dictionary<EffectAbilityName, EffectAbility> effectAbilityDictionary;

    protected override void Awake()
    {
        base.Awake();
        effectAbilityDictionary = effectAbilities.ToDictionary(ability => ability.Name, ability => ability);
    }
}