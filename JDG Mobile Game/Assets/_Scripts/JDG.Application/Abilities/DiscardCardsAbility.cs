using System.Linq;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Application.Repositories;

namespace JDG.Application.Abilities
{
    /// <summary>
    /// Ability that discards cards from hand to graveyard.
    /// Used for effects that require discarding as cost or effect.
    /// </summary>
    public class DiscardCardsAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IEventBus _eventBus;
        private readonly int _numberOfCards;
        private readonly bool _targetOpponent;

        public AbilityName Name { get; }
        public string Description { get; }

        /// <summary>
        /// Creates a discard cards ability.
        /// </summary>
        /// <param name="abilityName">The specific ability name</param>
        /// <param name="numberOfCards">Number of cards to discard</param>
        /// <param name="targetOpponent">True to discard from opponent's hand</param>
        /// <param name="playerRepository">Repository for accessing player data</param>
        /// <param name="eventBus">Event bus for publishing events</param>
        public DiscardCardsAbility(
            AbilityName abilityName,
            int numberOfCards,
            bool targetOpponent,
            IPlayerRepository playerRepository,
            IEventBus eventBus)
        {
            Name = abilityName;
            _numberOfCards = numberOfCards;
            _targetOpponent = targetOpponent;
            _playerRepository = playerRepository;
            _eventBus = eventBus;
            Description = $"Discard {numberOfCards} card(s) from {(targetOpponent ? "opponent's" : "your")} hand";
        }

        public bool CanActivate(AbilityContext context)
        {
            var targetPlayerId = _targetOpponent ? context.OpponentPlayerId : context.CurrentPlayerId;
            var player = _playerRepository.GetPlayer(targetPlayerId);
            return player != null && player.Hand.Count >= _numberOfCards;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var targetPlayerId = _targetOpponent ? context.OpponentPlayerId : context.CurrentPlayerId;
            var player = _playerRepository.GetPlayer(targetPlayerId);

            if (player == null)
            {
                return AbilityResult.Failure("Player not found");
            }

            if (player.Hand.Count < _numberOfCards)
            {
                return AbilityResult.Failure($"Not enough cards in hand (need {_numberOfCards}, have {player.Hand.Count})");
            }

            // Discard cards (take from hand, add to graveyard)
            // Phase 147: Use FirstOrDefault for safety against concurrent modification
            int cardsDiscarded = 0;
            for (int i = 0; i < _numberOfCards && player.Hand.Count > 0; i++)
            {
                var card = player.Hand.FirstOrDefault();
                if (card == null)
                {
                    break; // Hand was modified during iteration
                }
                player.DiscardCard(card);
                cardsDiscarded++;

                // Publish event for each discarded card
                _eventBus.Publish(new CardDiscardedEvent
                {
                    CardId = card.Id.ToGuid(),
                    Owner = targetPlayerId.ToCardOwner(),
                    CardTitle = card.Title
                });
            }

            _playerRepository.SavePlayer(player);

            return AbilityResult.Success($"Discarded {cardsDiscarded} card(s)");
        }
    }

    /// <summary>
    /// Factory for creating discard cards abilities.
    /// </summary>
    public class DiscardCardsAbilityFactory
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IEventBus _eventBus;

        public DiscardCardsAbilityFactory(
            IPlayerRepository playerRepository,
            IEventBus eventBus)
        {
            _playerRepository = playerRepository;
            _eventBus = eventBus;
        }

        public DiscardCardsAbility CreateDiscardSelf(int count)
        {
            return new DiscardCardsAbility(
                AbilityName.Default,
                count,
                targetOpponent: false,
                _playerRepository,
                _eventBus
            );
        }

        public DiscardCardsAbility CreateDiscardOpponent(int count)
        {
            return new DiscardCardsAbility(
                AbilityName.Default,
                count,
                targetOpponent: true,
                _playerRepository,
                _eventBus
            );
        }
    }
}
