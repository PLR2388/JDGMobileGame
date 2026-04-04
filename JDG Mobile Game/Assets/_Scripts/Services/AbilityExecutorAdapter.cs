using System.Linq;
using Cards;
using JDG.Application.Abilities;
using JDG.Application.Cards;
using JDG.Application.Services;
using JDG.Domain;
using JDG.Domain.Enums;
using DomainCard = JDG.Domain.Entities.Card;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Cards;
using UnityEngine;

namespace Services
{
    /// <summary>
    /// Adapter for IAbilityExecutor to execute modern IAbility implementations.
    /// Phase 74: Created as part of UseCase migration.
    /// Phase 106: Updated to also execute modern IAbility implementations.
    /// Phase 118: Removed all legacy Ability references, uses only ModernAbilities.
    /// Phase 141: Added ICardSyncService to sync domain Card changes to InGameInvocationCard.
    /// Phase 166: Uses domain enums directly (legacy enum aliases removed).
    /// </summary>
    public class AbilityExecutorAdapter : IAbilityExecutor
    {
        private readonly ICanvasProvider _canvasProvider;
        private readonly ICardSyncService _cardSyncService;

        public AbilityExecutorAdapter(ICanvasProvider canvasProvider, ICardSyncService cardSyncService)
        {
            _canvasProvider = canvasProvider ?? throw new System.ArgumentNullException(nameof(canvasProvider));
            _cardSyncService = cardSyncService ?? throw new System.ArgumentNullException(nameof(cardSyncService));
        }

        #region Modern Ability Helpers

        /// <summary>
        /// Creates an AbilityContext for modern ability execution.
        /// Phase 166: Simplified - CardOwner is now domain type directly.
        /// </summary>
        private AbilityContext CreateAbilityContext(
            InGameCard sourceCard,
            CardOwner owner,
            AbilityName abilityName = AbilityName.Default)
        {
            var ownerId = PlayerId.FromCardOwner(owner);
            var opponentOwner = owner == CardOwner.Player1 ? CardOwner.Player2 : CardOwner.Player1;
            var opponentId = PlayerId.FromCardOwner(opponentOwner);

            var domainSourceCard = ConvertToDomainCard(sourceCard);

            return new AbilityContext(ownerId, opponentId, domainSourceCard, abilityName);
        }

        /// <summary>
        /// Converts an InGameCard to a domain Card for use in AbilityContext.SourceCard.
        /// Phase 166: Simplified - Families are already domain CardFamily[], no conversion needed.
        /// </summary>
        private DomainCard ConvertToDomainCard(InGameCard sourceCard)
        {
            if (sourceCard == null) return null;

            if (sourceCard is InGameInvocationCard invocation)
            {
                // Phase 166: Families are already domain CardFamily[]
                var domainFamilies = invocation.Families?
                    .ToList() ?? new System.Collections.Generic.List<CardFamily>();

                return DomainCard.CreateInvocation(
                    CardId.New(),
                    invocation.Title ?? "Unknown",
                    invocation.GetDescription() ?? "",
                    invocation.GetDetailedDescription() ?? "",
                    invocation.Attack,
                    invocation.Defense,
                    domainFamilies,
                    invocation.IsAffectedByEffectCard
                );
            }
            else if (sourceCard is InGameEquipmentCard equipment)
            {
                return DomainCard.CreateEquipment(
                    CardId.New(),
                    equipment.Title ?? "Unknown",
                    equipment.GetDescription() ?? "",
                    equipment.GetDetailedDescription() ?? "",
                    System.Linq.Enumerable.Empty<EquipmentAbilityName>()
                );
            }
            else if (sourceCard is InGameFieldCard fieldCard)
            {
                // Phase 166: Family is already domain CardFamily
                return DomainCard.CreateField(
                    CardId.New(),
                    fieldCard.Title ?? "Unknown",
                    fieldCard.GetDescription() ?? "",
                    fieldCard.GetDetailedDescription() ?? "",
                    fieldCard.Family,
                    System.Linq.Enumerable.Empty<FieldAbilityName>()
                );
            }
            else if (sourceCard is InGameEffectCard effectCard)
            {
                return DomainCard.CreateEffect(
                    CardId.New(),
                    effectCard.Title ?? "Unknown",
                    effectCard.GetDescription() ?? "",
                    effectCard.GetDetailedDescription() ?? "",
                    System.Linq.Enumerable.Empty<EffectAbilityName>()
                );
            }

            Debug.LogWarning($"[AbilityExecutorAdapter] ConvertToDomainCard: Unknown card type {sourceCard.GetType().Name}");
            return null;
        }

