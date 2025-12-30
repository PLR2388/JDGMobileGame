using JDG.Domain;
using JDG.Domain.Enums;
using JDG.Application.Repositories;
using System.Linq;

namespace JDG.Application.Abilities.Implementations
{
    /// <summary>
    /// Field ability that gives ATK/DEF bonus to family members.
    /// Migrated from EarnATKDEFForFamilyAbility.
    /// </summary>
    public class FamilyBoostFieldAbility : IAbility, IPassiveAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly CardFamily _targetFamily;
        private readonly float _attackBonus;
        private readonly float _defenseBonus;

        public AbilityName Name { get; }
        public string Description { get; }
        public AbilityTrigger Trigger => AbilityTrigger.Continuous;

        public FamilyBoostFieldAbility(
            CardFamily targetFamily,
            float attackBonus,
            float defenseBonus,
            IPlayerRepository playerRepository)
        {
            Name = AbilityName.Default;
            _targetFamily = targetFamily;
            _attackBonus = attackBonus;
            _defenseBonus = defenseBonus;
            _playerRepository = playerRepository;
            Description = $"{targetFamily} cards gain +{attackBonus} ATK / +{defenseBonus} DEF";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return player != null && player.Field.Any(c =>
                c.Families != null && c.Families.Contains(_targetFamily));
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null)
                return AbilityResult.Failure("Player not found");

            var familyCards = player.Field.Where(c =>
                c.Families != null && c.Families.Contains(_targetFamily)).ToList();

            foreach (var card in familyCards)
            {
                card.ModifyStats((int)_attackBonus, (int)_defenseBonus);
            }

