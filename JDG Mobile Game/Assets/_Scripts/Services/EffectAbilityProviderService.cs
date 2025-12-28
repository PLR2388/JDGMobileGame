/// <summary>
/// Provides effect abilities using EffectAbilityLibrary.
/// Phase 48: Wraps legacy singleton for DI-compatible access.
/// Will be migrated to use modern IAbility implementations in future phases.
/// </summary>
public class EffectAbilityProviderService : IEffectAbilityProvider
{
    /// <summary>
    /// Gets an effect ability by its name.
    /// </summary>
    /// <param name="abilityName">The ability name to look up.</param>
    /// <returns>The effect ability, or null if not found.</returns>
    public EffectAbility GetAbility(EffectAbilityName abilityName)
    {
        var library = EffectAbilityLibrary.Instance;
        if (library == null || library.EffectAbilityDictionary == null)
            return null;

        library.EffectAbilityDictionary.TryGetValue(abilityName, out var ability);
        return ability;
    }

    /// <summary>
    /// Checks if an ability exists for the given name.
    /// </summary>
    /// <param name="abilityName">The ability name to check.</param>
    /// <returns>True if the ability exists.</returns>
    public bool HasAbility(EffectAbilityName abilityName)
    {
        var library = EffectAbilityLibrary.Instance;
        if (library == null || library.EffectAbilityDictionary == null)
            return false;

        return library.EffectAbilityDictionary.ContainsKey(abilityName);
    }
}
