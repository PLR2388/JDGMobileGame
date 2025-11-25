using System;
using JDG.Domain.Enums;

namespace JDG.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for player state.
    /// Used to transfer player data to the presentation layer.
    /// </summary>
    [Serializable]
    public class PlayerDTO
    {
        public int PlayerId { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Shields { get; set; }
        public bool BlockAttack { get; set; }
        public int DeckCount { get; set; }
        public int HandCount { get; set; }
        public int FieldCount { get; set; }
        public int GraveyardCount { get; set; }
        public bool IsDefeated { get; set; }
        public CardOwner Owner { get; set; }
    }
}
