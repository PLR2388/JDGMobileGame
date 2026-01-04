using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using JDG.Application.Abilities;
using JDG.Application.Cards;
using JDG.Application.Services;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;
using UnityEngine;
using DomainCardFamily = JDG.Domain.Enums.CardFamily;

namespace Services
{
    /// <summary>
    /// Adapter for IAbilityExecutor to execute modern IAbility implementations.
    /// Phase 74: Created as part of UseCase migration.
    /// Phase 106: Updated to also execute modern IAbility implementations.
    /// Phase 118: Removed all legacy Ability references, uses only ModernAbilities.
    /// Phase 141: Added ICardSyncService to sync domain Card changes to InGameInvocationCard.
    ///
    /// This adapter allows use cases to trigger abilities without coupling
    /// to concrete card types. It executes modern IAbility implementations
    /// based on ability triggers.
    /// </summary>
    public class AbilityExecutorAdapter : IAbilityExecutor
    {
        private readonly ICanvasProvider _canvasProvider;
        private readonly ICardSyncService _cardSyncService;

        /// <summary>
        /// Phase 144: Added null validation for required parameters.
        /// </summary>
        public AbilityExecutorAdapter(ICanvasProvider canvasProvider, ICardSyncService cardSyncService)
        {
            _canvasProvider = canvasProvider ?? throw new System.ArgumentNullException(nameof(canvasProvider));
            _cardSyncService = cardSyncService ?? throw new System.ArgumentNullException(nameof(cardSyncService));
        }

        #region Modern Ability Helpers

        /// <summary>
        /// Creates an AbilityContext for modern ability execution.
        /// Phase 106: Bridge between legacy types and modern domain types.
        /// Phase 156: Fixed - now properly converts sourceCard to domain Card instead of passing null.
        /// </summary>
        private AbilityContext CreateAbilityContext(
            InGameCard sourceCard,
            CardOwner owner,
            AbilityName abilityName = AbilityName.Default)
        {
            var ownerId = PlayerId.FromCardOwner((JDG.Domain.CardOwner)(int)owner);
            var opponentOwner = owner == CardOwner.Player1 ? CardOwner.Player2 : CardOwner.Player1;
            var opponentId = PlayerId.FromCardOwner((JDG.Domain.CardOwner)(int)opponentOwner);

            // Phase 156: Convert sourceCard to domain Card for ability context
            var domainSourceCard = ConvertToDomainCard(sourceCard);

            return new AbilityContext(ownerId, opponentId, domainSourceCard, abilityName);
        }

        /// <summary>
        /// Converts an InGameCard to a domain Card for use in AbilityContext.SourceCard.
        /// Phase 156: Created to fix null SourceCard issue in CreateAbilityContext.
        ///
        /// This creates a read-only domain Card without linking (no sync needed for SourceCard).
        /// For TargetCard that needs modification syncing, use ICardSyncService.CreateLinkedCard instead.
        /// </summary>
        /// <param name="sourceCard">The InGameCard to convert.</param>
        /// <returns>A domain Card, or null if sourceCard is null.</returns>
        private Card ConvertToDomainCard(InGameCard sourceCard)
        {
            if (sourceCard == null) return null;

            if (sourceCard is InGameInvocationCard invocation)
            {
                // Convert legacy CardFamily[] to domain CardFamily[]
                var domainFamilies = invocation.Families?
                    .Select(f => (DomainCardFamily)(int)f)
                    .ToList() ?? new System.Collections.Generic.List<DomainCardFamily>();

                return Card.CreateInvocation(
                    CardId.Create(),
                    invocation.Title ?? "Unknown",
                    invocation.Description ?? "",
                    invocation.DetailedDescription ?? "",
                    invocation.Attack,
                    invocation.Defense,
                    domainFamilies,
                    invocation.IsAffectedByEffectCard
                );
            }
            else if (sourceCard is InGameEquipmentCard equipment)
            {
                // Convert to equipment card (no stats needed for equipment context)
                return Card.CreateEquipment(
                    CardId.Create(),
                    equipment.Title ?? "Unknown",
                    equipment.Description ?? "",
                    equipment.DetailedDescription ?? "",
                    System.Linq.Enumerable.Empty<JDG.Domain.Enums.EquipmentAbilityName>()
                );
            }
            else if (sourceCard is InGameFieldCard fieldCard)
            {
                // Convert to field card
                var domainFamily = (DomainCardFamily)(int)fieldCard.Family;
                return Card.CreateField(
                    CardId.Create(),
                    fieldCard.Title ?? "Unknown",
                    fieldCard.Description ?? "",
                    fieldCard.DetailedDescription ?? "",
                    domainFamily,
                    System.Linq.Enumerable.Empty<JDG.Domain.Enums.FieldAbilityName>()
                );
            }
            else if (sourceCard is InGameEffectCard effectCard)
            {
                // Convert to effect card
                return Card.CreateEffect(
                    CardId.Create(),
                    effectCard.Title ?? "Unknown",
                    effectCard.Description ?? "",
                    effectCard.DetailedDescription ?? "",
                    System.Linq.Enumerable.Empty<JDG.Domain.Enums.EffectAbilityName>()
                );
            }

            // Fallback: log warning and return null for unknown types
            Debug.LogWarning($"[AbilityExecutorAdapter] ConvertToDomainCard: Unknown card type {sourceCard.GetType().Name}");
            return null;
        }

