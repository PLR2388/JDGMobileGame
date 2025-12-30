using JDG.Application.Abilities;
using JDG.Domain;

/// <summary>
/// Provides abilities for card initialization.
/// This interface abstracts the ability lookup mechanism, allowing migration
/// from AbilityLibrary singleton to dependency-injected AbilityRegistry.
///
/// Phase 7: Part of AbilityLibrary → AbilityRegistry migration.
/// Phase 105: Added GetModernAbility for direct IAbility access.
/// </summary>
public interface IAbilityProvider
{
    /// <summary>
    /// Gets a legacy Ability instance by name.
    /// Returns wrapped modern IAbility if available, otherwise falls back to AbilityLibrary.
    /// Phase 105: Marked for deprecation - use GetModernAbility instead.
    /// </summary>
    /// <param name="abilityName">The ability name to look up</param>
    /// <returns>An Ability instance (either wrapped modern or legacy)</returns>
    Ability GetAbility(AbilityName abilityName);

    /// <summary>
    /// Gets a modern IAbility implementation by name.
    /// Phase 105: New method for clean architecture migration.
    /// </summary>
    /// <param name="abilityName">The ability name to look up</param>
    /// <returns>The modern ability, or null if not found</returns>
    IAbility GetModernAbility(AbilityName abilityName);

    /// <summary>
    /// Checks if an ability exists in the provider.
    /// </summary>
    /// <param name="abilityName">The ability name to check</param>
    /// <returns>True if the ability exists</returns>
    bool HasAbility(AbilityName abilityName);

    /// <summary>
    /// Checks if an ability has been migrated to the modern IAbility system.
    /// </summary>
    /// <param name="abilityName">The ability name to check</param>
    /// <returns>True if using modern IAbility implementation</returns>
    bool IsMigrated(AbilityName abilityName);
}
