using System.Linq;
using JDG.Application.DTOs;
using JDG.Domain.Entities;

namespace JDG.Application.Mappers
{
    /// <summary>
    /// Maps between Player entity and PlayerDTO.
    /// </summary>
    public static class PlayerMapper
    {
        /// <summary>
        /// Converts a Player entity to a PlayerDTO.
        /// </summary>
        public static PlayerDTO ToDTO(this Player player)
        {
            if (player == null)
                return null;

            return new PlayerDTO
            {
                PlayerId = player.Id.ToInt(),
                Health = player.Health,
                MaxHealth = player.MaxHealth,
                Shields = player.Shields,
                BlockAttack = player.BlockAttack,
                DeckCount = player.DeckCount,
                HandCount = player.HandCount,
                FieldCount = player.FieldCount,
                GraveyardCount = player.GraveyardCount,
                IsDefeated = player.IsDefeated,
                Owner = player.Id.ToCardOwner()
            };
        }
    }
}
