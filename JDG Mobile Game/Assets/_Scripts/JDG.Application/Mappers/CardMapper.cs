using System.Linq;
using JDG.Application.DTOs;
using JDG.Domain.Entities;

namespace JDG.Application.Mappers
{
    /// <summary>
    /// Maps between Card entity and CardDTO.
    /// </summary>
    public static class CardMapper
    {
        /// <summary>
        /// Converts a Card entity to a CardDTO.
        /// </summary>
        public static CardDTO ToDTO(this Card card, bool isInHand = false, bool isOnField = false, bool isInGraveyard = false)
        {
            if (card == null)
                return null;

            return new CardDTO
            {
                Id = card.Id.ToGuid(),
                Title = card.Title,
                Description = card.Description,
                DetailedDescription = card.DetailedDescription,
                Type = card.Type,
                Owner = card.Owner,
                IsCollector = card.IsCollector,

                // Invocation properties
                Attack = card.Stats?.Attack ?? 0,
                Defense = card.Stats?.Defense ?? 0,
                Families = card.Families.ToArray(),
                AffectedByEffect = card.AffectedByEffect,
                Conditions = card.Conditions.ToArray(),
                Abilities = card.Abilities.ToArray(),

                // Equipment properties
                EquipmentAbilities = card.EquipmentAbilities.ToArray(),

                // Field properties
                FieldFamily = card.FieldFamily,
                FieldAbilities = card.FieldAbilities.ToArray(),

                // Effect properties
                EffectAbilities = card.EffectAbilities.ToArray(),

                // Runtime state
                IsDestroyed = card.IsDestroyed,
                IsInHand = isInHand,
                IsOnField = isOnField,
                IsInGraveyard = isInGraveyard
            };
        }
    }
}
