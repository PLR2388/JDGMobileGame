using JDG.Application.Abilities;
using JDG.Domain;
using UnityEngine;

/// <summary>
/// Service that provides abilities from the modern AbilityRegistry system.
///
/// Phase 7: Part of AbilityLibrary → AbilityRegistry migration.
/// Phase 42ag: Legacy AbilityLibrary removed - all abilities now use modern system.
/// Phase 118: Removed legacy GetAbility and ModernAbilityAdapter usage.
///
/// This service provides direct access to IAbility implementations from the registry.
/// </summary>
public class AbilityProviderService : IAbilityProvider
{
    private readonly AbilityRegistry _registry;

    public AbilityProviderService(AbilityRegistry registry)
    {
        _registry = registry;
    }

    /// <summary>
    /// Checks if an ability exists in the AbilityRegistry.
    /// </summary>
    public bool HasAbility(AbilityName abilityName)
    {
        return _registry != null && _registry.IsRegistered(abilityName);
    }

    /// <summary>
    /// Gets a modern IAbility implementation directly from the registry.
    /// Phase 118: This is now the only ability lookup method.
    /// </summary>
    /// <param name="abilityName">The ability name to look up.</param>
    /// <returns>The modern ability, or null if not found.</returns>
    public IAbility GetModernAbility(AbilityName abilityName)
    {
        if (_registry != null && _registry.IsRegistered(abilityName))
        {
            try
            {
                return _registry.GetAbility(abilityName);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AbilityProviderService] Failed to get ability '{abilityName}': {ex.Message}");
                return null;
            }
        }
        return null;
    }
}
