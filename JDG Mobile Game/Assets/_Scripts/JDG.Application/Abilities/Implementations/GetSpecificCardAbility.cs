using JDG.Domain;
using JDG.Domain.Enums;
using JDG.Application.Repositories;
using System.Linq;

namespace JDG.Application.Abilities.Implementations
{
    /// <summary>
    /// Ability that searches the deck for a specific card and adds it to hand.
    /// Migrated from GetSpecificCardFromDeckAbility.
    /// </summary>
    public class GetSpecificCardAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly string _targetCardName;

        public AbilityName Name { get; }
        public string Description { get; }

        public GetSpecificCardAbility(
            AbilityName abilityName,
            string targetCardName,
            IPlayerRepository playerRepository)
        {
            Name = abilityName;
            _targetCardName = targetCardName;
            _playerRepository = playerRepository;
            Description = $"Search deck for {targetCardName} and add to hand";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return player != null && player.Deck.Any(c => c.Title == _targetCardName);
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null)
                return AbilityResult.Failure("Player not found");

            var targetCard = player.Deck.FirstOrDefault(c => c.Title == _targetCardName);
            if (targetCard == null)
                return AbilityResult.Failure($"{_targetCardName} not found in deck");

            // Move card from deck to hand
            player.Deck.Remove(targetCard);
            player.Hand.Add(targetCard);
            _playerRepository.SavePlayer(player);

            return AbilityResult.Success($"Added {_targetCardName} to hand");
        }
    }

    /// <summary>
    /// Ability that searches deck by card family.
    /// Migrated from GetFamilyInDeckAbility.
    /// </summary>
    public class GetFamilyCardAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly CardFamily _targetFamily;

        public AbilityName Name { get; }
        public string Description { get; }

        public GetFamilyCardAbility(
            AbilityName abilityName,
            CardFamily targetFamily,
            IPlayerRepository playerRepository)
        {
            Name = abilityName;
            _targetFamily = targetFamily;
            _playerRepository = playerRepository;
            Description = $"Search deck for {targetFamily} family card";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null) return false;

            return player.Deck.Any(c =>
                c.Type == CardType.Invocation &&
                c.Families != null &&
                c.Families.Contains(_targetFamily));
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null)
                return AbilityResult.Failure("Player not found");

            var familyCards = player.Deck
                .Where(c => c.Type == CardType.Invocation &&
                           c.Families != null &&
                           c.Families.Contains(_targetFamily))
                .ToList();

            if (!familyCards.Any())
                return AbilityResult.Failure($"No {_targetFamily} cards in deck");

            // Return NeedsUserInput to signal UI should show card selector
            return AbilityResult.NeedsUserInput($"Select {_targetFamily} card from deck");
        }
    }

    /// <summary>
    /// Factory for creating deck search abilities.
    /// </summary>
    public class DeckSearchAbilityFactory
    {
        private readonly IPlayerRepository _playerRepository;

        public DeckSearchAbilityFactory(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public GetSpecificCardAbility CreateGetSpecificCard(AbilityName name, string cardName)
        {
            return new GetSpecificCardAbility(name, cardName, _playerRepository);
        }

        public GetFamilyCardAbility CreateGetFamilyCard(AbilityName name, CardFamily family)
        {
            return new GetFamilyCardAbility(name, family, _playerRepository);
        }
    }
}
