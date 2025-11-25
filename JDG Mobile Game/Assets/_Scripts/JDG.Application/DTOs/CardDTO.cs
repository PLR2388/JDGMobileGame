using System;
using JDG.Domain;
using JDG.Domain.Enums;

namespace JDG.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for card data.
    /// Used to transfer card information to the presentation layer.
    /// </summary>
    [Serializable]
    public class CardDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DetailedDescription { get; set; }
        public CardType Type { get; set; }
        public CardOwner Owner { get; set; }
        public bool IsCollector { get; set; }

        // Invocation card properties
        public int Attack { get; set; }
        public int Defense { get; set; }
        public CardFamily[] Families { get; set; }
        public bool AffectedByEffect { get; set; }
        public ConditionName[] Conditions { get; set; }
        public AbilityName[] Abilities { get; set; }

        // Equipment card properties
        public EquipmentAbilityName[] EquipmentAbilities { get; set; }

        // Field card properties
        public CardFamily? FieldFamily { get; set; }
        public FieldAbilityName[] FieldAbilities { get; set; }

        // Effect card properties
        public EffectAbilityName[] EffectAbilities { get; set; }

        // Runtime state
        public bool IsDestroyed { get; set; }
        public bool IsInHand { get; set; }
        public bool IsOnField { get; set; }
        public bool IsInGraveyard { get; set; }
    }
}
