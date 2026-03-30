using System.Linq;
using UnityEngine;
using JDG.Domain;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;
using Cards.InvocationCards;
using Cards.EquipmentCards;
using Cards.FieldCards;
using Cards.EffectCards;
using _Scripts.Scriptables;

// Type aliases to avoid ambiguity
using LegacyCard = Cards.Card;
using DomainCard = JDG.Domain.Entities.Card;

namespace JDG.Bridge
{
    /// <summary>
    /// Converts old ScriptableObject cards to new domain Card entities.
    /// Bridges the legacy card system with the new Clean Architecture.
    /// Phase 166: Simplified - legacy and domain enums are now unified,
    /// so enum conversion methods are identity operations.
    /// </summary>
    public static class CardConverter
    {
        /// <summary>
        /// Converts a ScriptableObject Card to a domain Card entity.
        /// Automatically detects the card type and performs the appropriate conversion.
        /// </summary>
        public static DomainCard ConvertToDomain(LegacyCard scriptableCard)
        {
            if (scriptableCard == null)
                return null;

            // Generate a new CardId for this card definition
            var cardId = CardId.New();

            // Convert based on concrete type
            if (scriptableCard is InvocationCard invocationCard)
            {
                return ConvertInvocationCard(invocationCard, cardId);
            }
            else if (scriptableCard is EquipmentCard equipmentCard)
            {
                return ConvertEquipmentCard(equipmentCard, cardId);
            }
            else if (scriptableCard is FieldCard fieldCard)
            {
                return ConvertFieldCard(fieldCard, cardId);
            }
            else if (scriptableCard is EffectCard effectCard)
            {
                return ConvertEffectCard(effectCard, cardId);
            }
            else if (scriptableCard is ContreCard contreCard)
            {
                return ConvertContreCard(contreCard, cardId);
            }

            // Fallback for base Card type
            Debug.LogWarning($"Card '{scriptableCard.Title}' has unknown type, creating as Effect");
            return DomainCard.CreateEffect(
                cardId,
                scriptableCard.Title,
                scriptableCard.Description,
                scriptableCard.DetailedDescription,
                null,
                scriptableCard.Collector
            );
        }

        private static DomainCard ConvertInvocationCard(InvocationCard scriptableCard, CardId cardId)
        {
            // Phase 166: Families are already domain CardFamily[], no conversion needed
            var families = scriptableCard.BaseInvocationCardStats.Families
                ?? new CardFamily[0];

            // Phase 166: Abilities are already domain AbilityName, no conversion needed
            var abilities = scriptableCard.Abilities?.ToArray()
                ?? new AbilityName[0];

            // Phase 166: Conditions are already domain ConditionName, no conversion needed
            var conditions = scriptableCard.Conditions?.ToArray()
                ?? new ConditionName[0];

            return DomainCard.CreateInvocation(
                cardId,
                scriptableCard.Title,
                scriptableCard.Description,
                scriptableCard.DetailedDescription,
                scriptableCard.BaseInvocationCardStats.Attack,
                scriptableCard.BaseInvocationCardStats.Defense,
                families,
                scriptableCard.BaseInvocationCardStats.AffectedByEffect,
                conditions,
                abilities,
                scriptableCard.Collector
            );
        }

        private static DomainCard ConvertEquipmentCard(EquipmentCard scriptableCard, CardId cardId)
        {
            // Phase 166: Abilities are already domain EquipmentAbilityName, no conversion needed
            var abilities = scriptableCard.EquipmentAbilities?.ToArray()
                ?? new EquipmentAbilityName[0];

            return DomainCard.CreateEquipment(
                cardId,
                scriptableCard.Title,
                scriptableCard.Description,
                scriptableCard.DetailedDescription,
                abilities,
                scriptableCard.Collector
            );
        }

        private static DomainCard ConvertFieldCard(FieldCard scriptableCard, CardId cardId)
        {
            // Phase 166: Abilities are already domain FieldAbilityName, no conversion needed
            var abilities = scriptableCard.FieldAbilities?.ToArray()
                ?? new FieldAbilityName[0];

            return DomainCard.CreateField(
                cardId,
                scriptableCard.Title,
                scriptableCard.Description,
                scriptableCard.DetailedDescription,
                scriptableCard.Family,
                abilities,
                scriptableCard.Collector
            );
        }

        private static DomainCard ConvertEffectCard(EffectCard scriptableCard, CardId cardId)
        {
            // Phase 166: Abilities are already domain EffectAbilityName, no conversion needed
            var abilities = scriptableCard.EffectAbilities?.ToArray()
                ?? new EffectAbilityName[0];

            return DomainCard.CreateEffect(
                cardId,
                scriptableCard.Title,
                scriptableCard.Description,
                scriptableCard.DetailedDescription,
                abilities,
                scriptableCard.Collector
            );
        }

        private static DomainCard ConvertContreCard(ContreCard scriptableCard, CardId cardId)
        {
            return DomainCard.CreateContre(
                cardId,
                scriptableCard.Title,
                scriptableCard.Description,
                scriptableCard.DetailedDescription,
                scriptableCard.Collector
            );
        }
    }
}
