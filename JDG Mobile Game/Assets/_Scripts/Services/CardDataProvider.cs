using System.Collections.Generic;
using System.Linq;
using Cards;
using JDG.Application.Services;
using UnityEngine;

namespace JDG.Infrastructure.Services
{
    /// <summary>
    /// Infrastructure implementation of ICardDataProvider.
    /// Wraps ResourceSystem singleton during migration.
    /// Phase 8: Eliminates ResourceSystem.Instance direct access.
    /// </summary>
    public class CardDataProvider : ICardDataProvider
    {
        private List<Card> _cards;
        private Dictionary<string, Card> _cardsDict;

        public CardDataProvider()
        {
            // Load cards from Resources folder
            LoadCards();
        }

        private void LoadCards()
        {
            // Try to get from ResourceSystem if it exists
            if (ResourceSystem.Instance != null)
            {
                // Access Cards via reflection to maintain compatibility
                var property = typeof(ResourceSystem).GetProperty("Cards",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                _cards = (List<Card>)property?.GetValue(ResourceSystem.Instance);

                if (_cards != null && _cards.Count > 0)
                {
                    _cardsDict = _cards.ToDictionary(card => card.Title, card => card);
                    return;
                }
            }

            // Fallback: Load directly from Resources
            _cards = Resources.LoadAll<Card>("Cards").ToList();
            if (_cards != null && _cards.Count > 0)
            {
                _cardsDict = _cards.ToDictionary(card => card.Title, card => card);
            }
            else
            {
                Debug.LogWarning("CardDataProvider: No cards found in Resources/Cards");
                _cards = new List<Card>();
                _cardsDict = new Dictionary<string, Card>();
            }
        }

        public bool IsLoaded => _cards != null && _cards.Count > 0;

        public List<object> GetAllCards()
        {
            return _cards?.Cast<object>().ToList() ?? new List<object>();
        }

        public object GetCardByName(string title)
        {
            if (_cardsDict != null && _cardsDict.TryGetValue(title, out var card))
            {
                return card;
            }
            return null;
        }

        /// <summary>
        /// Gets all cards as strongly-typed Card objects.
        /// Internal method for services that need Card type directly.
        /// </summary>
        internal List<Card> GetAllCardsTyped()
        {
            return _cards ?? new List<Card>();
        }
    }
}
