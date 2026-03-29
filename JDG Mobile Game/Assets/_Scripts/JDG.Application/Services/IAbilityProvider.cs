using JDG.Application.Abilities;
using JDG.Domain;

namespace JDG.Application.Services
{
    /// <summary>
    /// Provides abilities for card initialization.
    /// This interface abstracts the ability lookup mechanism for the modern AbilityRegistry.
    ///
    /// Phase 7: Part of AbilityLibrary → AbilityRegistry migration.
    /// Phase 105: Added GetModernAbility for direct IAbility access.
    /// Phase 118: Removed legacy GetAbility method, uses only modern IAbility.
    /// </summary>
    public interface IAbilityProvider
    {
        /// <summary>
        /// Gets a modern IAbility implementation by name.
        /// Phase 118: This is now the only ability lookup method.
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
    }
}
