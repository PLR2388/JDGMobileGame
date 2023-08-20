using System;

namespace Cards
{
    public enum CardType
    {
        Contre,
        Effect,
        Equipment,
        Field,
        Invocation
    }

    public static class CardTypeExtensions
    {
        public static string ToName(this CardType type)
        {
            switch (type)
            {
                case CardType.Contre:
                    return LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.TYPE_CONTRE);
                case CardType.Effect:
                    return LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.TYPE_EFFECT);
                case CardType.Equipment:
                    return LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.TYPE_EQUIPMENT);
                case CardType.Field:
                    return LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.TYPE_FIELD);
                case CardType.Invocation:
                    return LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.TYPE_INVOCATION);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}