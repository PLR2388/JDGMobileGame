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
using LegacyCardFamily = Cards.CardFamily;
using DomainCardFamily = JDG.Domain.Enums.CardFamily;
using DomainAbilityName = JDG.Domain.AbilityName;
using DomainConditionName = JDG.Domain.Enums.ConditionName;
using DomainEquipmentAbilityName = JDG.Domain.Enums.EquipmentAbilityName;
using DomainFieldAbilityName = JDG.Domain.Enums.FieldAbilityName;
using DomainEffectAbilityName = JDG.Domain.Enums.EffectAbilityName;

namespace JDG.Bridge
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
            // Convert old CardFamily[] to new domain CardFamily enum
            var families = scriptableCard.BaseInvocationCardStats.Families
                ?.Select(ConvertFamily)
                .ToArray() ?? new DomainCardFamily[0];

            // Convert old AbilityName to new domain AbilityName
            var abilities = scriptableCard.Abilities
                ?.Select(ConvertAbilityName)
                .ToArray() ?? new DomainAbilityName[0];

            // Convert old ConditionName to new domain ConditionName
            var conditions = scriptableCard.Conditions
                ?.Select(ConvertConditionName)
                .ToArray() ?? new DomainConditionName[0];

            return DomainCard.CreateInvocation(
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

        private static DomainCard ConvertEquipmentCard(EquipmentCard scriptableCard, CardId cardId)
        {
            // Convert old EquipmentAbilityName to new domain EquipmentAbilityName
            var abilities = scriptableCard.EquipmentAbilities
                ?.Select(ConvertEquipmentAbilityName)
                .ToArray() ?? new DomainEquipmentAbilityName[0];

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
            // Convert old FieldAbilityName to new domain FieldAbilityName
            var abilities = scriptableCard.FieldAbilities
                ?.Select(ConvertFieldAbilityName)
                .ToArray() ?? new DomainFieldAbilityName[0];

            return DomainCard.CreateField(
                cardId,
                scriptableCard.Title,
                scriptableCard.Description,
                scriptableCard.DetailedDescription,
                ConvertFamily(scriptableCard.Family),
                abilities,
                scriptableCard.Collector
            );
        }

        private static DomainCard ConvertEffectCard(EffectCard scriptableCard, CardId cardId)
        {
            // Convert old EffectAbilityName to new domain EffectAbilityName
            var abilities = scriptableCard.EffectAbilities
                ?.Select(ConvertEffectAbilityName)
                .ToArray() ?? new DomainEffectAbilityName[0];

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

        // Enum conversion methods - these convert from old to new namespace

        private static DomainCardFamily ConvertFamily(LegacyCardFamily oldFamily)
        {
            // The enums have the same names, so we can parse
            return (DomainCardFamily)System.Enum.Parse(typeof(DomainCardFamily), oldFamily.ToString());
        }

        private static DomainAbilityName ConvertAbilityName(global::AbilityName oldAbility)
        {
            // The enums have the same names, so we can parse
            return (DomainAbilityName)System.Enum.Parse(typeof(DomainAbilityName), oldAbility.ToString());
        }

        private static DomainConditionName ConvertConditionName(global::ConditionName oldCondition)
        {
            // The enums have the same names, so we can parse
            return (DomainConditionName)System.Enum.Parse(typeof(DomainConditionName), oldCondition.ToString());
        }

        private static DomainEquipmentAbilityName ConvertEquipmentAbilityName(global::EquipmentAbilityName oldAbility)
        {
            // The enums have the same names, so we can parse
            return (DomainEquipmentAbilityName)System.Enum.Parse(typeof(DomainEquipmentAbilityName), oldAbility.ToString());
        }

        private static DomainFieldAbilityName ConvertFieldAbilityName(global::FieldAbilityName oldAbility)
        {
            // The enums have the same names, so we can parse
            return (DomainFieldAbilityName)System.Enum.Parse(typeof(DomainFieldAbilityName), oldAbility.ToString());
        }

        private static DomainEffectAbilityName ConvertEffectAbilityName(global::EffectAbilityName oldAbility)
        {
            // The enums have the same names, so we can parse
            return (DomainEffectAbilityName)System.Enum.Parse(typeof(DomainEffectAbilityName), oldAbility.ToString());
        }
    }
}
