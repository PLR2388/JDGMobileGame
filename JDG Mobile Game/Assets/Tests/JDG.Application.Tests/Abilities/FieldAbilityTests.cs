using System.Collections.Generic;
using System.Linq;
using JDG.Application.Abilities;
using JDG.Application.Abilities.Implementations;
using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;
using NUnit.Framework;

namespace JDG.Application.Tests.Abilities
{
    /// <summary>
    /// Tests for field ability implementations.
    /// Tests FamilyBoostFieldAbility, HealPerFamilyFieldAbility, ChangeFamilyFieldAbility,
    /// DrawBonusFieldAbility, SkipDrawForFamilyCardFieldAbility, and FieldAbilityFactory.
    /// </summary>
    [TestFixture]
    public class FieldAbilityTests
    {
        private TestPlayerRepository _playerRepository;
        private Card _sourceCard;
        private PlayerId _playerId;
        private PlayerId _opponentId;

        [SetUp]
        public void SetUp()
        {
            _playerRepository = new TestPlayerRepository();
            _playerId = PlayerId.Player1;
            _opponentId = PlayerId.Player2;
            _sourceCard = CreateFieldCard("TestField", CardFamily.Human);
        }

        private Card CreateInvocationCard(string name, int attack, int defense, CardFamily family = CardFamily.Human)
        {
            return Card.CreateInvocation(
                id: CardId.New(),
                title: name,
                description: "Test card",
                detailedDescription: "Test card description",
                attack: attack,
                defense: defense,
                families: new[] { family },
                affectedByEffect: true
            );
        }

        private Card CreateFieldCard(string name, CardFamily family)
        {
            return Card.CreateField(
                id: CardId.New(),
                title: name,
                description: "Test field",
                detailedDescription: "Test field description",
                fieldFamily: family,
                fieldAbilities: new FieldAbilityName[] { }
            );
        }

        private AbilityContext CreateContext(Card sourceCard = null)
        {
            return new AbilityContext(
                _playerId,
                _opponentId,
                sourceCard ?? _sourceCard,
                AbilityName.Default
            );
        }

