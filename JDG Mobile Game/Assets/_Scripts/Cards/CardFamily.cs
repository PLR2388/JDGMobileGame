using System;
using System.Collections.Generic;

namespace Cards
{
    public enum CardFamily
    {
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

    public static class CardFamilyExtensions
    {
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
                return LocalizationSystem.Instance.GetLocalizedValue(localizationKey);
            }

            throw new ArgumentOutOfRangeException(nameof(family), family, "Unmapped card family.");
        }
    }
}