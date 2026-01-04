using System;
using System.Collections.Generic;
using JDG.Application.Services;

namespace Cards
{
    /// <summary>
    /// Defines different card families/types.
    /// </summary>
    /// <remarks>
    /// DEPRECATED: Use JDG.Domain.Enums.CardFamily instead for clean architecture compatibility.
    /// This enum is kept for Unity ScriptableObject serialization - existing card assets reference these values.
    /// </remarks>
    [Obsolete("Use JDG.Domain.Enums.CardFamily for new code. This enum is kept for Unity serialization compatibility.")]
    public enum CardFamily
    {
        None,
        Comics,
        Developer,
        Fistiland,
        HardCorner,
        Human,
        Incarnation,
        Japan,
        Monster,
        Police,
        Rpg,
        Spatial,
        Wizard,
        Any
    }

    /// <summary>
    /// Provides extension methods related to the CardFamily enum.
    /// Phase 39: Uses ILocalizationService instead of LocalizationSystem.Instance.
    /// </summary>
    public static class CardFamilyExtensions
    {
        /// <summary>
        /// Shared ILocalizationService instance for CardFamily extensions.
        /// Phase 39: Set once at initialization to remove LocalizationSystem.Instance calls.
        /// </summary>
        public static ILocalizationService LocalizationService { get; set; }

        private static readonly Dictionary<CardFamily, LocalizationKeys> CardFamilyLocalizationMap = new Dictionary<CardFamily, LocalizationKeys>
        {
            { CardFamily.Comics, LocalizationKeys.FAMILY_COMICS },
            { CardFamily.Developer, LocalizationKeys.FAMILY_DEVELOPER },
            { CardFamily.Fistiland, LocalizationKeys.FAMILY_FISTILAND },
            { CardFamily.HardCorner, LocalizationKeys.FAMILY_HARD_CORNER },
            { CardFamily.Human, LocalizationKeys.FAMILY_HUMAN },
            { CardFamily.Incarnation, LocalizationKeys.FAMILY_INCARNATION },
            { CardFamily.Japan, LocalizationKeys.FAMILY_JAPAN },
            { CardFamily.Monster, LocalizationKeys.FAMILY_MONSTER },
            { CardFamily.Police, LocalizationKeys.FAMILY_POLICE },
            { CardFamily.Rpg, LocalizationKeys.FAMILY_RPG },
            { CardFamily.Spatial, LocalizationKeys.FAMILY_SPATIAL },
            { CardFamily.Wizard, LocalizationKeys.FAMILY_WIZARD }
        };

        public static string ToName(this CardFamily family)
        {
            if (CardFamilyLocalizationMap.TryGetValue(family, out var localizationKey))
            {
                // Phase 63: Removed fallback - LocalizationService is set by LegacySystemInitializer
                if (LocalizationService == null)
                {
                    throw new System.InvalidOperationException(
                        "CardFamilyExtensions.LocalizationService is not set. " +
                        "Ensure LegacySystemInitializer.Initialize() is called before using card family extensions.");
                }
                return LocalizationService.GetLocalizedValue(localizationKey.ToString());
            }

            throw new ArgumentOutOfRangeException(nameof(family), family, "Unmapped card family.");
        }
    }
}