        // Phase 141: Removed ConvertToCard - replaced by ICardSyncService.CreateLinkedCard
        // The old method created a disconnected domain Card using CreateEffect (no stats),
        // causing equipment ability stat modifications to be lost.
        // Phase 156: Added ConvertToDomainCard for SourceCard conversion (read-only context).

        /// <summary>
        /// Executes modern abilities that match the specified trigger.
        /// Phase 106: Enables parallel execution of modern abilities alongside legacy.
        /// </summary>
        private void ExecuteModernAbilities(
            System.Collections.Generic.IEnumerable<IAbility> abilities,
            AbilityTrigger trigger,
            AbilityContext context)
        {
            foreach (var ability in abilities)
            {
                // Only execute passive abilities with matching trigger
                if (ability is IPassiveAbility passiveAbility && passiveAbility.Trigger == trigger)
                {
                    if (ability.CanActivate(context))
                    {
                        var result = ability.Execute(context);
                        if (!result.IsSuccess && !string.IsNullOrEmpty(result.Message))
                        {
                            Debug.LogWarning($"[AbilityExecutorAdapter] Modern ability failed: {result.Message}");
                        }
                    }
                }
            }
        }

        #endregion

        #region Death Triggers

        /// <summary>
        /// Executes death-related abilities.
        /// Phase 118: Removed legacy ability calls, uses only ModernAbilities.
        /// </summary>
        public void ExecuteOnCardDeath(
            IInGameInvocationCard deadCard,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards)
        {
            if (deadCard is InGameInvocationCard concreteDeadCard)
            {
                // Phase 118: Execute modern abilities with OnDeath trigger
                var context = CreateAbilityContext(concreteDeadCard, concreteDeadCard.CardOwner);
                ExecuteModernAbilities(concreteDeadCard.ModernAbilities, AbilityTrigger.OnDeath, context);
            }
        }

        #endregion

        #region Field Entry/Exit Triggers

