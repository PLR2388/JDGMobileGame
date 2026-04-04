using Cards;
using JDG.Application.Cards;
using JDG.Application.Services;
using JDG.Infrastructure.Cards;
using UnityEngine;

/// <summary>
/// Service for resolving card visual data (Materials).
/// Phase 39: Created to abstract Unity Material dependencies from presenters.
///
/// Note: This service is in the default assembly (not JDG.Infrastructure) because
/// it needs to access the concrete InGameCard class which lives in the Cards namespace.
/// This follows the same pattern as RaycastService, PlayerService, etc.
/// </summary>
public class CardVisualService : ICardVisualService
{
    /// <summary>
    /// Gets the visual material for a card.
    /// </summary>
    /// <param name="card">The card to get visual for</param>
    /// <returns>The Material object (UnityEngine.Material), or null if not found</returns>
    public object GetMaterial(IInGameCard card)
    {
        if (card == null)
        {
            return null;
        }

        // If the card is an InGameCard, directly access MaterialCard
        if (card is InGameCard inGameCard)
        {
            return inGameCard.MaterialCard;
        }

        // Fallback: try to find material by visual ID
        return GetMaterialByVisualId(card.VisualId);
    }

    /// <summary>
    /// Gets the visual material by visual ID.
    /// The visual ID is typically the card title.
    /// </summary>
    /// <param name="visualId">The visual identifier (card title)</param>
    /// <returns>The Material object, or null if not found</returns>
    public object GetMaterialByVisualId(string visualId)
    {
        if (string.IsNullOrEmpty(visualId))
        {
            return null;
        }

        // Try to load material from Resources
        // Cards are typically stored as Materials with the card title as name
        var material = Resources.Load<Material>($"Materials/Cards/{visualId}");
        return material;
    }

    /// <summary>
    /// Checks if a visual exists for the given card.
    /// </summary>
    /// <param name="card">The card to check</param>
    /// <returns>True if visual exists, false otherwise</returns>
    public bool HasVisual(IInGameCard card)
    {
        return GetMaterial(card) != null;
    }
}
