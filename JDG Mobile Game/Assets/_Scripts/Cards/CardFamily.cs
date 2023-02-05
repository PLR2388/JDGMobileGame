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
                    return "Comics";
                case CardFamily.Developer:
                    return "Developpeur";
                case CardFamily.Fistiland:
                    return "Fistiland";
                case CardFamily.HardCorner:
                    return "HandCorner";
                case CardFamily.Human:
                    return "Humain";
                case CardFamily.Incarnation:
                    return "Incarnation";
                case CardFamily.Japan:
                    return "Japon";
                case CardFamily.Monster:
                    return "Monstre";
                case CardFamily.Police:
                    return "Police";
                case CardFamily.Rpg:
                    return "Rpg";
                case CardFamily.Spatial:
                    return "Spatial";
                case CardFamily.Wizard:
                    return "Sorcier";
                default:
                    throw new ArgumentOutOfRangeException(nameof(family), family, null);
            }
        }
    }
}