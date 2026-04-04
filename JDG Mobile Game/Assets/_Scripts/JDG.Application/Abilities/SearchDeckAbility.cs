using System.Linq;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Domain.Enums;
using JDG.Application.Repositories;
using JDG.Application.UseCases;

namespace JDG.Application.Abilities
{
    /// <summary>
    /// Ability that searches the deck for a specific card type and adds it to hand.
    /// Used for "Get [CardType] from deck" abilities.
    /// </summary>
    public class SearchDeckAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IEventBus _eventBus;
        private readonly CardType _searchCardType;
        private readonly CardFamily? _searchFamily;

        public AbilityName Name { get; }
        public string Description { get; }

        /// <summary>
        /// Creates a search deck ability.
        /// </summary>
        /// <param name="abilityName">The specific ability name</param>
        /// <param name="searchCardType">Type of card to search for</param>
        /// <param name="searchFamily">Optional: specific family to search for</param>
        /// <param name="playerRepository">Repository for accessing player data</param>
        /// <param name="eventBus">Event bus for publishing events</param>
        public SearchDeckAbility(
            AbilityName abilityName,
            CardType searchCardType,
            CardFamily? searchFamily,
            IPlayerRepository playerRepository,
            IEventBus eventBus)
        {
            Name = abilityName;
            _searchCardType = searchCardType;
            _searchFamily = searchFamily;
            _playerRepository = playerRepository;
            _eventBus = eventBus;

            string familyDesc = searchFamily.HasValue ? $" ({searchFamily.Value})" : "";
            Description = $"Search deck for {searchCardType}{familyDesc} and add to hand";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null) return false;

            // Check if deck has matching card
            return player.Deck.Any(c => c.Type == _searchCardType &&
                                       (!_searchFamily.HasValue || c.HasFamily(_searchFamily.Value)));
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);

            if (player == null)
            {
                return AbilityResult.Failure("Player not found");
            }

            // Search deck and draw matching card
            var matchingCard = player.SearchDeckAndDraw(c =>
                c.Type == _searchCardType &&
                (!_searchFamily.HasValue || c.HasFamily(_searchFamily.Value)));

            if (matchingCard == null)
            {
                return AbilityResult.Failure($"No matching {_searchCardType} found in deck");
            }

            _playerRepository.SavePlayer(player);

            // Publish event
            _eventBus.Publish(new CardDrawnEvent
            {
                CardId = matchingCard.Id.ToGuid(),
                Owner = context.CurrentPlayerId.ToCardOwner(),
                CardTitle = matchingCard.Title,
                DeckCount = player.DeckCount,
                HandCount = player.HandCount
            });

            return AbilityResult.Success($"Added {matchingCard.Title} to hand");
        }
    }

    /// <summary>
    /// Factory for creating search deck abilities.
    /// </summary>
    public class SearchDeckAbilityFactory
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IEventBus _eventBus;

        public SearchDeckAbilityFactory(
            IPlayerRepository playerRepository,
            IEventBus eventBus)
        {
            _playerRepository = playerRepository;
            _eventBus = eventBus;
        }

        public SearchDeckAbility CreateSearchByType(CardType cardType)
        {
            return new SearchDeckAbility(
                AbilityName.Default,
                cardType,
                searchFamily: null,
                _playerRepository,
                _eventBus
            );
        }

        public SearchDeckAbility CreateSearchByFamily(CardType cardType, CardFamily family)
        {
            return new SearchDeckAbility(
                AbilityName.Default,
                cardType,
                searchFamily: family,
                _playerRepository,
                _eventBus
            );
        }
    }
}
