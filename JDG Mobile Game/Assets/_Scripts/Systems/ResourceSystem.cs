using System;
using System.Collections.Generic;
using System.Linq;
using Cards;
using UnityEngine;

/// <summary>
/// Manages the resources related to the Card system, ensuring they're loaded and accessible.
/// Phase 59: Marked obsolete - use ICardDataProvider via dependency injection instead.
/// </summary>
[Obsolete("ResourceSystem singleton is obsolete. Use ICardDataProvider via dependency injection. Inject ICardDataProvider in your constructor or use [Inject] attribute for MonoBehaviours.")]
public class ResourceSystem : StaticInstance<ResourceSystem>
{
    /// <summary>
    /// A list of all loaded card resources.
    /// </summary>
    private List<Card> Cards { get; set; }
    
    /// <summary>
    /// A dictionary for fast lookup of cards by their title.
    /// </summary>
    private Dictionary<string, Card> cardsDict;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// Ensures base initialization is done and then assembles the card resources.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        AssembleResources();
    }

    /// <summary>
    /// Assembles the card resources, loading them from Unity's resource system and storing them in an easily accessible format.
    /// </summary>
    private void AssembleResources()
    {
        Cards = Resources.LoadAll<Card>("Cards").ToList();
        cardsDict = Cards.ToDictionary(card => card.Title, card => card);
    }

    /// <summary>
    /// Retrieves a card from the resources using its title.
    /// </summary>
    /// <param name="title">The title of the card.</param>
    /// <returns>The card with the given title or null if not found.</returns>
    public Card GetCardByName(string title) => cardsDict[title];
}
