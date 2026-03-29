using System.Collections.Generic;
using System.Linq;
using JDG.Application.Cards;
using JDG.Application.Repositories;
using JDG.Application.Services;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Cards;
using DomainCardFamily = JDG.Domain.Enums.CardFamily;
using LegacyCardFamily = Cards.CardFamily;

namespace Services
{
    /// <summary>
    /// Service for synchronizing domain Card state changes back to InGameInvocationCard.
    ///
    /// Phase 141: Fixes the equipment ability sync issue where domain Card modifications
    /// were lost because ConvertToCard() created a disconnected temporary Card.
    /// Phase 143: Added bulk sync for modern abilities that modify cards via IPlayerRepository.
    ///
    /// This service maintains a mapping between domain Cards and their originating
    /// InGameInvocationCards, enabling state to be synced back after ability execution.
    /// </summary>
    public class CardSyncService : ICardSyncService
    {
        private readonly IPlayerRepository _playerRepository;

        /// <summary>
        /// Maps domain Card.Id to the originating InGameInvocationCard.
        /// </summary>
        private readonly Dictionary<CardId, InGameInvocationCard> _cardMappings =
            new Dictionary<CardId, InGameInvocationCard>();

        public CardSyncService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

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
            // Phase 159: Pass float stats directly - no more int truncation
            var domainCard = Card.CreateInvocation(
                id: CardId.New(),
                title: concreteCard.Title,
                description: concreteCard.GetDescription() ?? "",
                detailedDescription: concreteCard.GetDetailedDescription() ?? "",
                attack: concreteCard.Attack,
                defense: concreteCard.Defense,
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

            // Phase 142: Sync additional runtime state
            // Copy TimesRevived for resurrection ability tracking
            for (int i = 0; i < concreteCard.TimesRevived; i++)
                domainCard.IncrementTimesRevived();

            // Copy BonusAttacks for multi-attack abilities
            if (concreteCard.BonusAttacks > 0)
                domainCard.SetBonusAttacks(concreteCard.BonusAttacks);

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
            // Phase 144: Add null check before accessing Count to prevent NullReferenceException
            if (domainCard.Families != null && domainCard.Families.Count > 0)
            {
                inGameCard.Families = ConvertFamiliesToLegacy(domainCard.Families);
            }

            // Phase 142: Sync additional runtime state
            // Sync TimesRevived for resurrection ability tracking
            inGameCard.TimesRevived = domainCard.TimesRevived;

            // Sync BonusAttacks for multi-attack abilities
            // Domain Card uses BonusAttacks as extra attacks, InGameCard tracks total remaining
            inGameCard.BonusAttacks = domainCard.BonusAttacks;

            // Sync AttackBlocked
            if (domainCard.AttackBlocked)
            {
                inGameCard.BlockAttack();
            }
            else
            {
                inGameCard.UnblockAttack();
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

        /// <summary>
        /// Syncs all field cards from the player repository back to InGameInvocationCards.
        /// Phase 143: Matches cards by Title since CardId differs between domain and presentation.
        /// Phase 144: Added logging for debugging sync failures.
        /// </summary>
        public void SyncAllFieldCards(PlayerId playerId, IPlayerCardCollection playerCards)
        {
            if (playerCards == null)
            {
                UnityEngine.Debug.LogWarning("[CardSyncService] SyncAllFieldCards skipped: playerCards is null");
                return;
            }
            if (_playerRepository == null)
            {
                UnityEngine.Debug.LogWarning("[CardSyncService] SyncAllFieldCards skipped: _playerRepository is null");
                return;
            }

            var player = _playerRepository.GetPlayer(playerId);
            if (player == null)
            {
                UnityEngine.Debug.LogWarning($"[CardSyncService] SyncAllFieldCards skipped: player not found for {playerId}");
                return;
            }

            foreach (var inGameCard in playerCards.InvocationCards)
            {
                if (inGameCard is not InGameInvocationCard concreteCard)
                    continue;

                // Match by Title (CardId differs between domain and presentation systems)
                var domainCard = player.Field.FirstOrDefault(c => c.Title == concreteCard.Title);
                if (domainCard == null)
                    continue;

                // Sync stats (domain Card uses int, InGameInvocationCard uses float)
                if (domainCard.Stats.HasValue)
                {
                    concreteCard.Attack = domainCard.Stats.Value.Attack;
                    concreteCard.Defense = domainCard.Stats.Value.Defense;
                }

                // Sync boolean state flags
                if (concreteCard.CancelEffect != domainCard.CancelEffect)
                {
                    concreteCard.CancelEffect = domainCard.CancelEffect;
                }

                concreteCard.CanDirectAttack = domainCard.CanDirectAttack;
                concreteCard.CantBeAttack = domainCard.CantBeAttacked;

                // Sync families if they were changed
                // Phase 144: Add null check before accessing Count to prevent NullReferenceException
                if (domainCard.Families != null && domainCard.Families.Count > 0)
                {
                    concreteCard.Families = ConvertFamiliesToLegacy(domainCard.Families);
                }

                // Sync runtime state counters
                concreteCard.TimesRevived = domainCard.TimesRevived;
                concreteCard.BonusAttacks = domainCard.BonusAttacks;

                // Sync AttackBlocked state
                if (domainCard.AttackBlocked)
                {
                    concreteCard.BlockAttack();
                }
                else
                {
                    concreteCard.UnblockAttack();
                }
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
