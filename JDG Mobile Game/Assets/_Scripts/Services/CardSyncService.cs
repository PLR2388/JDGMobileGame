using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using JDG.Application.Cards;
using JDG.Application.Services;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;
using DomainCardFamily = JDG.Domain.Enums.CardFamily;
using LegacyCardFamily = Cards.CardFamily;

namespace Services
{
    /// <summary>
    /// Service for synchronizing domain Card state changes back to InGameInvocationCard.
    ///
    /// Phase 141: Fixes the equipment ability sync issue where domain Card modifications
    /// were lost because ConvertToCard() created a disconnected temporary Card.
    ///
    /// This service maintains a mapping between domain Cards and their originating
    /// InGameInvocationCards, enabling state to be synced back after ability execution.
    /// </summary>
    public class CardSyncService : ICardSyncService
    {
        /// <summary>
        /// Maps domain Card.Id to the originating InGameInvocationCard.
        /// </summary>
        private readonly Dictionary<CardId, InGameInvocationCard> _cardMappings =
            new Dictionary<CardId, InGameInvocationCard>();

        /// <summary>
        /// Creates a domain Card linked to an InGameInvocationCard for ability execution.
        /// Uses Card.CreateInvocation with current stats (unlike old ConvertToCard which used CreateEffect).
        /// </summary>
        public Card CreateLinkedCard(IInGameInvocationCard inGameCard)
        {
            if (inGameCard is not InGameInvocationCard concreteCard)
                return null;

            // Convert legacy CardFamily[] to domain CardFamily enumerable
            var domainFamilies = ConvertFamiliesToDomain(concreteCard.Families);

            // Create domain Card using CreateInvocation with CURRENT stats (not base stats)
            // This is the key fix - old code used CreateEffect which has no stats
            var domainCard = Card.CreateInvocation(
                id: CardId.New(),
                title: concreteCard.Title,
                description: concreteCard.GetDescription() ?? "",
                detailedDescription: concreteCard.GetDetailedDescription() ?? "",
                attack: (int)concreteCard.Attack,
                defense: (int)concreteCard.Defense,
                families: domainFamilies,
                affectedByEffect: concreteCard.IsAffectedByEffectCard,
                conditions: null, // Not needed for ability context
                abilities: null,  // Not needed for ability context
                isCollector: concreteCard.Collector
            );

            // Copy current runtime state to domain Card
            if (concreteCard.CancelEffect)
                domainCard.SetCancelEffect(true);
            if (concreteCard.CanDirectAttack)
                domainCard.EnableDirectAttack();
            if (concreteCard.CantBeAttack)
                domainCard.SetCantBeAttacked(true);

            // Store mapping for later sync
            _cardMappings[domainCard.Id] = concreteCard;

            return domainCard;
        }

        /// <summary>
        /// Syncs all state changes from the domain Card back to its linked InGameInvocationCard.
        /// </summary>
        public void SyncCardState(Card domainCard)
        {
            if (domainCard == null)
                return;

            if (!_cardMappings.TryGetValue(domainCard.Id, out var inGameCard))
                return;

            // Sync stats (domain Card uses int, InGameInvocationCard uses float)
            if (domainCard.Stats.HasValue)
            {
                inGameCard.Attack = domainCard.Stats.Value.Attack;
                inGameCard.Defense = domainCard.Stats.Value.Defense;
            }

            // Sync boolean state flags
            // Note: CancelEffect setter publishes an event, so only set if changed
            if (inGameCard.CancelEffect != domainCard.CancelEffect)
            {
                inGameCard.CancelEffect = domainCard.CancelEffect;
            }

            inGameCard.CanDirectAttack = domainCard.CanDirectAttack;

            // Note naming difference: CantBeAttack (legacy) vs CantBeAttacked (domain)
            inGameCard.CantBeAttack = domainCard.CantBeAttacked;

            // Sync families if they were changed
            if (domainCard.Families.Count > 0)
            {
                inGameCard.Families = ConvertFamiliesToLegacy(domainCard.Families);
            }
        }

        /// <summary>
        /// Clears the mapping for a specific card to prevent memory leaks.
        /// </summary>
        public void ClearMapping(Card domainCard)
        {
            if (domainCard != null)
            {
                _cardMappings.Remove(domainCard.Id);
            }
        }

        #region Helper Methods

        /// <summary>
        /// Converts legacy CardFamily[] to domain CardFamily enumerable.
        /// Both enums have the same values, allowing safe int casting.
        /// </summary>
        private static IEnumerable<DomainCardFamily> ConvertFamiliesToDomain(LegacyCardFamily[] families)
        {
            if (families == null)
                return Enumerable.Empty<DomainCardFamily>();

            return families.Select(f => (DomainCardFamily)(int)f);
        }

        /// <summary>
        /// Converts domain CardFamily list to legacy CardFamily array.
        /// Both enums have the same values, allowing safe int casting.
        /// </summary>
        private static LegacyCardFamily[] ConvertFamiliesToLegacy(IReadOnlyList<DomainCardFamily> families)
        {
            if (families == null || families.Count == 0)
                return System.Array.Empty<LegacyCardFamily>();

            return families.Select(f => (LegacyCardFamily)(int)f).ToArray();
        }

        #endregion
    }
}
