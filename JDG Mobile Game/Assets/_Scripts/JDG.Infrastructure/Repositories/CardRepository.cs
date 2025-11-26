using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;

namespace JDG.Infrastructure.Repositories
{
    /// <summary>
    /// Infrastructure implementation of ICardRepository.
    /// Bridges domain Card entities to Unity ScriptableObject card definitions.
    /// Maintains a registry of card instances by CardId.
    /// </summary>
    public class CardRepository : ICardRepository
    {
        private readonly Dictionary<CardId, Card> _cardInstances = new Dictionary<CardId, Card>();
        private readonly Dictionary<string, Card> _cardDefinitions = new Dictionary<string, Card>();
        private bool _isInitialized;

        /// <summary>
        /// Initializes the repository by loading all card ScriptableObjects.
        /// Should be called once at game startup.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
                return;

            // Load all card ScriptableObjects from Resources
            // In a real implementation, this would load from your asset database
            LoadCardDefinitions();

            _isInitialized = true;
        }

        public Card GetCard(CardId cardId)
        {
            return _cardInstances.ContainsKey(cardId) ? _cardInstances[cardId] : null;
        }

        public Card CreateCardInstance(string cardDefinitionName)
        {
            if (!_cardDefinitions.ContainsKey(cardDefinitionName))
                return null;

            // Get the card definition (template)
            var definition = _cardDefinitions[cardDefinitionName];

            // Create a new instance with a unique CardId
            Card newInstance = CreateCardCopy(definition);

            // Register the instance
            _cardInstances[newInstance.Id] = newInstance;

            return newInstance;
        }

        public IEnumerable<Card> GetAllCardDefinitions()
        {
            return _cardDefinitions.Values;
        }

        public IEnumerable<Card> GetCardsByType(CardType type)
        {
            return _cardDefinitions.Values.Where(c => c.Type == type);
        }

        public IEnumerable<Card> GetCardsByFamily(CardFamily family)
        {
            return _cardDefinitions.Values.Where(c => c.HasFamily(family));
        }

        public Card GetCardByTitle(string title)
        {
            return _cardDefinitions.Values.FirstOrDefault(c => c.Title == title);
        }

        /// <summary>
        /// Loads card definitions from ScriptableObjects in Resources/Cards.
        /// Converts all old ScriptableObject cards to domain Card entities.
        /// </summary>
        private void LoadCardDefinitions()
        {
            int loadedCount = 0;

            // Load all cards from Resources/Cards folder
            // Unity's Resources.LoadAll loads from any Resources folder in the project
            var allScriptableCards = Resources.LoadAll<Cards.Card>("Cards");

            foreach (var scriptableCard in allScriptableCards)
            {
                try
                {
                    // Convert ScriptableObject to domain Card entity
                    var domainCard = CardConverter.ConvertToDomain(scriptableCard);

                    if (domainCard != null)
                    {
                        _cardDefinitions[domainCard.Title] = domainCard;
                        loadedCount++;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to convert card '{scriptableCard.Title}': {ex.Message}");
                }
            }

            Debug.Log($"CardRepository: Loaded {loadedCount} card definitions from ScriptableObjects");
        }

        /// <summary>
        /// Creates a copy of a card with a new unique CardId.
        /// </summary>
        private Card CreateCardCopy(Card original)
        {
            var newId = CardId.New();

            switch (original.Type)
            {
                case CardType.Invocation:
                    return Card.CreateInvocation(
                        newId,
                        original.Title,
                        original.Description,
                        original.DetailedDescription,
                        original.Stats?.Attack ?? 0,
                        original.Stats?.Defense ?? 0,
                        original.Families,
                        original.AffectedByEffect,
                        original.Conditions,
                        original.Abilities,
                        original.IsCollector
                    );

                case CardType.Equipment:
                    return Card.CreateEquipment(
                        newId,
                        original.Title,
                        original.Description,
                        original.DetailedDescription,
                        original.EquipmentAbilities,
                        original.IsCollector
                    );

                case CardType.Field:
                    return Card.CreateField(
                        newId,
                        original.Title,
                        original.Description,
                        original.DetailedDescription,
                        original.FieldFamily ?? CardFamily.Any,
                        original.FieldAbilities,
                        original.IsCollector
                    );

                case CardType.Effect:
                    return Card.CreateEffect(
                        newId,
                        original.Title,
                        original.Description,
                        original.DetailedDescription,
                        original.EffectAbilities,
                        original.IsCollector
                    );

                case CardType.Contre:
                    return Card.CreateContre(
                        newId,
                        original.Title,
                        original.Description,
                        original.DetailedDescription,
                        original.IsCollector
                    );

                default:
                    throw new ArgumentException($"Unknown card type: {original.Type}");
            }
        }

        /// <summary>
        /// Registers a card instance (useful for testing or manual card creation).
        /// </summary>
        public void RegisterCardInstance(Card card)
        {
            if (card != null)
            {
                _cardInstances[card.Id] = card;
            }
        }

        /// <summary>
        /// Registers a card definition (useful for testing or manual setup).
        /// </summary>
        public void RegisterCardDefinition(Card card)
        {
            if (card != null)
            {
                _cardDefinitions[card.Title] = card;
            }
        }
    }
}
