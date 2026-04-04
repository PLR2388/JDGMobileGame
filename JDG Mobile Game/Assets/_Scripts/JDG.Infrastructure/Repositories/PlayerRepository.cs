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
            // First try to get existing instances, then try to get from definitions
            var deckCards = new List<Card>();

            foreach (var cardId in deckCardIds)
            {
                var card = _cardRepository.GetCard(cardId);

                // If not found as instance, try to find in definitions and create instance
                if (card == null)
                {
                    // Try to find by matching the CardId in all definitions
                    var definitions = _cardRepository.GetAllCardDefinitions();
                    var definition = definitions.FirstOrDefault(d => d.Id == cardId);

                    if (definition != null)
                    {
                        // Create a new instance from the definition
                        card = _cardRepository.CreateCardInstance(definition.Title);
                    }
                }

                if (card != null)
                {
                    deckCards.Add(card);
                }
            }

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
