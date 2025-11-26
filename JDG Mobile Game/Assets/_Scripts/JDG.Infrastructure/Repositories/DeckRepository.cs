using System.Collections.Generic;
using System.Linq;
using JDG.Application.Repositories;
using JDG.Domain.ValueObjects;
using UnityEngine;

namespace JDG.Infrastructure.Repositories
{
    /// <summary>
    /// Infrastructure implementation of IDeckRepository.
    /// Manages deck configurations (could be extended to load from files/PlayerPrefs).
    /// </summary>
    public class DeckRepository : IDeckRepository
    {
        private readonly Dictionary<string, CardId[]> _decks = new Dictionary<string, CardId[]>();
        private readonly ICardRepository _cardRepository;

        public DeckRepository(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public CardId[] GetDeck(string deckName)
        {
            return _decks.ContainsKey(deckName) ? _decks[deckName] : null;
        }

        public void SaveDeck(string deckName, CardId[] cardIds)
        {
            if (string.IsNullOrEmpty(deckName) || cardIds == null || cardIds.Length == 0)
                return;

            _decks[deckName] = cardIds;

            // TODO: Persist to PlayerPrefs or file system
            Debug.Log($"DeckRepository: Saved deck '{deckName}' with {cardIds.Length} cards");
        }

        public IEnumerable<string> GetAllDeckNames()
        {
            return _decks.Keys;
        }

        public void DeleteDeck(string deckName)
        {
            if (_decks.ContainsKey(deckName))
            {
                _decks.Remove(deckName);
                Debug.Log($"DeckRepository: Deleted deck '{deckName}'");
            }
        }

        public CardId[] GetDefaultDeck(PlayerId playerId)
        {
            // For now, create a default deck by taking the first 40 cards
            // In a real implementation, this would load from a configuration file or ScriptableObject

            var availableCards = _cardRepository.GetAllCardDefinitions().Take(40).ToList();

            if (availableCards.Count == 0)
            {
                Debug.LogWarning($"DeckRepository: No cards available for default deck");
                return new CardId[0];
            }

            // Create instances of each card
            var deckCardIds = new List<CardId>();
            foreach (var cardDefinition in availableCards)
            {
                var instance = _cardRepository.CreateCardInstance(cardDefinition.Title);
                if (instance != null)
                {
                    deckCardIds.Add(instance.Id);
                }
            }

            Debug.Log($"DeckRepository: Created default deck for {playerId} with {deckCardIds.Count} cards");
            return deckCardIds.ToArray();
        }

        /// <summary>
        /// Loads deck configurations from persistent storage.
        /// TODO: Implement loading from PlayerPrefs or file system.
        /// </summary>
        public void LoadDecks()
        {
            // Placeholder for loading saved decks from PlayerPrefs or files
            Debug.Log("DeckRepository: LoadDecks called - implement deck loading");
        }
    }
}
