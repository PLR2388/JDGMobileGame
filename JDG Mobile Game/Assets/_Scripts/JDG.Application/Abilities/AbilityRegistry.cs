using System;
using System.Collections.Generic;
using JDG.Domain;

namespace JDG.Application.Abilities
{
    /// <summary>
    /// Registry that maps AbilityName to IAbility instances.
    /// Acts as a centralized lookup for all game abilities.
    /// </summary>
    public class AbilityRegistry
    {
        private readonly Dictionary<AbilityName, Func<IAbility>> _abilityFactories;
        private readonly Dictionary<AbilityName, IAbility> _cachedAbilities;

        public AbilityRegistry()
        {
            _abilityFactories = new Dictionary<AbilityName, Func<IAbility>>();
            _cachedAbilities = new Dictionary<AbilityName, IAbility>();
        }

        /// <summary>
        /// Registers an ability factory for a given ability name.
        /// </summary>
        /// <param name="abilityName">The unique ability name</param>
        /// <param name="factory">Factory function that creates the ability instance</param>
        public void Register(AbilityName abilityName, Func<IAbility> factory)
        {
            if (_abilityFactories.ContainsKey(abilityName))
            {
                throw new InvalidOperationException($"Ability '{abilityName}' is already registered");
            }

            _abilityFactories[abilityName] = factory;
        }

        /// <summary>
        /// Gets an ability instance by name.
        /// Abilities are cached after first creation.
        /// </summary>
        /// <param name="abilityName">The ability to retrieve</param>
        /// <returns>The ability instance</returns>
        /// <exception cref="KeyNotFoundException">If ability is not registered</exception>
        public IAbility GetAbility(AbilityName abilityName)
        {
            // Check cache first
            if (_cachedAbilities.TryGetValue(abilityName, out var cachedAbility))
            {
                return cachedAbility;
            }

            // Create new instance from factory
            if (!_abilityFactories.TryGetValue(abilityName, out var factory))
            {
                throw new KeyNotFoundException($"Ability '{abilityName}' is not registered");
            }

            var ability = factory();
            _cachedAbilities[abilityName] = ability;
            return ability;
        }

        /// <summary>
        /// Checks if an ability is registered.
        /// </summary>
        public bool IsRegistered(AbilityName abilityName)
        {
            return _abilityFactories.ContainsKey(abilityName);
        }

        /// <summary>
        /// Gets all registered ability names.
        /// </summary>
        public IEnumerable<AbilityName> GetRegisteredAbilities()
        {
            return _abilityFactories.Keys;
        }

        /// <summary>
        /// Clears the ability cache (useful for testing).
        /// </summary>
        public void ClearCache()
        {
            _cachedAbilities.Clear();
        }

        /// <summary>
        /// Unregisters an ability (useful for testing).
        /// </summary>
        public void Unregister(AbilityName abilityName)
        {
            _abilityFactories.Remove(abilityName);
            _cachedAbilities.Remove(abilityName);
        }
    }
}
