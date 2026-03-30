using System.Collections.Generic;
using System.Linq;
using JDG.Application.Cards;
using JDG.Application.Repositories;
using JDG.Application.Services;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Cards;

namespace JDG.Infrastructure.Services
{
    /// <summary>
    /// Service for synchronizing domain Card state changes back to InGameInvocationCard.
    ///
    /// Phase 141: Fixes the equipment ability sync issue where domain Card modifications
    /// were lost because ConvertToCard() created a disconnected temporary Card.
    /// Phase 143: Added bulk sync for modern abilities that modify cards via IPlayerRepository.
    /// Phase 166: Simplified - uses domain enums directly (legacy enum conversion removed).
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
        /// Phase 166: Simplified - Families are already domain CardFamily[], no conversion needed.
        /// </summary>
        public Card CreateLinkedCard(IInGameInvocationCard inGameCard)
        {
            if (inGameCard is not InGameInvocationCard concreteCard)
                return null;

            // Phase 166: Families are already domain CardFamily[], pass directly
            var domainFamilies = concreteCard.Families ?? System.Array.Empty<CardFamily>();

            // Create domain Card using CreateInvocation with CURRENT stats (not base stats)
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
            for (int i = 0; i < concreteCard.TimesRevived; i++)
                domainCard.IncrementTimesRevived();

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

            // Sync stats
            if (domainCard.Stats.HasValue)
            {
                inGameCard.Attack = domainCard.Stats.Value.Attack;
                inGameCard.Defense = domainCard.Stats.Value.Defense;
            }

            // Sync boolean state flags
            if (inGameCard.CancelEffect != domainCard.CancelEffect)
            {
                inGameCard.CancelEffect = domainCard.CancelEffect;
            }

            inGameCard.CanDirectAttack = domainCard.CanDirectAttack;
            inGameCard.CantBeAttack = domainCard.CantBeAttacked;

            // Phase 166: Families are already domain CardFamily[], assign directly
            if (domainCard.Families != null && domainCard.Families.Count > 0)
            {
                inGameCard.Families = domainCard.Families.ToArray();
            }

            // Phase 142: Sync additional runtime state
            inGameCard.TimesRevived = domainCard.TimesRevived;
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
        /// Phase 166: Simplified - Families are already domain CardFamily[], assign directly.
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

                var domainCard = player.Field.FirstOrDefault(c => c.Title == concreteCard.Title);
                if (domainCard == null)
                    continue;

                if (domainCard.Stats.HasValue)
                {
                    concreteCard.Attack = domainCard.Stats.Value.Attack;
                    concreteCard.Defense = domainCard.Stats.Value.Defense;
                }

                if (concreteCard.CancelEffect != domainCard.CancelEffect)
                {
                    concreteCard.CancelEffect = domainCard.CancelEffect;
                }

                concreteCard.CanDirectAttack = domainCard.CanDirectAttack;
                concreteCard.CantBeAttack = domainCard.CantBeAttacked;

                // Phase 166: Families are already domain CardFamily[], assign directly
                if (domainCard.Families != null && domainCard.Families.Count > 0)
                {
                    concreteCard.Families = domainCard.Families.ToArray();
                }

                concreteCard.TimesRevived = domainCard.TimesRevived;
                concreteCard.BonusAttacks = domainCard.BonusAttacks;

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
    }
}