        /// <summary>
        /// Executes abilities when a card is added to the field.
        /// Phase 118: Removed legacy ability calls, uses only ModernAbilities.
        /// Phase 141: Added ICardSyncService for equipment ability state sync.
        /// </summary>
        public void ExecuteOnCardAddedToField(
            IInGameInvocationCard addedCard,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards)
        {
            if (addedCard is InGameInvocationCard concreteAdded &&
                ownerCards is PlayerCards concreteOwner &&
                opponentCards is PlayerCards concreteOpponent)
            {
                // 1. Trigger opponent's equipment abilities that react to new cards
                // Phase 141: Use ICardSyncService to sync any changes to the added card
                // Phase 148: Added null-coalescing to prevent NullReferenceException
                foreach (var opponentCard in concreteOpponent.InvocationCards ?? System.Linq.Enumerable.Empty<InGameInvocationCard>())
                {
                    var equipmentCard = opponentCard.EquipmentCard;
                    if (equipmentCard == null) continue;

                    var equipContext = CreateAbilityContext(equipmentCard, opponentCard.CardOwner);
                    equipContext.TargetCard = _cardSyncService.CreateLinkedCard(addedCard);
                    // Phase 144: Skip if CreateLinkedCard returns null (card is not InGameInvocationCard)
                    if (equipContext.TargetCard == null) continue;

                    ExecuteModernAbilities(equipmentCard.ModernEquipmentAbilities, AbilityTrigger.OnCardPlayed, equipContext);

                    // Phase 141: Sync state changes back to the added InGameInvocationCard
                    _cardSyncService.SyncCardState(equipContext.TargetCard);
                    _cardSyncService.ClearMapping(equipContext.TargetCard);
                }

                // 2. Trigger existing invocation card abilities on same field
                // Phase 148: Added null-coalescing to prevent NullReferenceException
                foreach (var existingCard in concreteOwner.InvocationCards ?? System.Linq.Enumerable.Empty<InGameInvocationCard>())
                {
                    // Phase 118: Use only ModernAbilities
                    var invocContext = CreateAbilityContext(existingCard, existingCard.CardOwner);
                    ExecuteModernAbilities(existingCard.ModernAbilities, AbilityTrigger.OnCardPlayed, invocContext);
                }

                // 3. Trigger effect card abilities
                // Phase 148: Added null-coalescing to prevent NullReferenceException
                foreach (var effectCard in concreteOwner.EffectCards ?? System.Linq.Enumerable.Empty<InGameEffectCard>())
                {
                    if (effectCard is InGameEffectCard concreteEffect)
                    {
                        var effectContext = CreateAbilityContext(concreteEffect, concreteEffect.CardOwner);
                        ExecuteModernAbilities(concreteEffect.ModernEffectAbilities, AbilityTrigger.OnCardPlayed, effectContext);
                    }
                }

                // 4. Trigger field card abilities
                if (concreteOwner.FieldCard != null)
                {
                    var fieldContext = CreateAbilityContext(concreteOwner.FieldCard, concreteOwner.FieldCard.CardOwner);
                    ExecuteModernAbilities(concreteOwner.FieldCard.ModernFieldAbilities, AbilityTrigger.OnCardPlayed, fieldContext);
                }

                // 5. Trigger OnSummon for the added card's modern abilities
                var summonContext = CreateAbilityContext(concreteAdded, concreteAdded.CardOwner);
                ExecuteModernAbilities(concreteAdded.ModernAbilities, AbilityTrigger.OnSummon, summonContext);

                // Phase 143: Sync all field cards after ability executions
                // Modern abilities may have modified cards via IPlayerRepository
                var ownerPlayerId = PlayerId.FromCardOwner((JDG.Domain.CardOwner)(int)concreteAdded.CardOwner);
                _cardSyncService.SyncAllFieldCards(ownerPlayerId, concreteOwner);
            }
        }

        /// <summary>
        /// Executes abilities when a card is removed from the field.
        /// Phase 118: Removed legacy ability calls, uses only ModernAbilities.
        /// Note: Modern abilities use AbilityTrigger for triggering. Card removal
        /// is not yet a specific trigger, so this method is a placeholder.
        /// </summary>
        public void ExecuteOnCardRemovedFromField(
            IInGameInvocationCard removedCard,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards)
        {
            // Phase 118: Modern abilities don't have a specific OnCardRemoved trigger yet
            // This is intentional - card removal reactions are handled via OnDeath trigger
            // or through game state observation patterns in the modern system
        }

        public void ExecuteOnFieldCardChanged(
            IInGameFieldCard oldFieldCard,
            IInGameFieldCard newFieldCard,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards)
        {
            // Phase 116: Removed legacy FieldAbility calls - now uses only modern IAbility
            // Note: Modern abilities don't have a specific trigger for field card removal yet
            // This will be added when field removal triggers are needed
            if (oldFieldCard is InGameFieldCard concreteOldField)
            {
                var context = CreateAbilityContext(concreteOldField, concreteOldField.CardOwner);
                // Execute any OnDeath-like triggers for the removed field card
                // Currently no specific trigger exists for field removal in AbilityTrigger enum
            }
        }

        #endregion

        #region Turn Triggers

