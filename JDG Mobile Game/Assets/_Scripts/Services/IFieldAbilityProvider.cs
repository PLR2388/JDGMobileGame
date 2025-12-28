/// <summary>
/// Provides field abilities by name for InGameFieldCard.
/// Phase 48: Abstracts FieldAbilityLibrary.Instance access for DI.
/// </summary>
public interface IFieldAbilityProvider
{
    /// <summary>
    /// Gets a field ability by its name.
    /// </summary>
    /// <param name="abilityName">The ability name to look up.</param>
    /// <returns>The field ability, or null if not found.</returns>
    FieldAbility GetAbility(FieldAbilityName abilityName);

    /// <summary>
    /// Checks if an ability exists for the given name.
    /// </summary>
    /// <param name="abilityName">The ability name to check.</param>
    /// <returns>True if the ability exists.</returns>
    bool HasAbility(FieldAbilityName abilityName);
}
