using Cards;
using UnityEngine;

/// <summary>
/// Service for managing pooled card GameObjects.
/// Replaces CardPoolManager singleton access.
/// Part of Phase 9 - Remaining singleton elimination.
/// Phase 140: Added AddCardToPool for player entity registration.
///
/// Note: This interface is in the default assembly because it references legacy types
/// (InGameCard). Will be migrated once InGameCard is replaced with domain Card entity.
/// </summary>
public interface ICardPoolService
{
    /// <summary>
    /// Gets the transform where pooled cards are stored.
    /// </summary>
    Transform CardPoolHolder { get; }

    /// <summary>
    /// Retrieves a pooled card GameObject matching the provided InGameCard.
    /// </summary>
    /// <param name="inGameCard">The card to find in the pool</param>
    /// <returns>GameObject if found, null otherwise</returns>
    GameObject GetPooledObject(InGameCard inGameCard);

    /// <summary>
    /// Adds a card to the pool with a visual representation.
    /// Phase 140: Added for player entity registration.
    /// </summary>
    /// <param name="inGameCard">The card to add to the pool</param>
    void AddCardToPool(InGameCard inGameCard);
}
