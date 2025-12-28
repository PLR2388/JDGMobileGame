/// <summary>
/// Provides equipment abilities by name for InGameEquipmentCard.
/// Phase 48: Abstracts EquipmentAbilityLibrary.Instance access for DI.
/// </summary>
public interface IEquipmentAbilityProvider
{
    /// <summary>
    /// Gets an equipment ability by its name.
    /// </summary>
    /// <param name="abilityName">The ability name to look up.</param>
    /// <returns>The equipment ability, or null if not found.</returns>
    EquipmentAbility GetAbility(EquipmentAbilityName abilityName);

    /// <summary>
    /// Checks if an ability exists for the given name.
    /// </summary>
    /// <param name="abilityName">The ability name to check.</param>
    /// <returns>True if the ability exists.</returns>
    bool HasAbility(EquipmentAbilityName abilityName);
}
