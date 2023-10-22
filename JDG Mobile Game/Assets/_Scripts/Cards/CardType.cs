using System;
using System.Collections.Generic;

namespace Cards
{
    
    /// <summary>
    /// Defines different types of cards.
    /// </summary>
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
    /// </summary>
    public static class CardTypeExtensions
    {
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
                return LocalizationSystem.Instance.GetLocalizedValue(localizationKey);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(type), type, $"The provided CardType '{type}' does not have a corresponding localization key.");
            }
        }
    }
}