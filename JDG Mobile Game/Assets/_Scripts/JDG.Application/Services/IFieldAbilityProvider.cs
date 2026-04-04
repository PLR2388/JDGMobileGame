using JDG.Application.Abilities;
using DomainFieldAbilityName = JDG.Domain.Enums.FieldAbilityName;

namespace JDG.Application.Services
{
    /// <summary>
    /// Provides field abilities by name for InGameFieldCard.
    /// Phase 48: Abstracts FieldAbilityLibrary.Instance access for DI.
    /// Phase 104: Added GetModernAbility for IAbility migration.
    /// Phase 116: Removed legacy GetAbility - now uses only modern IAbility.
    /// </summary>
    public interface IFieldAbilityProvider
    {
        /// <summary>
        /// Gets a modern IAbility implementation by field ability name.
        /// Phase 116: Now the primary (and only) lookup method.
        /// </summary>
        /// <param name="abilityName">The domain ability name to look up.</param>
        /// <returns>The modern ability, or null if not found.</returns>
        IAbility GetModernAbility(DomainFieldAbilityName abilityName);

        /// <summary>
        /// Checks if an ability exists for the given name.
        /// </summary>
        /// <param name="abilityName">The ability name to check.</param>
        /// <returns>True if the ability exists.</returns>
        bool HasAbility(DomainFieldAbilityName abilityName);
    }
}