        /// <summary>
        /// Executes turn start abilities for all player cards.
        /// Phase 118: Removed legacy ability calls, uses only ModernAbilities.
        /// </summary>
        public void ExecuteOnTurnStart(
            IPlayerCardCollection currentPlayerCards,
            IPlayerCardCollection opponentCards)
        {
            if (currentPlayerCards is PlayerCards concretePlayer)
            {
                // Execute invocation card turn start abilities
                // Phase 148: Added null-coalescing to prevent NullReferenceException
                foreach (var invocation in concretePlayer.InvocationCards ?? System.Linq.Enumerable.Empty<InGameInvocationCard>())
                {
                    if (invocation is InGameInvocationCard invocationCard)
                    {
                        // Phase 118: Use only ModernAbilities
                        var context = CreateAbilityContext(invocationCard, invocationCard.CardOwner);
                        ExecuteModernAbilities(invocationCard.ModernAbilities, AbilityTrigger.OnTurnStart, context);
                    }
                }

                // Field abilities for turn start
                if (concretePlayer.FieldCard != null)
                {
                    var fieldContext = CreateAbilityContext(concretePlayer.FieldCard, concretePlayer.FieldCard.CardOwner);
                    ExecuteModernAbilities(concretePlayer.FieldCard.ModernFieldAbilities, AbilityTrigger.OnTurnStart, fieldContext);
                }

                // Effect abilities for turn start
                // Phase 148: Added null-coalescing to prevent NullReferenceException
                foreach (var effectCard in concretePlayer.EffectCards ?? System.Linq.Enumerable.Empty<InGameEffectCard>())
                {
                    if (effectCard is InGameEffectCard concreteEffect)
                    {
                        var effectContext = CreateAbilityContext(concreteEffect, concreteEffect.CardOwner);
                        ExecuteModernAbilities(concreteEffect.ModernEffectAbilities, AbilityTrigger.OnTurnStart, effectContext);
                    }
                }

                // Phase 143: Sync all field cards after turn start abilities
                var playerId = concretePlayer.IsPlayerOne ? PlayerId.Player1 : PlayerId.Player2;
                _cardSyncService.SyncAllFieldCards(playerId, concretePlayer);
            }
        }

        public void ExecuteOnTurnEnd(
            IPlayerCardCollection currentPlayerCards,
            IPlayerCardCollection opponentCards)
        {
            // Note: Legacy Ability class does not have OnTurnEnd method.
            // Turn end logic should be handled by the game loop directly.

            // Phase 106: Modern abilities DO support OnTurnEnd trigger
            if (currentPlayerCards is PlayerCards concretePlayer)
            {
                // Invocation cards
                // Phase 148: Added null-coalescing to prevent NullReferenceException
                foreach (var invocation in concretePlayer.InvocationCards ?? System.Linq.Enumerable.Empty<InGameInvocationCard>())
                {
                    if (invocation is InGameInvocationCard invocationCard)
                    {
                        var context = CreateAbilityContext(invocationCard, invocationCard.CardOwner);
                        ExecuteModernAbilities(invocationCard.ModernAbilities, AbilityTrigger.OnTurnEnd, context);
                    }
                }

                // Field card
                if (concretePlayer.FieldCard != null)
                {
                    var fieldContext = CreateAbilityContext(concretePlayer.FieldCard, concretePlayer.FieldCard.CardOwner);
                    ExecuteModernAbilities(concretePlayer.FieldCard.ModernFieldAbilities, AbilityTrigger.OnTurnEnd, fieldContext);
                }

                // Effect cards
                // Phase 148: Added null-coalescing to prevent NullReferenceException
                foreach (var effectCard in concretePlayer.EffectCards ?? System.Linq.Enumerable.Empty<InGameEffectCard>())
                {
                    if (effectCard is InGameEffectCard concreteEffect)
                    {
                        var effectContext = CreateAbilityContext(concreteEffect, concreteEffect.CardOwner);
                        ExecuteModernAbilities(concreteEffect.ModernEffectAbilities, AbilityTrigger.OnTurnEnd, effectContext);
                    }
                }

                // Phase 143: Sync all field cards after turn end abilities
                var playerId = concretePlayer.IsPlayerOne ? PlayerId.Player1 : PlayerId.Player2;
                _cardSyncService.SyncAllFieldCards(playerId, concretePlayer);
            }
        }

        #endregion

        #region Hand Change Triggers

