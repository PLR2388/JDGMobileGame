using System.Collections.Generic;


public class AbilityLibrary : StaticInstance<AbilityLibrary>
{
    public List<Ability> abilities = new List<Ability>()
    {
        new CanOnlyAttackItselfAbility(
            AbilityName.CanOnlyAttackItself,
            "Opponent can only attach this card"
        )
    };
}