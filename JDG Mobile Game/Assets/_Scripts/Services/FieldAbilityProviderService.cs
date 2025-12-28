/// <summary>
/// Provides field abilities using FieldAbilityLibrary.
/// Phase 48: Wraps legacy singleton for DI-compatible access.
/// Will be migrated to use modern IAbility implementations in future phases.
/// </summary>
public class FieldAbilityProviderService : IFieldAbilityProvider
{
    /// <summary>
    /// Gets a field ability by its name.
    /// </summary>
    /// <param name="abilityName">The ability name to look up.</param>
    /// <returns>The field ability, or null if not found.</returns>
    public FieldAbility GetAbility(FieldAbilityName abilityName)
    {
        var library = FieldAbilityLibrary.Instance;
        if (library == null || library.FieldAbilityDictionary == null)
            return null;

        library.FieldAbilityDictionary.TryGetValue(abilityName, out var ability);
        return ability;
    }

    /// <summary>
    /// Checks if an ability exists for the given name.
    /// </summary>
    /// <param name="abilityName">The ability name to check.</param>
    /// <returns>True if the ability exists.</returns>
    public bool HasAbility(FieldAbilityName abilityName)
    {
        var library = FieldAbilityLibrary.Instance;
        if (library == null || library.FieldAbilityDictionary == null)
            return false;

        return library.FieldAbilityDictionary.ContainsKey(abilityName);
    }
}
