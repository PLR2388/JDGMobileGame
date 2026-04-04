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

        /// <summary>
        /// Marks the repository as initialized.
        /// Actual card loading is done by CardRepositoryInitializer (in default assembly).
        /// Note: Currently a no-op but kept for interface compliance and future use.
        /// </summary>
        public void Initialize()
        {
            // Intentionally empty - initialization is handled by CardRepositoryInitializer
        }

        public Card GetCard(CardId cardId)
        {
            return _cardInstances.TryGetValue(cardId, out var card) ? card : null;
        }

        public Card CreateCardInstance(string cardDefinitionName)
        {
            if (!_cardDefinitions.TryGetValue(cardDefinitionName, out var definition))
            {
                UnityEngine.Debug.LogWarning($"CardRepository.CreateCardInstance: Card definition '{cardDefinitionName}' not found. " +
                    $"Available definitions: {_cardDefinitions.Count}");
                return null;
            }

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
            var card = _cardDefinitions.Values.FirstOrDefault(c => c.Title == title);
            // Phase 147: Log warning when card not found to help diagnose issues
            if (card == null)
            {
                UnityEngine.Debug.LogWarning($"CardRepository: Card with title '{title}' not found");
            }
            return card;
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