        /// <summary>
        /// Executes modern abilities that match the specified trigger.
        /// </summary>
        private void ExecuteModernAbilities(
            System.Collections.Generic.IEnumerable<IAbility> abilities,
            AbilityTrigger trigger,
            AbilityContext context)
        {
            foreach (var ability in abilities)
            {
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
            if (deadCard is InGameInvocationCard concreteDeadCard)
            {
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
                foreach (var opponentCard in concreteOpponent.InvocationCards ?? System.Linq.Enumerable.Empty<InGameInvocationCard>())
                {
                    var equipmentCard = opponentCard.EquipmentCard;
                    if (equipmentCard == null) continue;

                    var equipContext = CreateAbilityContext(equipmentCard, opponentCard.CardOwner);
                    equipContext.TargetCard = _cardSyncService.CreateLinkedCard(addedCard);
                    if (equipContext.TargetCard == null) continue;

                    ExecuteModernAbilities(equipmentCard.ModernEquipmentAbilities, AbilityTrigger.OnCardPlayed, equipContext);

                    _cardSyncService.SyncCardState(equipContext.TargetCard);
                    _cardSyncService.ClearMapping(equipContext.TargetCard);
                }

                // 2. Trigger existing invocation card abilities on same field
                foreach (var existingCard in concreteOwner.InvocationCards ?? System.Linq.Enumerable.Empty<InGameInvocationCard>())
                {
                    var invocContext = CreateAbilityContext(existingCard, existingCard.CardOwner);
                    ExecuteModernAbilities(existingCard.ModernAbilities, AbilityTrigger.OnCardPlayed, invocContext);
                }

                // 3. Trigger effect card abilities
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
                var ownerPlayerId = PlayerId.FromCardOwner(concreteAdded.CardOwner);
                _cardSyncService.SyncAllFieldCards(ownerPlayerId, concreteOwner);
            }
        }

        public void ExecuteOnCardRemovedFromField(
            IInGameInvocationCard removedCard,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards)
        {
            // Phase 118: Modern abilities don't have a specific OnCardRemoved trigger yet
        }

        public void ExecuteOnFieldCardChanged(
            IInGameFieldCard oldFieldCard,
            IInGameFieldCard newFieldCard,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards)
        {
            if (oldFieldCard is InGameFieldCard concreteOldField)
            {
                var context = CreateAbilityContext(concreteOldField, concreteOldField.CardOwner);
            }
        }

        #endregion

        #region Turn Triggers

        public void ExecuteOnTurnStart(
            IPlayerCardCollection currentPlayerCards,
            IPlayerCardCollection opponentCards)
        {
            if (currentPlayerCards is PlayerCards concretePlayer)
            {
                foreach (var invocation in concretePlayer.InvocationCards ?? System.Linq.Enumerable.Empty<InGameInvocationCard>())
                {
                    if (invocation is InGameInvocationCard invocationCard)
                    {
                        var context = CreateAbilityContext(invocationCard, invocationCard.CardOwner);
                        ExecuteModernAbilities(invocationCard.ModernAbilities, AbilityTrigger.OnTurnStart, context);
                    }
                }

                if (concretePlayer.FieldCard != null)
                {
                    var fieldContext = CreateAbilityContext(concretePlayer.FieldCard, concretePlayer.FieldCard.CardOwner);
                    ExecuteModernAbilities(concretePlayer.FieldCard.ModernFieldAbilities, AbilityTrigger.OnTurnStart, fieldContext);
                }

                foreach (var effectCard in concretePlayer.EffectCards ?? System.Linq.Enumerable.Empty<InGameEffectCard>())
                {
                    if (effectCard is InGameEffectCard concreteEffect)
                    {
                        var effectContext = CreateAbilityContext(concreteEffect, concreteEffect.CardOwner);
                        ExecuteModernAbilities(concreteEffect.ModernEffectAbilities, AbilityTrigger.OnTurnStart, effectContext);
                    }
                }

                var playerId = concretePlayer.IsPlayerOne ? PlayerId.Player1 : PlayerId.Player2;
                _cardSyncService.SyncAllFieldCards(playerId, concretePlayer);
            }
        }

        public void ExecuteOnTurnEnd(
            IPlayerCardCollection currentPlayerCards,
            IPlayerCardCollection opponentCards)
        {
            if (currentPlayerCards is PlayerCards concretePlayer)
            {
                foreach (var invocation in concretePlayer.InvocationCards ?? System.Linq.Enumerable.Empty<InGameInvocationCard>())
                {
                    if (invocation is InGameInvocationCard invocationCard)
                    {
                        var context = CreateAbilityContext(invocationCard, invocationCard.CardOwner);
                        ExecuteModernAbilities(invocationCard.ModernAbilities, AbilityTrigger.OnTurnEnd, context);
                    }
                }

                if (concretePlayer.FieldCard != null)
                {
                    var fieldContext = CreateAbilityContext(concretePlayer.FieldCard, concretePlayer.FieldCard.CardOwner);
                    ExecuteModernAbilities(concretePlayer.FieldCard.ModernFieldAbilities, AbilityTrigger.OnTurnEnd, fieldContext);
                }

                foreach (var effectCard in concretePlayer.EffectCards ?? System.Linq.Enumerable.Empty<InGameEffectCard>())
                {
                    if (effectCard is InGameEffectCard concreteEffect)
                    {
                        var effectContext = CreateAbilityContext(concreteEffect, concreteEffect.CardOwner);
                        ExecuteModernAbilities(concreteEffect.ModernEffectAbilities, AbilityTrigger.OnTurnEnd, effectContext);
                    }
                }

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
                foreach (var invocation in concretePlayer.InvocationCards ?? System.Linq.Enumerable.Empty<InGameInvocationCard>())
                {
                    if (invocation is InGameInvocationCard invocationCard &&
                        invocationCard.EquipmentCard != null)
                    {
                        var context = CreateAbilityContext(invocationCard.EquipmentCard, invocationCard.CardOwner);
                        context.TargetCard = _cardSyncService.CreateLinkedCard(invocationCard);
                        if (context.TargetCard == null) continue;

                        ExecuteModernAbilities(invocationCard.EquipmentCard.ModernEquipmentAbilities, AbilityTrigger.OnHandChange, context);

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
                var context = CreateAbilityContext(concreteEquipment, concreteTarget.CardOwner);
                context.TargetCard = _cardSyncService.CreateLinkedCard(target);
                if (context.TargetCard == null) return;

                ExecuteModernAbilities(concreteEquipment.ModernEquipmentAbilities, AbilityTrigger.OnEquip, context);

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
                var context = CreateAbilityContext(concreteEquipment, concreteTarget.CardOwner);
                context.TargetCard = _cardSyncService.CreateLinkedCard(previousTarget);
                if (context.TargetCard == null) return;

                ExecuteModernAbilities(concreteEquipment.ModernEquipmentAbilities, AbilityTrigger.OnUnequip, context);

                _cardSyncService.SyncCardState(context.TargetCard);
                _cardSyncService.ClearMapping(context.TargetCard);
            }
        }

        #endregion

        #region Effect Card Triggers

        public void ExecuteOnEffectCardPlayed(
            IInGameEffectCard effectCard,
            IPlayerCardCollection ownerCards,
            IPlayerCardCollection opponentCards)
        {
            if (effectCard is InGameEffectCard concreteEffect &&
                ownerCards is PlayerCards concreteOwner)
            {
                var context = CreateAbilityContext(concreteEffect, concreteEffect.CardOwner);

                foreach (var ability in concreteEffect.ModernEffectAbilities)
                {
                    if (ability.CanActivate(context))
                    {
                        var result = ability.Execute(context);
                        if (!result.IsSuccess && !string.IsNullOrEmpty(result.Message))
                        {
                            Debug.LogWarning($"[AbilityExecutorAdapter] Effect ability failed: {result.Message}");
                        }
                    }
                }

                var playerId = PlayerId.FromCardOwner(concreteEffect.CardOwner);
                _cardSyncService.SyncAllFieldCards(playerId, concreteOwner);
            }
        }

        #endregion
    }
}