        private void SetupPlayerWithFieldCards(PlayerId playerId, params Card[] fieldCards)
        {
            var deckCards = new List<Card>();
            var player = new Player(playerId, deckCards);

            // Use reflection or create a test helper to add cards to field
            // Since Player.PlayCard requires cards in hand, we'll set up the player with cards in hand first
            foreach (var card in fieldCards)
            {
                // Add to hand then play to field
                var handField = typeof(Player).GetField("_hand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var hand = (List<Card>)handField.GetValue(player);
                hand.Add(card);
                player.PlayCard(card);
            }

            _playerRepository.SavePlayer(player);
        }

        private void SetupPlayerWithDeckCards(PlayerId playerId, params Card[] deckCards)
        {
            var player = new Player(playerId, deckCards.ToList());
            _playerRepository.SavePlayer(player);
        }

        #region FamilyBoostFieldAbility Tests

        [Test]
        public void FamilyBoostFieldAbility_Constructor_SetsProperties()
        {
            var ability = new FamilyBoostFieldAbility(
                CardFamily.Human,
                500,
                300,
                _playerRepository
            );

            Assert.That(ability.Name, Is.EqualTo(AbilityName.Default));
            Assert.That(ability.Description, Is.EqualTo("Human cards gain +500 ATK / +300 DEF"));
            Assert.That(ability.Trigger, Is.EqualTo(AbilityTrigger.Continuous));
        }

        [Test]
        public void FamilyBoostFieldAbility_CanActivate_ReturnsTrueWhenFamilyCardsOnField()
        {
            var card1 = CreateInvocationCard("Card1", 1000, 800, CardFamily.Human);
            SetupPlayerWithFieldCards(_playerId, card1);

            var ability = new FamilyBoostFieldAbility(
                CardFamily.Human,
                500,
                300,
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.True);
        }

        [Test]
        public void FamilyBoostFieldAbility_CanActivate_ReturnsFalseWhenNoFamilyCardsOnField()
        {
            var card1 = CreateInvocationCard("Card1", 1000, 800, CardFamily.Monster);
            SetupPlayerWithFieldCards(_playerId, card1);

            var ability = new FamilyBoostFieldAbility(
                CardFamily.Human,
                500,
                300,
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.False);
        }

        [Test]
        public void FamilyBoostFieldAbility_Execute_BoostsAllFamilyCards()
        {
            var card1 = CreateInvocationCard("Card1", 1000, 800, CardFamily.Human);
            var card2 = CreateInvocationCard("Card2", 500, 400, CardFamily.Human);
            var card3 = CreateInvocationCard("Card3", 2000, 1500, CardFamily.Monster);
            SetupPlayerWithFieldCards(_playerId, card1, card2, card3);

            var ability = new FamilyBoostFieldAbility(
                CardFamily.Human,
                500,
                300,
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Boosted 2 Human cards"));
            Assert.That(card1.Stats.Value.Attack, Is.EqualTo(1500)); // 1000 + 500
            Assert.That(card1.Stats.Value.Defense, Is.EqualTo(1100)); // 800 + 300
            Assert.That(card2.Stats.Value.Attack, Is.EqualTo(1000)); // 500 + 500
            Assert.That(card2.Stats.Value.Defense, Is.EqualTo(700)); // 400 + 300
            // Card3 should not be affected (different family)
            Assert.That(card3.Stats.Value.Attack, Is.EqualTo(2000));
            Assert.That(card3.Stats.Value.Defense, Is.EqualTo(1500));
        }

        [Test]
        public void FamilyBoostFieldAbility_Execute_FailsWhenPlayerNotFound()
        {
            // Create a truly empty repository with no players
            var emptyRepo = TestPlayerRepository.CreateEmpty();

            var ability = new FamilyBoostFieldAbility(
                CardFamily.Human,
                500,
                300,
                emptyRepo
            );
            // Create context that points to a non-existent player ID
            var context = CreateContext();

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Player not found"));
        }

        #endregion

        #region HealPerFamilyFieldAbility Tests

        [Test]
        public void HealPerFamilyFieldAbility_Constructor_SetsProperties()
        {
            var ability = new HealPerFamilyFieldAbility(
                CardFamily.Human,
                100,
                _playerRepository
            );

            Assert.That(ability.Name, Is.EqualTo(AbilityName.Default));
            Assert.That(ability.Description, Is.EqualTo("Restore 100 HP per Human card on turn start"));
            Assert.That(ability.Trigger, Is.EqualTo(AbilityTrigger.OnTurnStart));
        }

        [Test]
        public void HealPerFamilyFieldAbility_CanActivate_ReturnsTrueWhenFamilyCardsOnField()
        {
            var card1 = CreateInvocationCard("Card1", 1000, 800, CardFamily.Human);
            SetupPlayerWithFieldCards(_playerId, card1);

            var ability = new HealPerFamilyFieldAbility(
                CardFamily.Human,
                100,
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.True);
        }

        [Test]
        public void HealPerFamilyFieldAbility_CanActivate_ReturnsFalseWhenNoFamilyCardsOnField()
        {
            var card1 = CreateInvocationCard("Card1", 1000, 800, CardFamily.Monster);
            SetupPlayerWithFieldCards(_playerId, card1);

            var ability = new HealPerFamilyFieldAbility(
                CardFamily.Human,
                100,
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.False);
        }

        [Test]
        public void HealPerFamilyFieldAbility_Execute_CalculatesCorrectHealAmount()
        {
            var card1 = CreateInvocationCard("Card1", 1000, 800, CardFamily.Human);
            var card2 = CreateInvocationCard("Card2", 500, 400, CardFamily.Human);
            var card3 = CreateInvocationCard("Card3", 2000, 1500, CardFamily.Human);
            SetupPlayerWithFieldCards(_playerId, card1, card2, card3);

            var ability = new HealPerFamilyFieldAbility(
                CardFamily.Human,
                100,
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Healed 300 HP (3 × 100)"));
        }

        #endregion

        #region ChangeFamilyFieldAbility Tests

        [Test]
        public void ChangeFamilyFieldAbility_Constructor_SetsProperties()
        {
            var ability = new ChangeFamilyFieldAbility(
                CardFamily.Human,
                _playerRepository
            );

            Assert.That(ability.Name, Is.EqualTo(AbilityName.Default));
            Assert.That(ability.Description, Is.EqualTo("Change all invocations to Human family"));
        }

        [Test]
        public void ChangeFamilyFieldAbility_CanActivate_ReturnsTrueWhenInvocationsOnField()
        {
            var card1 = CreateInvocationCard("Card1", 1000, 800, CardFamily.Monster);
            SetupPlayerWithFieldCards(_playerId, card1);

            var ability = new ChangeFamilyFieldAbility(
                CardFamily.Human,
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ChangeFamilyFieldAbility_CanActivate_ReturnsFalseWhenNoInvocationsOnField()
        {
            // Setup player with no cards on field
            _playerRepository.SetupPlayer(_playerId, new List<Card>());

            var ability = new ChangeFamilyFieldAbility(
                CardFamily.Human,
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ChangeFamilyFieldAbility_Execute_ChangesAllInvocationFamilies()
        {
            var card1 = CreateInvocationCard("Card1", 1000, 800, CardFamily.Monster);
            var card2 = CreateInvocationCard("Card2", 500, 400, CardFamily.Japan);
            SetupPlayerWithFieldCards(_playerId, card1, card2);

            var ability = new ChangeFamilyFieldAbility(
                CardFamily.Human,
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Changed 2 cards to Human"));
            Assert.That(card1.Families, Contains.Item(CardFamily.Human));
            Assert.That(card2.Families, Contains.Item(CardFamily.Human));
        }

        #endregion

        #region DrawBonusFieldAbility Tests

        [Test]
        public void DrawBonusFieldAbility_Constructor_SetsProperties()
        {
            var ability = new DrawBonusFieldAbility(2);

            Assert.That(ability.Name, Is.EqualTo(AbilityName.Default));
            Assert.That(ability.Description, Is.EqualTo("Draw +2 card(s) per turn"));
            Assert.That(ability.Trigger, Is.EqualTo(AbilityTrigger.Continuous));
        }

        [Test]
        public void DrawBonusFieldAbility_Constructor_DefaultsToOneBonus()
        {
            var ability = new DrawBonusFieldAbility();

            Assert.That(ability.Description, Is.EqualTo("Draw +1 card(s) per turn"));
        }

        [Test]
        public void DrawBonusFieldAbility_CanActivate_AlwaysReturnsTrue()
        {
            var ability = new DrawBonusFieldAbility(1);
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.True);
        }

        [Test]
        public void DrawBonusFieldAbility_Execute_ReturnsSuccess()
        {
            var ability = new DrawBonusFieldAbility(2);
            var context = CreateContext();

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Draw +2 bonus enabled"));
        }

        #endregion

        #region SkipDrawForFamilyCardFieldAbility Tests

        [Test]
        public void SkipDrawForFamilyCardFieldAbility_Constructor_SetsProperties()
        {
            var ability = new SkipDrawForFamilyCardFieldAbility(
                CardFamily.Human,
                _playerRepository
            );

            Assert.That(ability.Name, Is.EqualTo(AbilityName.Default));
            Assert.That(ability.Description, Is.EqualTo("Skip draw to get Human card from deck"));
        }

        [Test]
        public void SkipDrawForFamilyCardFieldAbility_CanActivate_ReturnsTrueWhenFamilyCardInDeck()
        {
            var deckCard = CreateInvocationCard("DeckCard", 1000, 800, CardFamily.Human);
            SetupPlayerWithDeckCards(_playerId, deckCard);

            var ability = new SkipDrawForFamilyCardFieldAbility(
                CardFamily.Human,
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.True);
        }

        [Test]
        public void SkipDrawForFamilyCardFieldAbility_CanActivate_ReturnsFalseWhenNoFamilyCardInDeck()
        {
            var deckCard = CreateInvocationCard("DeckCard", 1000, 800, CardFamily.Monster);
            SetupPlayerWithDeckCards(_playerId, deckCard);

            var ability = new SkipDrawForFamilyCardFieldAbility(
                CardFamily.Human,
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.False);
        }

        [Test]
        public void SkipDrawForFamilyCardFieldAbility_Execute_DrawsFamilyCardFromDeck()
        {
            var humanCard = CreateInvocationCard("HumanCard", 1000, 800, CardFamily.Human);
            var otherCard = CreateInvocationCard("OtherCard", 500, 400, CardFamily.Monster);
            SetupPlayerWithDeckCards(_playerId, otherCard, humanCard);

            var ability = new SkipDrawForFamilyCardFieldAbility(
                CardFamily.Human,
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Got HumanCard from deck (skipped draw)"));

            var player = _playerRepository.GetPlayer(_playerId);
            Assert.That(player.Hand, Contains.Item(humanCard));
            Assert.That(player.Deck, Has.No.Member(humanCard));
        }

        [Test]
        public void SkipDrawForFamilyCardFieldAbility_Execute_FailsWhenNoFamilyCardInDeck()
        {
            var otherCard = CreateInvocationCard("OtherCard", 500, 400, CardFamily.Monster);
            SetupPlayerWithDeckCards(_playerId, otherCard);

            var ability = new SkipDrawForFamilyCardFieldAbility(
                CardFamily.Human,
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("No Human cards in deck"));
        }

        #endregion

        #region FieldAbilityFactory Tests

        [Test]
        public void FieldAbilityFactory_CreateFamilyBoost_ReturnsCorrectAbility()
        {
            var factory = new FieldAbilityFactory(_playerRepository);

            var ability = factory.CreateFamilyBoost(CardFamily.Human, 500, 300);

            Assert.That(ability, Is.TypeOf<FamilyBoostFieldAbility>());
            Assert.That(ability.Description, Is.EqualTo("Human cards gain +500 ATK / +300 DEF"));
        }

        [Test]
        public void FieldAbilityFactory_CreateHealPerFamily_ReturnsCorrectAbility()
        {
            var factory = new FieldAbilityFactory(_playerRepository);

            var ability = factory.CreateHealPerFamily(CardFamily.Human, 100);

            Assert.That(ability, Is.TypeOf<HealPerFamilyFieldAbility>());
            Assert.That(ability.Description, Is.EqualTo("Restore 100 HP per Human card on turn start"));
        }

        [Test]
        public void FieldAbilityFactory_CreateChangeFamily_ReturnsCorrectAbility()
        {
            var factory = new FieldAbilityFactory(_playerRepository);

            var ability = factory.CreateChangeFamily(CardFamily.Human);

            Assert.That(ability, Is.TypeOf<ChangeFamilyFieldAbility>());
            Assert.That(ability.Description, Is.EqualTo("Change all invocations to Human family"));
        }

        [Test]
        public void FieldAbilityFactory_CreateDrawBonus_ReturnsCorrectAbility()
        {
            var factory = new FieldAbilityFactory(_playerRepository);

            var ability = factory.CreateDrawBonus(2);

            Assert.That(ability, Is.TypeOf<DrawBonusFieldAbility>());
            Assert.That(ability.Description, Is.EqualTo("Draw +2 card(s) per turn"));
        }

        [Test]
        public void FieldAbilityFactory_CreateSkipDrawForFamily_ReturnsCorrectAbility()
        {
            var factory = new FieldAbilityFactory(_playerRepository);

            var ability = factory.CreateSkipDrawForFamily(CardFamily.Human);

            Assert.That(ability, Is.TypeOf<SkipDrawForFamilyCardFieldAbility>());
            Assert.That(ability.Description, Is.EqualTo("Skip draw to get Human card from deck"));
        }

        [Test]
        public void FieldAbilityFactory_AllFactoryMethods_ReturnValidAbilities()
        {
            var factory = new FieldAbilityFactory(_playerRepository);

            var abilities = new IAbility[]
            {
                factory.CreateFamilyBoost(CardFamily.Human, 100, 100),
                factory.CreateHealPerFamily(CardFamily.Human, 50),
                factory.CreateChangeFamily(CardFamily.Japan),
                factory.CreateDrawBonus(1),
                factory.CreateSkipDrawForFamily(CardFamily.Monster)
            };

            foreach (var ability in abilities)
            {
                Assert.That(ability, Is.Not.Null);
                Assert.That(ability.Name, Is.EqualTo(AbilityName.Default));
                Assert.That(ability.Description, Is.Not.Null.And.Not.Empty);
            }
        }

        #endregion
    }
}
