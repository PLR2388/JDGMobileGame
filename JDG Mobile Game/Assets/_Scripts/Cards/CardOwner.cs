using System;

/// <summary>
/// Represents the potential ownership status of a card, indicating whether it belongs to Player1, Player2, or has not been defined an owner.
/// </summary>
/// <remarks>
/// DEPRECATED: Use JDG.Domain.CardOwner instead for clean architecture compatibility.
/// This enum will be removed in a future version.
/// </remarks>
[Obsolete("Use JDG.Domain.CardOwner instead. This enum will be removed in Phase 54.")]
public enum CardOwner
{
    NotDefined,
    Player1,
    Player2
}