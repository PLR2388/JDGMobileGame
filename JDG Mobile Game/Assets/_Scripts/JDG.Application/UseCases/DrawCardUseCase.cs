using JDG.Application.Repositories;
using JDG.Domain.Entities;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;

namespace JDG.Application.UseCases
{
    /// <summary>
    /// Use case for drawing a card from a player's deck to their hand.
    /// Publishes CardDrawnEvent on success.
    /// </summary>
    public class DrawCardUseCase
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IEventBus _eventBus;

        public DrawCardUseCase(IPlayerRepository playerRepository, IEventBus eventBus)
        {
            _playerRepository = playerRepository;
            _eventBus = eventBus;
        }

        /// <summary>
        /// Executes the draw card use case.
        /// </summary>
        /// <param name="playerId">The player drawing the card</param>
        /// <returns>Result containing the drawn card or failure reason</returns>
        public DrawCardResult Execute(PlayerId playerId)
        {
            var player = _playerRepository.GetPlayer(playerId);

            if (player == null)
                return DrawCardResult.Failure("Player not found");

            if (player.SkipCurrentDraw)
            {
                player.SkipCurrentDraw = false;
                _playerRepository.SavePlayer(player);
                return DrawCardResult.Skipped("Draw skipped due to field effect");
            }

            var drawnCard = player.DrawCard();

            if (drawnCard == null)
                return DrawCardResult.Failure("Deck is empty");

            // Save player state
            _playerRepository.SavePlayer(player);

            // Publish event
            _eventBus.Publish(new CardDrawnEvent
            {
                PlayerId = playerId.ToCardOwner(),
                CardId = drawnCard.Id.ToGuid(),
                CardTitle = drawnCard.Title,
                DeckCount = player.DeckCount,
                HandCount = player.HandCount
            });

            return DrawCardResult.Success(drawnCard);
        }
    }

    /// <summary>
    /// Result of a draw card operation.
    /// </summary>
    public class DrawCardResult
    {
        public bool IsSuccess { get; private set; }
        public bool WasSkipped { get; private set; }
        public Card DrawnCard { get; private set; }
        public string Message { get; private set; }

        public static DrawCardResult Success(Card card) => new DrawCardResult
        {
            IsSuccess = true,
            WasSkipped = false,
            DrawnCard = card,
            Message = "Card drawn successfully"
        };

        public static DrawCardResult Failure(string message) => new DrawCardResult
        {
            IsSuccess = false,
            WasSkipped = false,
            DrawnCard = null,
            Message = message
        };

        public static DrawCardResult Skipped(string message) => new DrawCardResult
        {
            IsSuccess = false,
            WasSkipped = true,
            DrawnCard = null,
            Message = message
        };
    }
}
