using System;
using JDG.Domain;
using JDG.Domain.Enums;

namespace JDG.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for game state.
    /// Used to transfer current game state to the presentation layer.
    /// </summary>
    [Serializable]
    public class GameStateDTO
    {
        public Phase CurrentPhase { get; set; }
        public int TurnNumber { get; set; }
        public CardOwner CurrentPlayer { get; set; }
        public bool IsGameOver { get; set; }
        public CardOwner Winner { get; set; }
        public PlayerDTO Player1 { get; set; }
        public PlayerDTO Player2 { get; set; }
    }
}
