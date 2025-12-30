using JDG.Application.Abilities;
using DomainEffectAbilityName = JDG.Domain.Enums.EffectAbilityName;

/// <summary>
/// Provides effect abilities by name for InGameEffectCard.
/// Phase 48: Abstracts EffectAbilityLibrary.Instance access for DI.
/// Phase 102: Added GetModernAbility for IAbility migration.
/// Phase 115: Removed legacy GetAbility - now uses only modern IAbility.
/// </summary>
public interface IEffectAbilityProvider
{
    /// <summary>
    /// Gets a modern IAbility implementation by effect ability name.
    /// Phase 115: Now the primary (and only) lookup method.
    /// </summary>
    /// <param name="abilityName">The domain ability name to look up.</param>
    /// <returns>The modern ability, or null if not found.</returns>
    IAbility GetModernAbility(DomainEffectAbilityName abilityName);

    /// <summary>
    /// Checks if an ability exists for the given name.
    /// </summary>
    /// <param name="abilityName">The ability name to check.</param>
    /// <returns>True if the ability exists.</returns>
    bool HasAbility(DomainEffectAbilityName abilityName);
}
