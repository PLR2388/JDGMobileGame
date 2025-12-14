using System.Collections.Generic;
using JDG.Application.Abilities;
using JDG.Domain;
using UnityEngine;

namespace JDG.Infrastructure.Bridge
{
    /// <summary>
    /// Service that manages the migration from legacy Ability system to new IAbility system.
    /// Routes ability execution to the appropriate system based on migration status.
    ///
    /// Phase 7.1: Part of ability migration infrastructure.
    ///
    /// Usage Pattern:
    /// 1. Register migrated abilities with RegisterMigratedAbility()
    /// 2. Execute abilities through ExecuteAbility() which routes appropriately
    /// 3. For abilities requiring legacy context, use ExecuteLegacy()
    ///
    /// This service tracks:
    /// - Which abilities have been migrated to IAbility
    /// - Which abilities still require legacy execution
    /// - Migration progress statistics
    /// </summary>
    public class AbilityMigrationService
    {
        private readonly AbilityRegistry _abilityRegistry;
        private readonly HashSet<AbilityName> _migratedAbilities;
        private readonly Dictionary<AbilityName, LegacyAbilityAdapter> _legacyAdapters;

        public AbilityMigrationService(AbilityRegistry abilityRegistry)
        {
            _abilityRegistry = abilityRegistry;
            _migratedAbilities = new HashSet<AbilityName>();
            _legacyAdapters = new Dictionary<AbilityName, LegacyAbilityAdapter>();
        }

        /// <summary>
        /// Marks an ability as migrated to the new system.
        /// After calling this, ExecuteAbility will use the new IAbility implementation.
        /// </summary>
        public void RegisterMigratedAbility(AbilityName abilityName)
        {
            _migratedAbilities.Add(abilityName);
        }

        /// <summary>
        /// Registers a legacy ability adapter for abilities not yet fully migrated.
        /// </summary>
#pragma warning disable CS0618 // Type or member is obsolete
        public void RegisterLegacyAbility(Ability legacyAbility)
        {
            var adapter = new LegacyAbilityAdapter(legacyAbility);
            _legacyAdapters[legacyAbility.Name] = adapter;
        }
#pragma warning restore CS0618

        /// <summary>
        /// Checks if an ability has been migrated to the new system.
        /// </summary>
        public bool IsMigrated(AbilityName abilityName)
        {
            return _migratedAbilities.Contains(abilityName);
        }

        /// <summary>
        /// Executes an ability, routing to the appropriate system.
        /// - If migrated: Uses new IAbility from AbilityRegistry
        /// - If not migrated: Returns result indicating legacy execution needed
        /// </summary>
        public AbilityResult ExecuteAbility(AbilityName abilityName, AbilityContext context)
        {
            // Check if we have a modern implementation
            if (IsMigrated(abilityName))
            {
                var ability = _abilityRegistry.GetAbility(abilityName);
                if (ability != null && ability.CanActivate(context))
                {
                    return ability.Execute(context);
                }
                return AbilityResult.Failure($"Ability '{abilityName}' not found or cannot activate");
            }

            // Check if we have a legacy adapter
            if (_legacyAdapters.TryGetValue(abilityName, out var adapter))
            {
                return adapter.Execute(context);
            }

            // No implementation available
            return AbilityResult.Failure($"Ability '{abilityName}' has no implementation");
        }

        /// <summary>
        /// Gets the ability implementation, preferring modern over legacy.
        /// </summary>
        public IAbility GetAbility(AbilityName abilityName)
        {
            if (IsMigrated(abilityName))
            {
                return _abilityRegistry.GetAbility(abilityName);
            }

            if (_legacyAdapters.TryGetValue(abilityName, out var adapter))
            {
                return adapter;
            }

            return null;
        }

        /// <summary>
        /// Executes a legacy ability with full Unity context.
        /// Use this when AbilityResult.RequiresLegacyExecution is true.
        /// </summary>
#pragma warning disable CS0618 // Type or member is obsolete
        public void ExecuteLegacy(
            AbilityName abilityName,
            Transform canvas,
            PlayerCards playerCards,
            PlayerCards opponentPlayerCards)
        {
            if (_legacyAdapters.TryGetValue(abilityName, out var adapter))
            {
                var legacyAbility = adapter.GetLegacyAbility();
                legacyAbility.ApplyEffect(canvas, playerCards, opponentPlayerCards);
            }
            else
            {
                Debug.LogWarning($"No legacy adapter found for ability '{abilityName}'");
            }
        }
#pragma warning restore CS0618

        /// <summary>
        /// Gets migration statistics.
        /// </summary>
        public MigrationStats GetStats()
        {
            return new MigrationStats
            {
                TotalLegacyAbilities = _legacyAdapters.Count,
                MigratedAbilities = _migratedAbilities.Count,
                PendingMigration = _legacyAdapters.Count - _migratedAbilities.Count
            };
        }

        /// <summary>
        /// Gets a list of all abilities that still need migration.
        /// </summary>
        public IEnumerable<AbilityName> GetPendingMigrations()
        {
            foreach (var kvp in _legacyAdapters)
            {
                if (!_migratedAbilities.Contains(kvp.Key))
                {
                    yield return kvp.Key;
                }
            }
        }
    }

    /// <summary>
    /// Statistics about ability migration progress.
    /// </summary>
    public class MigrationStats
    {
        public int TotalLegacyAbilities { get; set; }
        public int MigratedAbilities { get; set; }
        public int PendingMigration { get; set; }

        public float MigrationPercentage =>
            TotalLegacyAbilities > 0
                ? (float)MigratedAbilities / TotalLegacyAbilities * 100
                : 100f;

        public override string ToString()
        {
            return $"Migration Progress: {MigratedAbilities}/{TotalLegacyAbilities} ({MigrationPercentage:F1}%)";
        }
    }
}
