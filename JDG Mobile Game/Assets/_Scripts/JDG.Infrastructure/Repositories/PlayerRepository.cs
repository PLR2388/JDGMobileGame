using System.Collections.Generic;
using System.Linq;
using JDG.Application.Repositories;
using JDG.Domain.Entities;
using JDG.Domain.ValueObjects;

namespace JDG.Infrastructure.Repositories
{
    /// <summary>
    /// Infrastructure implementation of IPlayerRepository.
    /// Manages player entities in memory.
    /// </summary>
    public class PlayerRepository : IPlayerRepository
    {
        private readonly Dictionary<PlayerId, Player> _players = new Dictionary<PlayerId, Player>();
        private readonly ICardRepository _cardRepository;

        public PlayerRepository(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public Player GetPlayer(PlayerId playerId)
        {
            return _players.ContainsKey(playerId) ? _players[playerId] : null;
        }

        public void SavePlayer(Player player)
        {
            if (player == null)
                return;

            _players[player.Id] = player;
        }

        public Player CreatePlayer(PlayerId playerId, CardId[] deckCardIds, int maxHealth = 30)
        {
            // Convert CardIds to Card instances
            var deckCards = deckCardIds
                .Select(cardId => _cardRepository.GetCard(cardId))
                .Where(card => card != null)
                .ToList();

            if (deckCards.Count == 0)
                return null;

            var player = new Player(playerId, deckCards, maxHealth);
            _players[playerId] = player;

            return player;
        }

        public void ResetPlayer(PlayerId playerId)
        {
            if (_players.ContainsKey(playerId))
            {
                _players.Remove(playerId);
            }
        }
    }
}
