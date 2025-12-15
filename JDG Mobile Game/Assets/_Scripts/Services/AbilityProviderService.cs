using System.Collections.Generic;
using JDG.Application;
using JDG.Application.Abilities;
using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Infrastructure.DI;
using UnityEngine;

/// <summary>
/// Service that provides abilities from the modern AbilityRegistry system,
/// falling back to legacy AbilityLibrary when necessary.
///
/// Phase 7: Part of AbilityLibrary → AbilityRegistry migration.
///
/// This service implements the Strangler Fig pattern:
/// - First tries to get ability from modern AbilityRegistry
/// - Wraps modern IAbility in ModernAbilityAdapter for legacy compatibility
/// - Falls back to AbilityLibrary.Instance for abilities not yet migrated
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
    /// Gets an ability by name.
    /// Prefers modern AbilityRegistry, falls back to AbilityLibrary.
    /// </summary>
    public Ability GetAbility(AbilityName abilityName)
    {
        // Check adapter cache first (for modern abilities already wrapped)
        if (_adapterCache.TryGetValue(abilityName, out var cachedAdapter))
        {
            return cachedAdapter;
        }

        // Try modern system first
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
                Debug.LogWarning($"[AbilityProviderService] Failed to load '{abilityName}' from modern system: {ex.Message}. Falling back to legacy.");
            }
        }

        // Fall back to legacy AbilityLibrary
        if (AbilityLibrary.Instance != null &&
            AbilityLibrary.Instance.AbilityDictionary != null &&
            AbilityLibrary.Instance.AbilityDictionary.TryGetValue(abilityName, out var legacyAbility))
        {
            _legacyAbilitiesUsed.Add(abilityName);
            Debug.Log($"[AbilityProviderService] Loaded ability '{abilityName}' from legacy AbilityLibrary");
            return legacyAbility;
        }

        // Neither system has this ability
        Debug.LogError($"[AbilityProviderService] Ability '{abilityName}' not found in modern or legacy system!");
        return null;
    }

    /// <summary>
    /// Checks if an ability exists in either system.
    /// </summary>
    public bool HasAbility(AbilityName abilityName)
    {
        // Check modern system
        if (_registry != null && _registry.IsRegistered(abilityName))
        {
            return true;
        }

        // Check legacy system
        if (AbilityLibrary.Instance != null &&
            AbilityLibrary.Instance.AbilityDictionary != null &&
            AbilityLibrary.Instance.AbilityDictionary.ContainsKey(abilityName))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if an ability has been migrated to the modern IAbility system.
    /// </summary>
    public bool IsMigrated(AbilityName abilityName)
    {
        return _registry != null && _registry.IsRegistered(abilityName);
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
