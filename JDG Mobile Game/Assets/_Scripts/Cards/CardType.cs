using System;
using System.Collections.Generic;
using JDG.Application.Services;

namespace Cards
{

    /// <summary>
    /// Defines different types of cards.
    /// </summary>
    /// <remarks>
    /// DEPRECATED: Use JDG.Domain.Enums.CardType instead for clean architecture compatibility.
    /// This enum will be removed in a future version.
    /// </remarks>
    [Obsolete("Use JDG.Domain.Enums.CardType instead. This enum will be removed in Phase 54.")]
    public enum CardType
    {
        Contre,
        Effect,
        Equipment,
        Field,
        Invocation
    }

    /// <summary>
    /// Provides extension methods related to the CardType enum.
    /// Phase 39: Uses ILocalizationService instead of LocalizationSystem.Instance.
    /// </summary>
    public static class CardTypeExtensions
    {
        /// <summary>
        /// Shared ILocalizationService instance for CardType extensions.
        /// Phase 39: Set once at initialization to remove LocalizationSystem.Instance calls.
        /// </summary>
        public static ILocalizationService LocalizationService { get; set; }

        private static readonly Dictionary<CardType, LocalizationKeys> CardTypeLocalizationKeyMap = new Dictionary<CardType, LocalizationKeys>
        {
            {CardType.Contre, LocalizationKeys.TYPE_CONTRE},
            {CardType.Effect, LocalizationKeys.TYPE_EFFECT},
            {CardType.Equipment, LocalizationKeys.TYPE_EQUIPMENT},
            {CardType.Field, LocalizationKeys.TYPE_FIELD},
            {CardType.Invocation, LocalizationKeys.TYPE_INVOCATION}
        };

        public static string ToName(this CardType type)
        {
            if (CardTypeLocalizationKeyMap.TryGetValue(type, out var localizationKey))
            {
                // Phase 39: Use injected ILocalizationService instead of LocalizationSystem.Instance
                if (LocalizationService != null)
                {
                    return LocalizationService.GetLocalizedValue(localizationKey.ToString());
                }
                // Fallback to singleton if service not initialized (during startup)
                return LocalizationSystem.Instance?.GetLocalizedValue(localizationKey) ?? localizationKey.ToString();
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(type), type, $"The provided CardType '{type}' does not have a corresponding localization key.");
            }
        }
    }
}