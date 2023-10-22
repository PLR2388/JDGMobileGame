
/// <summary>
/// Represents the default implementation of an ability. This is a base ability
/// with no added behaviors other than setting the name and description.
/// </summary>
public class DefaultAbility : Ability
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the ability.</param>
    /// <param name="description">The description of the ability.</param>
    public DefaultAbility(AbilityName name, string description)
    {
        Name = name;
        Description = description;
    }
}