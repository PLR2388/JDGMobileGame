using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using JDG.Application.Abilities;
using JDG.Application.Cards;
using JDG.Application.Services;
using JDG.Domain;
using JDG.Domain.ValueObjects;
using UnityEngine;

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

        public AbilityExecutorAdapter(ICanvasProvider canvasProvider, ICardSyncService cardSyncService)
        {
            _canvasProvider = canvasProvider;
            _cardSyncService = cardSyncService;
        }

        #region Modern Ability Helpers

        /// <summary>
        /// Creates an AbilityContext for modern ability execution.
        /// Phase 106: Bridge between legacy types and modern domain types.
        /// </summary>
        private AbilityContext CreateAbilityContext(
            InGameCard sourceCard,
            CardOwner owner,
            AbilityName abilityName = AbilityName.Default)
        {
            var ownerId = PlayerId.FromCardOwner((JDG.Domain.CardOwner)(int)owner);
            var opponentOwner = owner == CardOwner.Player1 ? CardOwner.Player2 : CardOwner.Player1;
            var opponentId = PlayerId.FromCardOwner((JDG.Domain.CardOwner)(int)opponentOwner);

            return new AbilityContext(ownerId, opponentId, null, abilityName);
        }

        // Phase 141: Removed ConvertToCard - replaced by ICardSyncService.CreateLinkedCard
        // The old method created a disconnected domain Card using CreateEffect (no stats),
        // causing equipment ability stat modifications to be lost.

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
                foreach (var opponentCard in concreteOpponent.InvocationCards)
                {
                    var equipmentCard = opponentCard.EquipmentCard;
                    if (equipmentCard == null) continue;

                    var equipContext = CreateAbilityContext(equipmentCard, opponentCard.CardOwner);
                    equipContext.TargetCard = _cardSyncService.CreateLinkedCard(addedCard);

                    ExecuteModernAbilities(equipmentCard.ModernEquipmentAbilities, AbilityTrigger.OnCardPlayed, equipContext);

                    // Phase 141: Sync state changes back to the added InGameInvocationCard
                    _cardSyncService.SyncCardState(equipContext.TargetCard);
                    _cardSyncService.ClearMapping(equipContext.TargetCard);
                }

                // 2. Trigger existing invocation card abilities on same field
                foreach (var existingCard in concreteOwner.InvocationCards)
                {
                    // Phase 118: Use only ModernAbilities
                    var invocContext = CreateAbilityContext(existingCard, existingCard.CardOwner);
                    ExecuteModernAbilities(existingCard.ModernAbilities, AbilityTrigger.OnCardPlayed, invocContext);
                }

                // 3. Trigger effect card abilities
                foreach (var effectCard in concreteOwner.EffectCards)
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
                foreach (var invocation in concretePlayer.InvocationCards)
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
                foreach (var effectCard in concretePlayer.EffectCards)
                {
                    if (effectCard is InGameEffectCard concreteEffect)
                    {
                        var effectContext = CreateAbilityContext(concreteEffect, concreteEffect.CardOwner);
                        ExecuteModernAbilities(concreteEffect.ModernEffectAbilities, AbilityTrigger.OnTurnStart, effectContext);
                    }
                }
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
                foreach (var invocation in concretePlayer.InvocationCards)
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
                foreach (var effectCard in concretePlayer.EffectCards)
                {
                    if (effectCard is InGameEffectCard concreteEffect)
                    {
                        var effectContext = CreateAbilityContext(concreteEffect, concreteEffect.CardOwner);
                        ExecuteModernAbilities(concreteEffect.ModernEffectAbilities, AbilityTrigger.OnTurnEnd, effectContext);
                    }
                }
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
                foreach (var invocation in concretePlayer.InvocationCards)
                {
                    if (invocation is InGameInvocationCard invocationCard &&
                        invocationCard.EquipmentCard != null)
                    {
                        var context = CreateAbilityContext(invocationCard.EquipmentCard, invocationCard.CardOwner);
                        context.TargetCard = _cardSyncService.CreateLinkedCard(invocationCard);

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
            }
        }

        #endregion
    }
}
