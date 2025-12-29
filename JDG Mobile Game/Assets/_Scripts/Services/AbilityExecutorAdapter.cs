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
                var canvas = _canvasProvider.GetGameCanvas() as Transform;

                // Trigger abilities on the added card
                foreach (var ability in concreteAdded.Abilities)
                {
                    ability.OnCardAdded(canvas, concreteAdded, concreteOwner, concreteOpponent);
                }

                // Trigger abilities on existing cards that react to new cards
                foreach (var invocation in concreteOwner.InvocationCards)
                {
                    if (invocation is InGameInvocationCard existingCard && existingCard != concreteAdded)
                    {
                        foreach (var ability in existingCard.Abilities)
                        {
                            ability.OnOtherCardAdded(canvas, concreteAdded, existingCard, concreteOwner, concreteOpponent);
                        }
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
                ownerCards is PlayerCards concreteOwner &&
                opponentCards is PlayerCards concreteOpponent)
            {
                var canvas = _canvasProvider.GetGameCanvas() as Transform;

                // Trigger OnCardRemove on the removed card's abilities
                foreach (var ability in concreteRemoved.Abilities)
                {
                    ability.OnCardRemove(canvas, concreteRemoved, concreteOwner, concreteOpponent);
                }

                // Trigger effect abilities that react to invocation removal
                foreach (var effectCard in concreteOwner.EffectCards)
                {
                    foreach (var ability in effectCard.EffectAbilities)
                    {
                        ability.OnInvocationCardRemoved(canvas, concreteRemoved, concreteOwner, concreteOpponent);
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
            if (ownerCards is PlayerCards concreteOwner &&
                opponentCards is PlayerCards concreteOpponent)
            {
                var canvas = _canvasProvider.GetGameCanvas() as Transform;

                // Trigger OnFieldCardRemoved on old field card's abilities
                if (oldFieldCard is InGameFieldCard concreteOldField)
                {
                    foreach (var ability in concreteOldField.FieldAbilities)
                    {
                        ability.OnFieldCardRemoved(canvas, concreteOldField, concreteOwner, concreteOpponent);
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

                // Execute field card turn start abilities
                if (concretePlayer.FieldCard is InGameFieldCard fieldCard)
                {
                    foreach (var ability in fieldCard.FieldAbilities)
                    {
                        ability.OnTurnStart(canvas, concretePlayer, concreteOpponent);
                    }
                }

                // Execute invocation card turn start abilities
                foreach (var invocation in concretePlayer.InvocationCards)
                {
                    if (invocation is InGameInvocationCard invocationCard)
                    {
                        foreach (var ability in invocationCard.Abilities)
                        {
                            ability.OnTurnStart(canvas, invocationCard, concretePlayer, concreteOpponent);
                        }
                    }
                }
            }
        }

        public void ExecuteOnTurnEnd(
            IPlayerCardCollection currentPlayerCards,
            IPlayerCardCollection opponentCards)
        {
            if (currentPlayerCards is PlayerCards concretePlayer &&
                opponentCards is PlayerCards concreteOpponent)
            {
                var canvas = _canvasProvider.GetGameCanvas() as Transform;

                // Execute invocation card turn end abilities
                foreach (var invocation in concretePlayer.InvocationCards)
                {
                    if (invocation is InGameInvocationCard invocationCard)
                    {
                        foreach (var ability in invocationCard.Abilities)
                        {
                            ability.OnTurnEnd(canvas, invocationCard, concretePlayer, concreteOpponent);
                        }
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
            if (playerCards is PlayerCards concretePlayer &&
                opponentCards is PlayerCards concreteOpponent)
            {
                var canvas = _canvasProvider.GetGameCanvas() as Transform;

                // Trigger equipment abilities that react to hand changes
                foreach (var invocation in concretePlayer.InvocationCards)
                {
                    if (invocation is InGameInvocationCard invocationCard &&
                        invocationCard.EquipmentCard != null)
                    {
                        foreach (var ability in invocationCard.EquipmentCard.EquipmentAbilities)
                        {
                            ability.OnHandCardsChange(canvas, invocationCard, concretePlayer, concreteOpponent);
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
                var canvas = _canvasProvider.GetGameCanvas() as Transform;

                foreach (var ability in concreteEquipment.EquipmentAbilities)
                {
                    ability.OnEquip(canvas, concreteTarget, concreteOwner, concreteOpponent);
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
                var canvas = _canvasProvider.GetGameCanvas() as Transform;

                foreach (var ability in concreteEquipment.EquipmentAbilities)
                {
                    ability.OnUnequip(canvas, concreteTarget, concreteOwner, concreteOpponent);
                }
            }
        }

        #endregion
    }
}
