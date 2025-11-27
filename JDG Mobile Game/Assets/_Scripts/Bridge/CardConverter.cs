using System.Linq;
using UnityEngine;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;
using Cards;
using Cards.InvocationCards;
using Cards.EquipmentCards;
using Cards.FieldCards;
using Cards.EffectCards;
using _Scripts.Scriptables;

namespace JDG.Infrastructure.Bridge
{
    /// <summary>
    /// Converts old ScriptableObject cards to new domain Card entities.
    /// Bridges the legacy card system with the new Clean Architecture.
    /// </summary>
    public static class CardConverter
    {
        /// <summary>
        /// Converts a ScriptableObject Card to a domain Card entity.
        /// Automatically detects the card type and performs the appropriate conversion.
        /// </summary>
        public static Card ConvertToDomain(Cards.Card scriptableCard)
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
            return Card.CreateEffect(
                cardId,
                scriptableCard.Title,
                scriptableCard.Description,
                scriptableCard.DetailedDescription,
                null,
                scriptableCard.Collector
            );
        }

        private static Card ConvertInvocationCard(InvocationCard scriptableCard, CardId cardId)
        {
            // Convert old CardFamily[] to new domain CardFamily enum
            var families = scriptableCard.BaseInvocationCardStats.Families
                ?.Select(ConvertFamily)
                .ToArray() ?? new CardFamily[0];

            // Convert old AbilityName to new domain AbilityName
            var abilities = scriptableCard.Abilities
                ?.Select(ConvertAbilityName)
                .ToArray() ?? new AbilityName[0];

            // Convert old ConditionName to new domain ConditionName
            var conditions = scriptableCard.Conditions
                ?.Select(ConvertConditionName)
                .ToArray() ?? new ConditionName[0];

            return Card.CreateInvocation(
                cardId,
                scriptableCard.Title,
                scriptableCard.Description,
                scriptableCard.DetailedDescription,
                (int)scriptableCard.BaseInvocationCardStats.Attack,
                (int)scriptableCard.BaseInvocationCardStats.Defense,
                families,
                scriptableCard.BaseInvocationCardStats.AffectedByEffect,
                conditions,
                abilities,
                scriptableCard.Collector
            );
        }

        private static Card ConvertEquipmentCard(EquipmentCard scriptableCard, CardId cardId)
        {
            // Convert old EquipmentAbilityName to new domain EquipmentAbilityName
            var abilities = scriptableCard.EquipmentAbilities
                ?.Select(ConvertEquipmentAbilityName)
                .ToArray() ?? new EquipmentAbilityName[0];

            return Card.CreateEquipment(
                cardId,
                scriptableCard.Title,
                scriptableCard.Description,
                scriptableCard.DetailedDescription,
                abilities,
                scriptableCard.Collector
            );
        }

        private static Card ConvertFieldCard(FieldCard scriptableCard, CardId cardId)
        {
            // Convert old FieldAbilityName to new domain FieldAbilityName
            var abilities = scriptableCard.FieldAbilities
                ?.Select(ConvertFieldAbilityName)
                .ToArray() ?? new FieldAbilityName[0];

            return Card.CreateField(
                cardId,
                scriptableCard.Title,
                scriptableCard.Description,
                scriptableCard.DetailedDescription,
                ConvertFamily(scriptableCard.Family),
                abilities,
                scriptableCard.Collector
            );
        }

        private static Card ConvertEffectCard(EffectCard scriptableCard, CardId cardId)
        {
            // Convert old EffectAbilityName to new domain EffectAbilityName
            var abilities = scriptableCard.EffectAbilities
                ?.Select(ConvertEffectAbilityName)
                .ToArray() ?? new EffectAbilityName[0];

            return Card.CreateEffect(
                cardId,
                scriptableCard.Title,
                scriptableCard.Description,
                scriptableCard.DetailedDescription,
                abilities,
                scriptableCard.Collector
            );
        }

        private static Card ConvertContreCard(ContreCard scriptableCard, CardId cardId)
        {
            return Card.CreateContre(
                cardId,
                scriptableCard.Title,
                scriptableCard.Description,
                scriptableCard.DetailedDescription,
                scriptableCard.Collector
            );
        }

        // Enum conversion methods - these convert from old to new namespace

        private static CardFamily ConvertFamily(Cards.CardFamily oldFamily)
        {
            // The enums have the same names, so we can parse
            return (CardFamily)System.Enum.Parse(typeof(CardFamily), oldFamily.ToString());
        }

        private static AbilityName ConvertAbilityName(global::AbilityName oldAbility)
        {
            // The enums have the same names, so we can parse
            return (AbilityName)System.Enum.Parse(typeof(AbilityName), oldAbility.ToString());
        }

        private static ConditionName ConvertConditionName(global::ConditionName oldCondition)
        {
            // The enums have the same names, so we can parse
            return (ConditionName)System.Enum.Parse(typeof(ConditionName), oldCondition.ToString());
        }

        private static EquipmentAbilityName ConvertEquipmentAbilityName(global::EquipmentAbilityName oldAbility)
        {
            // The enums have the same names, so we can parse
            return (EquipmentAbilityName)System.Enum.Parse(typeof(EquipmentAbilityName), oldAbility.ToString());
        }

        private static FieldAbilityName ConvertFieldAbilityName(global::FieldAbilityName oldAbility)
        {
            // The enums have the same names, so we can parse
            return (FieldAbilityName)System.Enum.Parse(typeof(FieldAbilityName), oldAbility.ToString());
        }

        private static EffectAbilityName ConvertEffectAbilityName(global::EffectAbilityName oldAbility)
        {
            // The enums have the same names, so we can parse
            return (EffectAbilityName)System.Enum.Parse(typeof(EffectAbilityName), oldAbility.ToString());
        }
    }
}
