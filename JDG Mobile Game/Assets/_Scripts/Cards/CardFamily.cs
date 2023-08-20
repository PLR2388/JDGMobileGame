using System;

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
    
    // Define an extension method in a non-nested static class.
    public static class Extensions
    {
        public static string ToName(this CardFamily family)
        {
            switch (family)
            {
                case CardFamily.Comics:
                    return LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.FAMILY_COMICS);
                case CardFamily.Developer:
                    return LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.FAMILY_DEVELOPER);
                case CardFamily.Fistiland:
                    return LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.FAMILY_FISTILAND);
                case CardFamily.HardCorner:
                    return LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.FAMILY_HARD_CORNER);
                case CardFamily.Human:
                    return LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.FAMILY_HUMAN);
                case CardFamily.Incarnation:
                    return LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.FAMILY_INCARNATION);
                case CardFamily.Japan:
                    return LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.FAMILY_JAPAN);
                case CardFamily.Monster:
                    return LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.FAMILY_MONSTER);
                case CardFamily.Police:
                    return LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.FAMILY_POLICE);
                case CardFamily.Rpg:
                    return LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.FAMILY_RPG);
                case CardFamily.Spatial:
                    return LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.FAMILY_SPATIAL);
                case CardFamily.Wizard:
                    return LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.FAMILY_WIZARD);
                default:
                    throw new ArgumentOutOfRangeException(nameof(family), family, null);
            }
        }
    }
}