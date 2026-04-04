using JDG.Application.Services;

/// <summary>
/// Extension methods for ILocalizationService to support LocalizationKeys enum.
/// This bridges the clean architecture interface with the legacy enum-based system.
/// Phase 34: LocalizationSystem Singleton Elimination.
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
