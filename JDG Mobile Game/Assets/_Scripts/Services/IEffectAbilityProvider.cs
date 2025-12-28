/// <summary>
/// Provides effect abilities by name for InGameEffectCard.
/// Phase 48: Abstracts EffectAbilityLibrary.Instance access for DI.
/// </summary>
public interface IEffectAbilityProvider
{
    /// <summary>
    /// Gets an effect ability by its name.
    /// </summary>
    /// <param name="abilityName">The ability name to look up.</param>
    /// <returns>The effect ability, or null if not found.</returns>
    EffectAbility GetAbility(EffectAbilityName abilityName);

    /// <summary>
    /// Checks if an ability exists for the given name.
    /// </summary>
    /// <param name="abilityName">The ability name to check.</param>
    /// <returns>True if the ability exists.</returns>
    bool HasAbility(EffectAbilityName abilityName);
}
