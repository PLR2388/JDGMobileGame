/// <summary>
/// Provides equipment abilities using EquipmentAbilityLibrary.
/// Phase 48: Wraps legacy singleton for DI-compatible access.
/// Will be migrated to use modern IAbility implementations in future phases.
/// </summary>
public class EquipmentAbilityProviderService : IEquipmentAbilityProvider
{
    /// <summary>
    /// Gets an equipment ability by its name.
    /// </summary>
    /// <param name="abilityName">The ability name to look up.</param>
    /// <returns>The equipment ability, or null if not found.</returns>
    public EquipmentAbility GetAbility(EquipmentAbilityName abilityName)
    {
        var library = EquipmentAbilityLibrary.Instance;
        if (library == null || library.EquipmentAbilityDictionary == null)
            return null;

        library.EquipmentAbilityDictionary.TryGetValue(abilityName, out var ability);
        return ability;
    }

    /// <summary>
    /// Checks if an ability exists for the given name.
    /// </summary>
    /// <param name="abilityName">The ability name to check.</param>
    /// <returns>True if the ability exists.</returns>
    public bool HasAbility(EquipmentAbilityName abilityName)
    {
        var library = EquipmentAbilityLibrary.Instance;
        if (library == null || library.EquipmentAbilityDictionary == null)
            return false;

        return library.EquipmentAbilityDictionary.ContainsKey(abilityName);
    }
}
