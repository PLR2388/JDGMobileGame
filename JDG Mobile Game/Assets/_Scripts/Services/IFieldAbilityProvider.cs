using JDG.Application.Abilities;
using DomainFieldAbilityName = JDG.Domain.Enums.FieldAbilityName;

/// <summary>
/// Provides field abilities by name for InGameFieldCard.
/// Phase 48: Abstracts FieldAbilityLibrary.Instance access for DI.
/// Phase 104: Added GetModernAbility for IAbility migration.
/// </summary>
public interface IFieldAbilityProvider
{
    /// <summary>
    /// Gets a legacy field ability by its name.
    /// Phase 104: Marked for deprecation - use GetModernAbility instead.
    /// </summary>
    /// <param name="abilityName">The ability name to look up.</param>
    /// <returns>The field ability, or null if not found.</returns>
    FieldAbility GetAbility(FieldAbilityName abilityName);

    /// <summary>
    /// Gets a modern IAbility implementation by field ability name.
    /// Phase 104: New method for clean architecture migration.
    /// </summary>
    /// <param name="abilityName">The domain ability name to look up.</param>
    /// <returns>The modern ability, or null if not found.</returns>
    IAbility GetModernAbility(DomainFieldAbilityName abilityName);

    /// <summary>
    /// Checks if an ability exists for the given name.
    /// </summary>
    /// <param name="abilityName">The ability name to check.</param>
    /// <returns>True if the ability exists.</returns>
    bool HasAbility(FieldAbilityName abilityName);
}