        public void ExecuteOnHandCardsChanged(
            IPlayerCardCollection playerCards,
            IPlayerCardCollection opponentCards,
            int oldCount,
            int newCount)
        {
            if (playerCards is PlayerCards concretePlayer)
            {
                // Phase 117: Uses only modern abilities with OnHandChange trigger
                // Phase 141: Use ICardSyncService to sync domain Card changes back to InGameInvocationCard
                // Phase 148: Added null-coalescing to prevent NullReferenceException
                foreach (var invocation in concretePlayer.InvocationCards ?? System.Linq.Enumerable.Empty<InGameInvocationCard>())
                {
                    if (invocation is InGameInvocationCard invocationCard &&
                        invocationCard.EquipmentCard != null)
                    {
                        var context = CreateAbilityContext(invocationCard.EquipmentCard, invocationCard.CardOwner);
                        context.TargetCard = _cardSyncService.CreateLinkedCard(invocationCard);
                        // Phase 144: Skip if CreateLinkedCard returns null
                        if (context.TargetCard == null) continue;

                        ExecuteModernAbilities(invocationCard.EquipmentCard.ModernEquipmentAbilities, AbilityTrigger.OnHandChange, context);

                        // Phase 141: Sync state changes back to InGameInvocationCard
                        _cardSyncService.SyncCardState(context.TargetCard);
                        _cardSyncService.ClearMapping(context.TargetCard);
                    }
                }
            }
        }

        #endregion

        #region Equipment Triggers

        public void ExecuteOnEquipmentAttached(
            IInGameEquipmentCard equipment,
            IInGameInvocationCard target,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards)
        {
            if (equipment is InGameEquipmentCard concreteEquipment &&
                target is InGameInvocationCard concreteTarget)
            {
                // Phase 117: Uses modern abilities with OnEquip trigger
                // Phase 141: Use ICardSyncService to sync domain Card changes back to InGameInvocationCard
                var context = CreateAbilityContext(concreteEquipment, concreteTarget.CardOwner);
                context.TargetCard = _cardSyncService.CreateLinkedCard(target);
                // Phase 144: Return early if CreateLinkedCard returns null
                if (context.TargetCard == null) return;

                ExecuteModernAbilities(concreteEquipment.ModernEquipmentAbilities, AbilityTrigger.OnEquip, context);

                // Phase 141: Sync state changes back to InGameInvocationCard
                _cardSyncService.SyncCardState(context.TargetCard);
                _cardSyncService.ClearMapping(context.TargetCard);
            }
        }

        public void ExecuteOnEquipmentDetached(
            IInGameEquipmentCard equipment,
            IInGameInvocationCard previousTarget,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards)
        {
            if (equipment is InGameEquipmentCard concreteEquipment &&
                previousTarget is InGameInvocationCard concreteTarget)
            {
                // Phase 117: Uses modern abilities with OnUnequip trigger
                // Phase 141: Use ICardSyncService to sync domain Card changes back to InGameInvocationCard
                var context = CreateAbilityContext(concreteEquipment, concreteTarget.CardOwner);
                context.TargetCard = _cardSyncService.CreateLinkedCard(previousTarget);
                // Phase 144: Return early if CreateLinkedCard returns null
                if (context.TargetCard == null) return;

                ExecuteModernAbilities(concreteEquipment.ModernEquipmentAbilities, AbilityTrigger.OnUnequip, context);

                // Phase 141: Sync state changes back to InGameInvocationCard
                _cardSyncService.SyncCardState(context.TargetCard);
                _cardSyncService.ClearMapping(context.TargetCard);
            }
        }

        #endregion

        #region Effect Card Triggers

        /// <summary>
        /// Executes abilities when an effect card is played to the field.
        /// Phase 114: Added for effect card ability migration.
        /// </summary>
        public void ExecuteOnEffectCardPlayed(
            IInGameEffectCard effectCard,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards)
        {
            if (effectCard is InGameEffectCard concreteEffect &&
                ownerCards is PlayerCards concreteOwner)
            {
                // Phase 114: Execute modern abilities with OnCardPlayed trigger
                var context = CreateAbilityContext(concreteEffect, concreteEffect.CardOwner);

                // Execute all modern effect abilities
                foreach (var ability in concreteEffect.ModernEffectAbilities)
                {
                    if (ability.CanActivate(context))
                    {
                        var result = ability.Execute(context);
                        if (!result.IsSuccess && !string.IsNullOrEmpty(result.Message))
                        {
                            UnityEngine.Debug.LogWarning($"[AbilityExecutorAdapter] Effect ability failed: {result.Message}");
                        }
                    }
                }

                // Phase 143: Sync all field cards after effect card abilities
                var playerId = PlayerId.FromCardOwner((JDG.Domain.CardOwner)(int)concreteEffect.CardOwner);
                _cardSyncService.SyncAllFieldCards(playerId, concreteOwner);
            }
        }

        #endregion
    }
}
