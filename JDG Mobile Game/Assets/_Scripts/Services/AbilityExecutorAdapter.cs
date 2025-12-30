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
    /// Adapter bridging IAbilityExecutor to legacy Ability classes.
    /// Phase 74: Created as part of UseCase migration.
    /// Phase 106: Updated to also execute modern IAbility implementations.
    ///
    /// This adapter allows use cases to trigger abilities without coupling
    /// to the legacy Ability class. It wraps both legacy ability execution
    /// and modern IAbility execution, enabling gradual migration.
    ///
    /// Execution order: Legacy abilities first, then modern abilities.
    /// </summary>
    public class AbilityExecutorAdapter : IAbilityExecutor
    {
        private readonly ICanvasProvider _canvasProvider;

        public AbilityExecutorAdapter(ICanvasProvider canvasProvider)
        {
            _canvasProvider = canvasProvider;
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

        public void ExecuteOnCardDeath(
            IInGameInvocationCard deadCard,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards)
        {
            // Cast to concrete types for legacy ability system
            if (deadCard is InGameInvocationCard concreteDeadCard &&
                ownerCards is PlayerCards concreteOwner &&
                opponentCards is PlayerCards concreteOpponent)
            {
                var canvas = _canvasProvider.GetGameCanvas() as Transform;

                // Legacy ability execution
                foreach (var ability in concreteDeadCard.Abilities)
                {
                    ability.OnCardDeath(canvas, concreteDeadCard, concreteOwner, concreteOpponent);
                }

                // Phase 106: Modern ability execution
                var context = CreateAbilityContext(concreteDeadCard, concreteDeadCard.CardOwner);
                ExecuteModernAbilities(concreteDeadCard.ModernAbilities, AbilityTrigger.OnDeath, context);
            }
        }

        #endregion

        #region Field Entry/Exit Triggers

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
                foreach (var opponentCard in concreteOpponent.InvocationCards)
                {
                    var equipmentCard = opponentCard.EquipmentCard;
                    if (equipmentCard == null) continue;

                    foreach (var equipmentAbility in equipmentCard.EquipmentAbilities)
                    {
                        equipmentAbility.OnOpponentInvocationCardAdded(concreteAdded);
                    }

                    // Phase 106: Modern equipment abilities
                    var equipContext = CreateAbilityContext(equipmentCard, opponentCard.CardOwner);
                    ExecuteModernAbilities(equipmentCard.ModernEquipmentAbilities, AbilityTrigger.OnCardPlayed, equipContext);
                }

                // 2. Trigger existing invocation card abilities on same field
                foreach (var existingCard in concreteOwner.InvocationCards)
                {
                    foreach (var ability in existingCard.Abilities)
                    {
                        ability.OnCardAdded(concreteAdded, concreteOwner);
                    }

                    // Phase 106: Modern invocation abilities
                    var invocContext = CreateAbilityContext(existingCard, existingCard.CardOwner);
                    ExecuteModernAbilities(existingCard.ModernAbilities, AbilityTrigger.OnCardPlayed, invocContext);
                }

                // 3. Trigger effect card abilities
                // Phase 115: Removed legacy EffectAbility calls - now uses only modern IAbility
                foreach (var effectCard in concreteOwner.EffectCards)
                {
                    if (effectCard is InGameEffectCard concreteEffect)
                    {
                        var effectContext = CreateAbilityContext(concreteEffect, concreteEffect.CardOwner);
                        ExecuteModernAbilities(concreteEffect.ModernEffectAbilities, AbilityTrigger.OnCardPlayed, effectContext);
                    }
                }

                // 4. Trigger field card abilities
                if (concreteOwner.FieldCard?.FieldAbilities != null)
                {
                    foreach (var fieldAbility in concreteOwner.FieldCard.FieldAbilities)
                    {
                        fieldAbility.OnInvocationCardAdded(concreteAdded, concreteOwner);
                    }

                    // Phase 106: Modern field abilities
                    var fieldContext = CreateAbilityContext(concreteOwner.FieldCard, concreteOwner.FieldCard.CardOwner);
                    ExecuteModernAbilities(concreteOwner.FieldCard.ModernFieldAbilities, AbilityTrigger.OnCardPlayed, fieldContext);
                }

                // 5. Phase 106: Trigger OnSummon for the added card's modern abilities
                var summonContext = CreateAbilityContext(concreteAdded, concreteAdded.CardOwner);
                ExecuteModernAbilities(concreteAdded.ModernAbilities, AbilityTrigger.OnSummon, summonContext);
            }
        }

        public void ExecuteOnCardRemovedFromField(
            IInGameInvocationCard removedCard,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards)
        {
            if (removedCard is InGameInvocationCard concreteRemoved &&
                ownerCards is PlayerCards concreteOwner)
            {
                // Clone the list to avoid modification during iteration
                var remainingCards = concreteOwner.InvocationCards.ToList();

                // 1. Trigger OnCardRemove on ALL remaining invocation cards' abilities
                // This allows cards to react when other cards leave the field
                // Ability.OnCardRemove signature: (InGameInvocationCard removeCard, PlayerCards playerCards)
                foreach (var invocationCard in remainingCards)
                {
                    foreach (var ability in invocationCard.Abilities)
                    {
                        ability.OnCardRemove(concreteRemoved, concreteOwner);
                    }

                    // Phase 106: Modern abilities (no specific trigger for card removal yet)
                }

                // 2. Trigger effect abilities that react to invocation removal
                // Phase 115: Removed legacy EffectAbility calls - now uses only modern IAbility
                // Note: Modern abilities don't have a specific trigger for card removal yet
                // This will be added when card removal triggers are needed for effect cards
            }
        }

        public void ExecuteOnFieldCardChanged(
            IInGameFieldCard oldFieldCard,
            IInGameFieldCard newFieldCard,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards)
        {
            if (ownerCards is PlayerCards concreteOwner)
            {
                // Trigger OnFieldCardRemoved on old field card's abilities
                // FieldAbility.OnFieldCardRemoved signature: (PlayerCards playerCards)
                if (oldFieldCard is InGameFieldCard concreteOldField)
                {
                    foreach (var ability in concreteOldField.FieldAbilities)
                    {
                        ability.OnFieldCardRemoved(concreteOwner);
                    }
                }
            }
        }

        #endregion

        #region Turn Triggers

        public void ExecuteOnTurnStart(
            IPlayerCardCollection currentPlayerCards,
            IPlayerCardCollection opponentCards)
        {
            if (currentPlayerCards is PlayerCards concretePlayer &&
                opponentCards is PlayerCards concreteOpponent)
            {
                var canvas = _canvasProvider.GetGameCanvas() as Transform;

                // Note: FieldAbility.OnTurnStart requires PlayerStatus which is not available
                // through IPlayerCardCollection. Field ability turn start triggers should be
                // handled by the game loop which has access to PlayerStatus.

                // Execute invocation card turn start abilities
                // Ability.OnTurnStart signature: (Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
                foreach (var invocation in concretePlayer.InvocationCards)
                {
                    if (invocation is InGameInvocationCard invocationCard)
                    {
                        // Legacy abilities
                        foreach (var ability in invocationCard.Abilities)
                        {
                            ability.OnTurnStart(canvas, concretePlayer, concreteOpponent);
                        }

                        // Phase 106: Modern abilities
                        var context = CreateAbilityContext(invocationCard, invocationCard.CardOwner);
                        ExecuteModernAbilities(invocationCard.ModernAbilities, AbilityTrigger.OnTurnStart, context);
                    }
                }

                // Phase 106: Modern field abilities for turn start
                if (concretePlayer.FieldCard != null)
                {
                    var fieldContext = CreateAbilityContext(concretePlayer.FieldCard, concretePlayer.FieldCard.CardOwner);
                    ExecuteModernAbilities(concretePlayer.FieldCard.ModernFieldAbilities, AbilityTrigger.OnTurnStart, fieldContext);
                }

                // Phase 106: Modern effect abilities for turn start
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
                int delta = newCount - oldCount;

                // Trigger equipment abilities that react to hand changes
                // EquipmentAbility.OnHandCardsChange signature: (InGameInvocationCard invocationCard, PlayerCards playerCards, int delta)
                foreach (var invocation in concretePlayer.InvocationCards)
                {
                    if (invocation is InGameInvocationCard invocationCard &&
                        invocationCard.EquipmentCard != null)
                    {
                        foreach (var ability in invocationCard.EquipmentCard.EquipmentAbilities)
                        {
                            ability.OnHandCardsChange(invocationCard, concretePlayer, delta);
                        }
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
                target is InGameInvocationCard concreteTarget &&
                ownerCards is PlayerCards concreteOwner &&
                opponentCards is PlayerCards concreteOpponent)
            {
                // EquipmentAbility.ApplyEffect signature: (InGameInvocationCard invocationCard, PlayerCards playerCards, PlayerCards opponentPlayerCards)
                foreach (var ability in concreteEquipment.EquipmentAbilities)
                {
                    ability.ApplyEffect(concreteTarget, concreteOwner, concreteOpponent);
                }
            }
        }

        public void ExecuteOnEquipmentDetached(
            IInGameEquipmentCard equipment,
            IInGameInvocationCard previousTarget,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards)
        {
            if (equipment is InGameEquipmentCard concreteEquipment &&
                previousTarget is InGameInvocationCard concreteTarget &&
                ownerCards is PlayerCards concreteOwner &&
                opponentCards is PlayerCards concreteOpponent)
            {
                // EquipmentAbility.RemoveEffect signature: (InGameInvocationCard invocationCard, PlayerCards playerCards, PlayerCards opponentPlayerCards)
                foreach (var ability in concreteEquipment.EquipmentAbilities)
                {
                    ability.RemoveEffect(concreteTarget, concreteOwner, concreteOpponent);
                }
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
