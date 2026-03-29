using JDG.Application.Services;
using JDG.Core;

namespace JDG.Infrastructure.Services
{
    /// <summary>
    /// Extension methods for ILocalizationService to support LocalizationKeys enum.
    /// Phase 166: Moved to JDG.Infrastructure to support CardHandler migration.
    /// Bridges the clean architecture interface with the enum-based localization system.
    /// </summary>
    public static class LocalizationServiceExtensions
    {
        /// <summary>
        /// Gets the localized text value for a LocalizationKeys enum value.
        /// </summary>
        /// <param name="service">The localization service.</param>
        /// <param name="key">The localization key enum value.</param>
        /// <returns>The localized string, or the key name in brackets if not found.</returns>
        public static string GetLocalizedValue(this ILocalizationService service, LocalizationKeys key)
        {
            return service.GetLocalizedValue(key.ToString());
        }
    }
}
