using System.Collections.Generic;
using JDG.Application;
using JDG.Application.Abilities;
using JDG.Application.Repositories;
using JDG.Domain;
using UnityEngine;

/// <summary>
/// Service that provides abilities from the modern AbilityRegistry system.
///
/// Phase 7: Part of AbilityLibrary → AbilityRegistry migration.
/// Phase 42ag: Legacy AbilityLibrary removed - all abilities now use modern system.
///
/// This service wraps modern IAbility implementations in ModernAbilityAdapter
/// for compatibility with the legacy Ability base class interface.
/// </summary>
public class AbilityProviderService : IAbilityProvider
{
    private readonly AbilityRegistry _registry;
    private readonly IPlayerRepository _playerRepository;
    private readonly IEventBus _eventBus;
    private readonly Dictionary<AbilityName, Ability> _adapterCache;

    /// <summary>
    /// Tracks which abilities have been served from the modern system.
    /// Useful for migration progress reporting.
    /// </summary>
    private readonly HashSet<AbilityName> _modernAbilitiesUsed;

    /// <summary>
    /// Tracks which abilities have been served from the legacy system.
    /// Useful for identifying remaining migration work.
    /// </summary>
    private readonly HashSet<AbilityName> _legacyAbilitiesUsed;

    public AbilityProviderService(
        AbilityRegistry registry,
        IPlayerRepository playerRepository = null,
        IEventBus eventBus = null)
    {
        _registry = registry;
        _playerRepository = playerRepository;
        _eventBus = eventBus;
        _adapterCache = new Dictionary<AbilityName, Ability>();
        _modernAbilitiesUsed = new HashSet<AbilityName>();
        _legacyAbilitiesUsed = new HashSet<AbilityName>();
    }

    /// <summary>
    /// Gets an ability by name from the modern AbilityRegistry.
    /// Phase 42ag: Legacy fallback removed - all abilities use modern system.
    /// </summary>
    public Ability GetAbility(AbilityName abilityName)
    {
        // Check adapter cache first (for modern abilities already wrapped)
        if (_adapterCache.TryGetValue(abilityName, out var cachedAdapter))
        {
            return cachedAdapter;
        }

        // Get from modern system
        if (_registry != null && _registry.IsRegistered(abilityName))
        {
            try
            {
                var modernAbility = _registry.GetAbility(abilityName);
                var adapter = new ModernAbilityAdapter(modernAbility, _playerRepository, _eventBus);
                _adapterCache[abilityName] = adapter;
                _modernAbilitiesUsed.Add(abilityName);

                Debug.Log($"[AbilityProviderService] Loaded ability '{abilityName}' from modern system");
                return adapter;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AbilityProviderService] Failed to load '{abilityName}' from modern system: {ex.Message}");
                return null;
            }
        }

        // Ability not found
        Debug.LogError($"[AbilityProviderService] Ability '{abilityName}' not found in AbilityRegistry!");
        return null;
    }

    /// <summary>
    /// Checks if an ability exists in the AbilityRegistry.
    /// Phase 42ag: Legacy fallback removed.
    /// </summary>
    public bool HasAbility(AbilityName abilityName)
    {
        return _registry != null && _registry.IsRegistered(abilityName);
    }

    /// <summary>
    /// Checks if an ability has been migrated to the modern IAbility system.
    /// </summary>
    public bool IsMigrated(AbilityName abilityName)
    {
        return _registry != null && _registry.IsRegistered(abilityName);
    }

    /// <summary>
    /// Gets a modern IAbility implementation directly from the registry.
    /// Phase 105: New method for clean architecture migration - bypasses ModernAbilityAdapter.
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
                UnityEngine.Debug.LogError($"[AbilityProviderService] Failed to get modern ability '{abilityName}': {ex.Message}");
                return null;
            }
        }
        return null;
    }

    /// <summary>
    /// Gets migration statistics for reporting.
    /// </summary>
    public MigrationStats GetMigrationStats()
    {
        return new MigrationStats
        {
            ModernAbilitiesUsed = _modernAbilitiesUsed.Count,
            LegacyAbilitiesUsed = _legacyAbilitiesUsed.Count,
            TotalAbilitiesRegistered = _registry?.GetRegisteredAbilities() != null
                ? System.Linq.Enumerable.Count(_registry.GetRegisteredAbilities())
                : 0
        };
    }

    /// <summary>
    /// Clears the adapter cache. Useful for testing.
    /// </summary>
    public void ClearCache()
    {
        _adapterCache.Clear();
    }

    /// <summary>
    /// Statistics about ability migration progress.
    /// </summary>
    public class MigrationStats
    {
        public int ModernAbilitiesUsed { get; set; }
        public int LegacyAbilitiesUsed { get; set; }
        public int TotalAbilitiesRegistered { get; set; }
    }
}