            _playerRepository.SavePlayer(player);
            return AbilityResult.Success($"Boosted {familyCards.Count} {_targetFamily} cards");
        }
    }

    /// <summary>
    /// Field ability that restores HP per family card on turn start.
    /// Migrated from EarnHPPerFamilyOnTurnStartAbility.
    /// </summary>
    public class HealPerFamilyFieldAbility : IAbility, IPassiveAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly CardFamily _targetFamily;
        private readonly float _hpPerCard;

        public AbilityName Name { get; }
        public string Description { get; }
        public AbilityTrigger Trigger => AbilityTrigger.OnTurnStart;

        public HealPerFamilyFieldAbility(
            CardFamily targetFamily,
            float hpPerCard,
            IPlayerRepository playerRepository)
        {
            Name = AbilityName.Default;
            _targetFamily = targetFamily;
            _hpPerCard = hpPerCard;
            _playerRepository = playerRepository;
            Description = $"Restore {hpPerCard} HP per {targetFamily} card on turn start";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return player != null && player.Field.Any(c =>
                c.Families != null && c.Families.Contains(_targetFamily));
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null)
                return AbilityResult.Failure("Player not found");

            int familyCardCount = player.Field.Count(c =>
                c.Families != null && c.Families.Contains(_targetFamily));

            float totalHeal = familyCardCount * _hpPerCard;
            player.Heal(totalHeal);
            _playerRepository.SavePlayer(player);

            return AbilityResult.Success($"Healed {totalHeal} HP ({familyCardCount} × {_hpPerCard})");
        }
    }

    /// <summary>
    /// Field ability that changes invocation families.
    /// Migrated from ChangeInvocationFamilyAbility.
    /// </summary>
    public class ChangeFamilyFieldAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly CardFamily _newFamily;

        public AbilityName Name { get; }
        public string Description { get; }

        public ChangeFamilyFieldAbility(
            CardFamily newFamily,
            IPlayerRepository playerRepository)
        {
            Name = AbilityName.Default;
            _newFamily = newFamily;
            _playerRepository = playerRepository;
            Description = $"Change all invocations to {newFamily} family";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return player != null && player.Field.Any(c => c.Type == CardType.Invocation);
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null)
                return AbilityResult.Failure("Player not found");

            var invocations = player.Field.Where(c => c.Type == CardType.Invocation).ToList();

            foreach (var card in invocations)
            {
                card.SetFamilies(new[] { _newFamily });
            }

            _playerRepository.SavePlayer(player);
            return AbilityResult.Success($"Changed {invocations.Count} cards to {_newFamily}");
        }
    }

    /// <summary>
    /// Field ability that allows drawing more cards.
    /// Migrated from DrawMoreCardsAbility.
    /// </summary>
    public class DrawBonusFieldAbility : IAbility, IPassiveAbility
    {
        private readonly int _bonusDraws;

        public AbilityName Name { get; }
        public string Description { get; }
        public AbilityTrigger Trigger => AbilityTrigger.Continuous;

        public DrawBonusFieldAbility(int bonusDraws = 1)
        {
            Name = AbilityName.Default;
            _bonusDraws = bonusDraws;
            Description = $"Draw +{bonusDraws} card(s) per turn";
        }

        public bool CanActivate(AbilityContext context)
        {
            return true;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            // This is a passive modifier to draw phase
            return AbilityResult.Success($"Draw +{_bonusDraws} bonus enabled");
        }
    }

    /// <summary>
    /// Field ability that grants card from family if draw is skipped.
    /// Migrated from GetCardFromFamilyIfSkipDrawAbility.
    /// </summary>
    public class SkipDrawForFamilyCardFieldAbility : IAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly CardFamily _targetFamily;

        public AbilityName Name { get; }
        public string Description { get; }

        public SkipDrawForFamilyCardFieldAbility(
            CardFamily targetFamily,
            IPlayerRepository playerRepository)
        {
            Name = AbilityName.Default;
            _targetFamily = targetFamily;
            _playerRepository = playerRepository;
            Description = $"Skip draw to get {targetFamily} card from deck";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return player != null && player.Deck.Any(c =>
                c.Families != null && c.Families.Contains(_targetFamily));
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null)
                return AbilityResult.Failure("Player not found");

            var familyCard = player.SearchDeckAndDraw(c =>
                c.Families != null && c.Families.Contains(_targetFamily));

            if (familyCard == null)
                return AbilityResult.Failure($"No {_targetFamily} cards in deck");

            _playerRepository.SavePlayer(player);
            return AbilityResult.Success($"Got {familyCard.Title} from deck (skipped draw)");
        }
    }

    /// <summary>
    /// Field ability that changes a specific card's family by name.
    /// Migrated from ChangeInvocationFamilyAbility (card-specific version).
    /// </summary>
    public class ChangeByNameFieldAbility : IAbility, IPassiveAbility
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly string _cardName;
        private readonly CardFamily _newFamily;

        public AbilityName Name { get; }
        public string Description { get; }
        public AbilityTrigger Trigger => AbilityTrigger.Continuous;

        public ChangeByNameFieldAbility(
            string cardName,
            CardFamily newFamily,
            IPlayerRepository playerRepository)
        {
            Name = AbilityName.Default;
            _cardName = cardName;
            _newFamily = newFamily;
            _playerRepository = playerRepository;
            Description = $"{cardName} gains {newFamily} family";
        }

        public bool CanActivate(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            return player != null && player.Field.Any(c => c.Title == _cardName);
        }

        public AbilityResult Execute(AbilityContext context)
        {
            var player = _playerRepository.GetPlayer(context.CurrentPlayerId);
            if (player == null)
                return AbilityResult.Failure("Player not found");

            var targetCard = player.Field.FirstOrDefault(c => c.Title == _cardName);
            if (targetCard == null)
                return AbilityResult.Failure($"{_cardName} not found on field");

            // Add the new family to existing families
            var families = targetCard.Families.ToList();
            if (!families.Contains(_newFamily))
            {
                families.Add(_newFamily);
                targetCard.SetFamilies(families);
            }

            _playerRepository.SavePlayer(player);
            return AbilityResult.Success($"{_cardName} now has {_newFamily} family");
        }
    }

    /// <summary>
    /// Factory for creating field abilities.
    /// </summary>
    public class FieldAbilityFactory
    {
        private readonly IPlayerRepository _playerRepository;

        public FieldAbilityFactory(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public FamilyBoostFieldAbility CreateFamilyBoost(CardFamily family, float atk, float def)
        {
            return new FamilyBoostFieldAbility(family, atk, def, _playerRepository);
        }

        public HealPerFamilyFieldAbility CreateHealPerFamily(CardFamily family, float hpPerCard)
        {
            return new HealPerFamilyFieldAbility(family, hpPerCard, _playerRepository);
        }

        public ChangeByNameFieldAbility CreateChangeByName(string cardName, CardFamily newFamily)
        {
            return new ChangeByNameFieldAbility(cardName, newFamily, _playerRepository);
        }

        public ChangeFamilyFieldAbility CreateChangeFamily(CardFamily newFamily)
        {
            return new ChangeFamilyFieldAbility(newFamily, _playerRepository);
        }

        public DrawBonusFieldAbility CreateDrawBonus(int bonusDraws = 1)
        {
            return new DrawBonusFieldAbility(bonusDraws);
        }

        public SkipDrawForFamilyCardFieldAbility CreateSkipDrawForFamily(CardFamily family)
        {
            return new SkipDrawForFamilyCardFieldAbility(family, _playerRepository);
        }
    }
}
