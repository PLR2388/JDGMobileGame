using JDG.Domain;
using JDG.Application.Repositories;
using System.Linq;

namespace JDG.Application.Abilities.Implementations
{
    /// <summary>
    /// Ability that sacrifices a specific card from the field.
    /// Migrated from SacrificeCardAbility.
    /// </summary>
    public class SacrificeCardAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly string _targetCardName;

        public AbilityName Name { get; }
        public string Description { get; }

        public SacrificeCardAbility(
            AbilityName abilityName,
            string targetCardName,
            IPlayerRepository playerRepository)
        {
            Name = abilityName;
            _targetCardName = targetCardName;
            _playerRepository = playerRepository;
            Description = $"Sacrifice {targetCardName}";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return player != null && player.Field.Any(c => c.Title == _targetCardName);
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null)
                return AbilityResult.Failure("Player not found");

            var cardToSacrifice = player.Field.FirstOrDefault(c => c.Title == _targetCardName);
            if (cardToSacrifice == null)
                return AbilityResult.Failure($"{_targetCardName} not on field");

            // Move to graveyard
            player.DestroyCardFromField(cardToSacrifice);
            _playerRepository.SavePlayer(player);

            return AbilityResult.Success($"Sacrificed {_targetCardName}");
        }
    }

    /// <summary>
    /// Ability that invokes (summons) a specific card from deck to field.
    /// Migrated from InvokeSpecificCardAbility.
    /// </summary>
    public class InvokeSpecificCardAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly string _targetCardName;

        public AbilityName Name { get; }
        public string Description { get; }

        public InvokeSpecificCardAbility(
            AbilityName abilityName,
            string targetCardName,
            IPlayerRepository playerRepository)
        {
            Name = abilityName;
            _targetCardName = targetCardName;
            _playerRepository = playerRepository;
            Description = $"Invoke {targetCardName} from deck";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null) return false;

            // Check if we have room on field (max 4 invocations) and card is in deck
            return player.Field.Count < 4 &&
                   player.Deck.Any(c => c.Title == _targetCardName && c.Type == CardType.Invocation);
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null)
                return AbilityResult.Failure("Player not found");

            if (player.Field.Count >= 4)
                return AbilityResult.Failure("Field is full");

            // Move from deck to hand, then play to field
            var card = player.SearchDeckAndDraw(c => c.Title == _targetCardName && c.Type == CardType.Invocation);
            if (card == null)
                return AbilityResult.Failure($"{_targetCardName} not in deck");

            player.PlayCard(card);
            _playerRepository.SavePlayer(player);

            return AbilityResult.Success($"Invoked {_targetCardName}");
        }
    }

    /// <summary>
    /// Ability to sacrifice one card to invoke another.
    /// Migrated from SacrificeToInvokeAbility.
    /// </summary>
    public class SacrificeToInvokeAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;

        public AbilityName Name { get; }
        public string Description { get; }

        public SacrificeToInvokeAbility(IPlayerRepository playerRepository)
        {
            Name = AbilityName.SacrificeToInvoke;
            _playerRepository = playerRepository;
            Description = "Sacrifice a card to invoke another";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return player != null && player.Field.Any() && player.Hand.Any(c => c.Type == CardType.Invocation);
        }

        public AbilityResult Execute(AbilityContext context)
        {
            // This requires user input to select which card to sacrifice and which to invoke
            return AbilityResult.NeedsUserInput("Select card to sacrifice and card to invoke");
        }
    }

    /// <summary>
    /// Factory for creating sacrifice and invocation abilities.
    /// </summary>
    public class SacrificeAbilityFactory
    {
        private readonly IPlayerRepository _playerRepository;

        public SacrificeAbilityFactory(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public SacrificeCardAbility CreateSacrificeCard(AbilityName name, string cardName)
        {
            return new SacrificeCardAbility(name, cardName, _playerRepository);
        }

        public InvokeSpecificCardAbility CreateInvokeSpecificCard(AbilityName name, string cardName)
        {
            return new InvokeSpecificCardAbility(name, cardName, _playerRepository);
        }

        public SacrificeToInvokeAbility CreateSacrificeToInvoke()
        {
            return new SacrificeToInvokeAbility(_playerRepository);
        }
    }
}
