using System;
using System.Collections.Generic;
using System.Linq;
using JDG.Application.Repositories;
using JDG.Domain.ValueObjects;
using UnityEngine;

namespace JDG.Infrastructure.Repositories
{
    /// <summary>
    /// Infrastructure implementation of IDeckRepository.
    /// Phase 89: Added PlayerPrefs persistence for saved decks.
    /// </summary>
    public class DeckRepository : IDeckRepository
    {
        private const string DeckListKey = "JDG_DeckList";
        private const string DeckPrefix = "JDG_Deck_";

        private readonly Dictionary<string, CardId[]> _decks = new Dictionary<string, CardId[]>();
        private readonly ICardRepository _cardRepository;

        public DeckRepository(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
            LoadDecks();
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

            // Phase 89: Persist to PlayerPrefs
            PersistDeck(deckName, cardIds);
            SaveDeckList();

#if UNITY_EDITOR
            Debug.Log($"DeckRepository: Saved deck '{deckName}' with {cardIds.Length} cards to PlayerPrefs");
#endif
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

                // Phase 89: Remove from PlayerPrefs
                PlayerPrefs.DeleteKey(DeckPrefix + deckName);
                SaveDeckList();

#if UNITY_EDITOR
                Debug.Log($"DeckRepository: Deleted deck '{deckName}' from PlayerPrefs");
#endif
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

#if UNITY_EDITOR
            Debug.Log($"DeckRepository: Created default deck for {playerId} with {deckCardIds.Count} cards");
#endif
            return deckCardIds.ToArray();
        }

        /// <summary>
        /// Loads deck configurations from PlayerPrefs.
        /// Phase 89: Implemented PlayerPrefs loading.
        /// </summary>
        public void LoadDecks()
        {
            _decks.Clear();

            var deckListJson = PlayerPrefs.GetString(DeckListKey, "");
            if (string.IsNullOrEmpty(deckListJson))
            {
#if UNITY_EDITOR
                Debug.Log("DeckRepository: No saved decks found in PlayerPrefs");
#endif
                return;
            }

            try
            {
                var deckList = JsonUtility.FromJson<DeckListData>(deckListJson);
                if (deckList?.DeckNames == null)
                {
#if UNITY_EDITOR
                    Debug.Log("DeckRepository: Empty deck list");
#endif
                    return;
                }

                foreach (var deckName in deckList.DeckNames)
                {
                    var deckJson = PlayerPrefs.GetString(DeckPrefix + deckName, "");
                    if (!string.IsNullOrEmpty(deckJson))
                    {
                        try
                        {
                            var deckData = JsonUtility.FromJson<DeckData>(deckJson);
                            if (deckData?.CardGuids != null)
                            {
                                // Phase 158: Parse each GUID individually to prevent one corrupted GUID
                                // from losing the entire deck (was: Select + ToArray with no per-card try-catch)
                                var cardIdsList = new List<CardId>();
                                foreach (var guidString in deckData.CardGuids)
                                {
                                    try
                                    {
                                        var cardId = CardId.FromGuid(System.Guid.Parse(guidString));
                                        cardIdsList.Add(cardId);
                                    }
                                    catch (Exception cardEx)
                                    {
                                        Debug.LogWarning($"DeckRepository: Skipping invalid card GUID '{guidString}' in deck '{deckName}': {cardEx.Message}");
                                        // Continue loading other cards instead of failing entire deck
                                    }
                                }
                                _decks[deckName] = cardIdsList.ToArray();
                                if (cardIdsList.Count < deckData.CardGuids.Length)
                                {
                                    Debug.LogWarning($"DeckRepository: Deck '{deckName}' loaded with {cardIdsList.Count}/{deckData.CardGuids.Length} cards (some GUIDs were invalid)");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning($"DeckRepository: Failed to load deck '{deckName}': {ex.Message}");
                        }
                    }
                }

#if UNITY_EDITOR
                Debug.Log($"DeckRepository: Loaded {_decks.Count} decks from PlayerPrefs");
#endif
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"DeckRepository: Failed to parse deck list: {ex.Message}");
            }
        }

        #region Private Persistence Methods

        private void PersistDeck(string deckName, CardId[] cardIds)
        {
            var deckData = new DeckData
            {
                CardGuids = cardIds.Select(id => id.ToGuid().ToString()).ToArray()
            };
            var json = JsonUtility.ToJson(deckData);
            PlayerPrefs.SetString(DeckPrefix + deckName, json);
        }

        private void SaveDeckList()
        {
            var deckList = new DeckListData
            {
                DeckNames = _decks.Keys.ToArray()
            };
            var json = JsonUtility.ToJson(deckList);
            PlayerPrefs.SetString(DeckListKey, json);
            PlayerPrefs.Save();
        }

        #endregion

        #region Serialization Data Classes

        /// <summary>
        /// Serializable container for the list of deck names.
        /// </summary>
        [Serializable]
        private class DeckListData
        {
            public string[] DeckNames;
        }

        /// <summary>
        /// Serializable container for deck card data.
        /// </summary>
        [Serializable]
        private class DeckData
        {
            public string[] CardGuids;
        }

        #endregion
    }
}
