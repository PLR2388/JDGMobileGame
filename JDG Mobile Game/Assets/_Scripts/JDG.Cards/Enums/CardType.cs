using System;
using System.Collections.Generic;
using JDG.Application.Services;
using JDG.Core;

namespace Cards
{

    /// <summary>
    /// Defines different types of cards.
    /// </summary>
    /// <remarks>
    /// DEPRECATED: Use JDG.Domain.Enums.CardType instead for clean architecture compatibility.
    /// This enum is kept for Unity ScriptableObject serialization - existing card assets reference these values.
    /// </remarks>
    [Obsolete("Use JDG.Domain.Enums.CardType for new code. This enum is kept for Unity serialization compatibility.")]
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
                // Phase 63: Removed fallback - LocalizationService is set by LegacySystemInitializer
                if (LocalizationService == null)
                {
                    throw new System.InvalidOperationException(
                        "CardTypeExtensions.LocalizationService is not set. " +
                        "Ensure LegacySystemInitializer.Initialize() is called before using card type extensions.");
                }
                return LocalizationService.GetLocalizedValue(localizationKey.ToString());
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(type), type, $"The provided CardType '{type}' does not have a corresponding localization key.");
            }
        }
    }
}