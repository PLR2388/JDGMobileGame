using System.Collections.Generic;
using System.Linq;
using Cards;
using JDG.Application.Services;
using UnityEngine;

namespace JDG.Infrastructure.Services
{
    /// <summary>
    /// Infrastructure implementation of ICardDataProvider.
    /// Phase 8: Created to replace ResourceSystem.Instance direct access.
    /// Phase 84: Now loads directly from Resources (ResourceSystem deleted).
    /// </summary>
    public class CardDataProvider : ICardDataProvider
    {
        private List<Card> _cards;
        private Dictionary<string, Card> _cardsDict;

        public CardDataProvider()
        {
            LoadCards();
        }

        private void LoadCards()
        {
            // Load directly from Resources folder
            _cards = Resources.LoadAll<Card>("Cards").ToList();

            if (_cards != null && _cards.Count > 0)
            {
                // Phase 158: Handle duplicate card titles gracefully instead of crashing
                // ToDictionary throws ArgumentException if duplicate keys exist
                _cardsDict = new Dictionary<string, Card>();
                foreach (var card in _cards)
                {
                    if (string.IsNullOrEmpty(card.Title))
                    {
                        Debug.LogWarning($"CardDataProvider: Card has null or empty Title, skipping dictionary entry");
                        continue;
                    }

                    if (_cardsDict.ContainsKey(card.Title))
                    {
                        Debug.LogWarning($"CardDataProvider: Duplicate card title '{card.Title}' found. Keeping first instance.");
                    }
                    else
                    {
                        _cardsDict[card.Title] = card;
                    }
                }
#if UNITY_EDITOR
                Debug.Log($"CardDataProvider: Loaded {_cards.Count} cards from Resources/Cards ({_cardsDict.Count} unique titles)");
#endif
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
