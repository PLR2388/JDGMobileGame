using System;

namespace Cards
{
    /// <summary>
    /// Represents the potential ownership status of a card, indicating whether it belongs to Player1, Player2, or has not been defined an owner.
    /// </summary>
    /// <remarks>
    /// DEPRECATED: Use JDG.Domain.CardOwner instead for clean architecture compatibility.
    /// This enum is kept for Unity ScriptableObject serialization - existing card assets reference these values.
    /// </remarks>
    [Obsolete("Use JDG.Domain.CardOwner for new code. This enum is kept for Unity serialization compatibility.")]
    public enum CardOwner
    {
        NotDefined,
        Player1,
        Player2
    }
}