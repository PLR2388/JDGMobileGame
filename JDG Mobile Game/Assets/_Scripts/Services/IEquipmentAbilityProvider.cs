using JDG.Application.Abilities;
using DomainEquipmentAbilityName = JDG.Domain.Enums.EquipmentAbilityName;

/// <summary>
/// Provides equipment abilities by name for InGameEquipmentCard.
/// Phase 48: Abstracts EquipmentAbilityLibrary.Instance access for DI.
/// Phase 103: Added GetModernAbility for IAbility migration.
/// Phase 117: Removed legacy GetAbility method - now uses only modern IAbility.
/// </summary>
public interface IEquipmentAbilityProvider
{
    /// <summary>
    /// Gets a modern IAbility implementation by equipment ability name.
    /// Phase 117: Now the primary (and only) lookup method.
    /// </summary>
    /// <param name="abilityName">The domain ability name to look up.</param>
    /// <returns>The modern ability, or null if not found.</returns>
    IAbility GetModernAbility(DomainEquipmentAbilityName abilityName);

    /// <summary>
    /// Checks if an ability exists for the given name.
    /// </summary>
    /// <param name="abilityName">The ability name to check.</param>
    /// <returns>True if the ability exists.</returns>
    bool HasAbility(DomainEquipmentAbilityName abilityName);
}
