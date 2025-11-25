using System.Linq;
using JDG.Application.Repositories;
using JDG.Domain.Entities;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;

namespace JDG.Application.UseCases
{
    /// <summary>
    /// Use case for playing a card from hand to field.
    /// Publishes CardPlayedEvent on success.
    /// </summary>
    public class PlayCardUseCase
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IEventBus _eventBus;

        public PlayCardUseCase(
            IPlayerRepository playerRepository,
            ICardRepository cardRepository,
            IEventBus eventBus)
        {
            _playerRepository = playerRepository;
            _cardRepository = cardRepository;
            _eventBus = eventBus;
        }

        /// <summary>
        /// Executes the play card use case.
        /// </summary>
        public PlayCardResult Execute(PlayerId playerId, CardId cardId)
        {
            var player = _playerRepository.GetPlayer(playerId);
            if (player == null)
                return PlayCardResult.Failure("Player not found");

            var card = _cardRepository.GetCard(cardId);
            if (card == null)
                return PlayCardResult.Failure("Card not found");

            // Check if card is in hand
            if (!player.Hand.Contains(card))
                return PlayCardResult.Failure("Card is not in hand");

            // Play the card
            var success = player.PlayCard(card);
            if (!success)
                return PlayCardResult.Failure("Failed to play card");

            // Set card owner
            card.SetOwner(playerId.ToCardOwner());

            // Save state
            _playerRepository.SavePlayer(player);

            // Publish event
            _eventBus.Publish(new CardPlayedEvent
            {
                Owner = playerId.ToCardOwner(),
                CardId = cardId.ToGuid(),
                CardType = card.Type,
                CardTitle = card.Title,
                HandCount = player.HandCount,
                FieldCount = player.FieldCount
            });

            return PlayCardResult.Success(card);
        }
    }

    /// <summary>
    /// Result of a play card operation.
    /// </summary>
    public class PlayCardResult
    {
        public bool IsSuccess { get; private set; }
        public Card PlayedCard { get; private set; }
        public string Message { get; private set; }

        public static PlayCardResult Success(Card card) => new PlayCardResult
        {
            IsSuccess = true,
            PlayedCard = card,
            Message = "Card played successfully"
        };

        public static PlayCardResult Failure(string message) => new PlayCardResult
        {
            IsSuccess = false,
            PlayedCard = null,
            Message = message
        };
    }
}
