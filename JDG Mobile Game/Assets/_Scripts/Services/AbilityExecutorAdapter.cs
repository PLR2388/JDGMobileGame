using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using JDG.Application.Abilities;
using JDG.Application.Cards;
using JDG.Application.Services;
using UnityEngine;

namespace Services
{
    /// <summary>
    /// Adapter bridging IAbilityExecutor to legacy Ability classes.
    /// Phase 74: Created as part of UseCase migration.
    ///
    /// This adapter allows use cases to trigger abilities without coupling
    /// to the legacy Ability class. It wraps the legacy ability execution
    /// and provides a clean interface.
    ///
    /// As abilities are migrated to pure domain implementations,
    /// this adapter will delegate to new ability use cases instead.
    /// </summary>
    public class AbilityExecutorAdapter : IAbilityExecutor
    {
        private readonly ICanvasProvider _canvasProvider;

        public AbilityExecutorAdapter(ICanvasProvider canvasProvider)
        {
            _canvasProvider = canvasProvider;
        }

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

                foreach (var ability in concreteDeadCard.Abilities)
                {
                    ability.OnCardDeath(canvas, concreteDeadCard, concreteOwner, concreteOpponent);
                }
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
                }

                // 2. Trigger existing invocation card abilities on same field
                foreach (var existingCard in concreteOwner.InvocationCards)
                {
                    foreach (var ability in existingCard.Abilities)
                    {
                        ability.OnCardAdded(concreteAdded, concreteOwner);
                    }
                }

                // 3. Trigger effect card abilities
                foreach (var effectCard in concreteOwner.EffectCards)
                {
                    foreach (var effectAbility in effectCard.EffectAbilities)
                    {
                        effectAbility.OnInvocationCardAdded(concreteOwner, concreteAdded);
                    }
                }

                // 4. Trigger field card abilities
                if (concreteOwner.FieldCard?.FieldAbilities != null)
                {
                    foreach (var fieldAbility in concreteOwner.FieldCard.FieldAbilities)
                    {
                        fieldAbility.OnInvocationCardAdded(concreteAdded, concreteOwner);
                    }
                }
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
                }

                // 2. Trigger effect abilities that react to invocation removal
                // EffectAbility.OnInvocationCardRemoved signature: (PlayerCards playerCards, InGameInvocationCard invocationCard)
                foreach (var effectCard in concreteOwner.EffectCards)
                {
                    foreach (var ability in effectCard.EffectAbilities)
                    {
                        ability.OnInvocationCardRemoved(concreteOwner, concreteRemoved);
                    }
                }
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
                        foreach (var ability in invocationCard.Abilities)
                        {
                            ability.OnTurnStart(canvas, concretePlayer, concreteOpponent);
                        }
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
            // This method is a no-op for now.
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
    }
}
