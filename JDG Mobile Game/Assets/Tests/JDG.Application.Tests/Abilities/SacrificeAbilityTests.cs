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
    /// Tests for sacrifice ability implementations.
    /// Tests SacrificeCardAbility, InvokeSpecificCardAbility, SacrificeToInvokeAbility,
    /// and SacrificeAbilityFactory.
    /// </summary>
    [TestFixture]
    public class SacrificeAbilityTests
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
            _sourceCard = CreateInvocationCard("SourceCard", 1000, 800);
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

            foreach (var card in fieldCards)
            {
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

        private void SetupPlayerWithFieldAndHand(PlayerId playerId, Card[] fieldCards, Card[] handCards)
        {
            var player = new Player(playerId, new List<Card>());

            // Add field cards
            foreach (var card in fieldCards)
            {
                var handField = typeof(Player).GetField("_hand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var hand = (List<Card>)handField.GetValue(player);
                hand.Add(card);
                player.PlayCard(card);
            }

            // Add hand cards
            foreach (var card in handCards)
            {
                var handField = typeof(Player).GetField("_hand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var hand = (List<Card>)handField.GetValue(player);
                hand.Add(card);
            }

            _playerRepository.SavePlayer(player);
        }

        #region SacrificeCardAbility Tests

        [Test]
        public void SacrificeCardAbility_Constructor_SetsProperties()
        {
            var ability = new SacrificeCardAbility(
                AbilityName.Default,
                "TargetCard",
                _playerRepository
            );

            Assert.That(ability.Name, Is.EqualTo(AbilityName.Default));
            Assert.That(ability.Description, Is.EqualTo("Sacrifice TargetCard"));
        }

        [Test]
        public void SacrificeCardAbility_CanActivate_ReturnsTrueWhenTargetOnField()
        {
            var targetCard = CreateInvocationCard("TargetCard", 500, 400);
            SetupPlayerWithFieldCards(_playerId, targetCard);

            var ability = new SacrificeCardAbility(
                AbilityName.Default,
                "TargetCard",
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.True);
        }

        [Test]
        public void SacrificeCardAbility_CanActivate_ReturnsFalseWhenTargetNotOnField()
        {
            var otherCard = CreateInvocationCard("OtherCard", 500, 400);
            SetupPlayerWithFieldCards(_playerId, otherCard);

            var ability = new SacrificeCardAbility(
                AbilityName.Default,
                "TargetCard",
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.False);
        }

        [Test]
        public void SacrificeCardAbility_Execute_MovesCardToGraveyard()
        {
            var targetCard = CreateInvocationCard("TargetCard", 500, 400);
            SetupPlayerWithFieldCards(_playerId, targetCard);

            var ability = new SacrificeCardAbility(
                AbilityName.Default,
                "TargetCard",
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Sacrificed TargetCard"));

            var player = _playerRepository.GetPlayer(_playerId);
            Assert.That(player.Field, Has.No.Member(targetCard));
            Assert.That(player.Graveyard, Contains.Item(targetCard));
        }

        [Test]
        public void SacrificeCardAbility_Execute_FailsWhenTargetNotOnField()
        {
            var otherCard = CreateInvocationCard("OtherCard", 500, 400);
            SetupPlayerWithFieldCards(_playerId, otherCard);

            var ability = new SacrificeCardAbility(
                AbilityName.Default,
                "TargetCard",
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("TargetCard not on field"));
        }

        [Test]
        public void SacrificeCardAbility_Execute_FailsWhenPlayerNotFound()
        {
            // Create a truly empty repository with no players
            var emptyRepo = TestPlayerRepository.CreateEmpty();

            var ability = new SacrificeCardAbility(
                AbilityName.Default,
                "TargetCard",
                emptyRepo
            );
            var context = CreateContext();

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Player not found"));
        }

        #endregion

        #region InvokeSpecificCardAbility Tests

        [Test]
        public void InvokeSpecificCardAbility_Constructor_SetsProperties()
        {
            var ability = new InvokeSpecificCardAbility(
                AbilityName.Default,
                "TargetCard",
                _playerRepository
            );

            Assert.That(ability.Name, Is.EqualTo(AbilityName.Default));
            Assert.That(ability.Description, Is.EqualTo("Invoke TargetCard from deck"));
        }

        [Test]
        public void InvokeSpecificCardAbility_CanActivate_ReturnsTrueWhenCardInDeckAndFieldNotFull()
        {
            var deckCard = CreateInvocationCard("TargetCard", 500, 400);
            SetupPlayerWithDeckCards(_playerId, deckCard);

            var ability = new InvokeSpecificCardAbility(
                AbilityName.Default,
                "TargetCard",
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.True);
        }

        [Test]
        public void InvokeSpecificCardAbility_CanActivate_ReturnsFalseWhenCardNotInDeck()
        {
            var otherCard = CreateInvocationCard("OtherCard", 500, 400);
            SetupPlayerWithDeckCards(_playerId, otherCard);

            var ability = new InvokeSpecificCardAbility(
                AbilityName.Default,
                "TargetCard",
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.False);
        }

        [Test]
        public void InvokeSpecificCardAbility_CanActivate_ReturnsFalseWhenFieldIsFull()
        {
            var deckCard = CreateInvocationCard("TargetCard", 500, 400);
            var fieldCard1 = CreateInvocationCard("Field1", 100, 100);
            var fieldCard2 = CreateInvocationCard("Field2", 100, 100);
            var fieldCard3 = CreateInvocationCard("Field3", 100, 100);
            var fieldCard4 = CreateInvocationCard("Field4", 100, 100);

            // Setup player with full field and card in deck
            var player = new Player(_playerId, new[] { deckCard });

            // Add 4 cards to field
            var handField = typeof(Player).GetField("_hand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var hand = (List<Card>)handField.GetValue(player);
            hand.Add(fieldCard1);
            hand.Add(fieldCard2);
            hand.Add(fieldCard3);
            hand.Add(fieldCard4);
            player.PlayCard(fieldCard1);
            player.PlayCard(fieldCard2);
            player.PlayCard(fieldCard3);
            player.PlayCard(fieldCard4);

            _playerRepository.SavePlayer(player);

            var ability = new InvokeSpecificCardAbility(
                AbilityName.Default,
                "TargetCard",
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.False);
        }

        [Test]
        public void InvokeSpecificCardAbility_Execute_MovesCardFromDeckToField()
        {
            var deckCard = CreateInvocationCard("TargetCard", 500, 400);
            SetupPlayerWithDeckCards(_playerId, deckCard);

            var ability = new InvokeSpecificCardAbility(
                AbilityName.Default,
                "TargetCard",
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Invoked TargetCard"));

            var player = _playerRepository.GetPlayer(_playerId);
            Assert.That(player.Field, Contains.Item(deckCard));
            Assert.That(player.Deck, Has.No.Member(deckCard));
        }

        [Test]
        public void InvokeSpecificCardAbility_Execute_FailsWhenFieldIsFull()
        {
            var deckCard = CreateInvocationCard("TargetCard", 500, 400);
            var fieldCard1 = CreateInvocationCard("Field1", 100, 100);
            var fieldCard2 = CreateInvocationCard("Field2", 100, 100);
            var fieldCard3 = CreateInvocationCard("Field3", 100, 100);
            var fieldCard4 = CreateInvocationCard("Field4", 100, 100);

            var player = new Player(_playerId, new[] { deckCard });
            var handField = typeof(Player).GetField("_hand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var hand = (List<Card>)handField.GetValue(player);
            hand.Add(fieldCard1);
            hand.Add(fieldCard2);
            hand.Add(fieldCard3);
            hand.Add(fieldCard4);
            player.PlayCard(fieldCard1);
            player.PlayCard(fieldCard2);
            player.PlayCard(fieldCard3);
            player.PlayCard(fieldCard4);

            _playerRepository.SavePlayer(player);

            var ability = new InvokeSpecificCardAbility(
                AbilityName.Default,
                "TargetCard",
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Field is full"));
        }

        [Test]
        public void InvokeSpecificCardAbility_Execute_FailsWhenCardNotInDeck()
        {
            var otherCard = CreateInvocationCard("OtherCard", 500, 400);
            SetupPlayerWithDeckCards(_playerId, otherCard);

            var ability = new InvokeSpecificCardAbility(
                AbilityName.Default,
                "TargetCard",
                _playerRepository
            );
            var context = CreateContext();

            var result = ability.Execute(context);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("TargetCard not in deck"));
        }

        #endregion

        #region SacrificeToInvokeAbility Tests

        [Test]
        public void SacrificeToInvokeAbility_Constructor_SetsProperties()
        {
            var ability = new SacrificeToInvokeAbility(_playerRepository);

            Assert.That(ability.Name, Is.EqualTo(AbilityName.SacrificeToInvoke));
            Assert.That(ability.Description, Is.EqualTo("Sacrifice a card to invoke another"));
        }

        [Test]
        public void SacrificeToInvokeAbility_CanActivate_ReturnsTrueWhenFieldHasCardAndHandHasInvocation()
        {
            var fieldCard = CreateInvocationCard("FieldCard", 500, 400);
            var handCard = CreateInvocationCard("HandCard", 1000, 800);
            SetupPlayerWithFieldAndHand(_playerId, new[] { fieldCard }, new[] { handCard });

            var ability = new SacrificeToInvokeAbility(_playerRepository);
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.True);
        }

        [Test]
        public void SacrificeToInvokeAbility_CanActivate_ReturnsFalseWhenFieldIsEmpty()
        {
            var handCard = CreateInvocationCard("HandCard", 1000, 800);
            SetupPlayerWithFieldAndHand(_playerId, new Card[] { }, new[] { handCard });

            var ability = new SacrificeToInvokeAbility(_playerRepository);
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.False);
        }

        [Test]
        public void SacrificeToInvokeAbility_CanActivate_ReturnsFalseWhenNoInvocationInHand()
        {
            var fieldCard = CreateInvocationCard("FieldCard", 500, 400);
            SetupPlayerWithFieldCards(_playerId, fieldCard);
            // No cards in hand

            var ability = new SacrificeToInvokeAbility(_playerRepository);
            var context = CreateContext();

            var result = ability.CanActivate(context);

            Assert.That(result, Is.False);
        }

        [Test]
        public void SacrificeToInvokeAbility_Execute_RequiresUserInput()
        {
            var fieldCard = CreateInvocationCard("FieldCard", 500, 400);
            var handCard = CreateInvocationCard("HandCard", 1000, 800);
            SetupPlayerWithFieldAndHand(_playerId, new[] { fieldCard }, new[] { handCard });

            var ability = new SacrificeToInvokeAbility(_playerRepository);
            var context = CreateContext();

            var result = ability.Execute(context);

            Assert.That(result.RequiresUserInput, Is.True);
            Assert.That(result.Message, Is.EqualTo("Select card to sacrifice and card to invoke"));
        }

        #endregion

        #region SacrificeAbilityFactory Tests

        [Test]
        public void SacrificeAbilityFactory_CreateSacrificeCard_ReturnsCorrectAbility()
        {
            var factory = new SacrificeAbilityFactory(_playerRepository);

            var ability = factory.CreateSacrificeCard(AbilityName.Default, "TestCard");

            Assert.That(ability, Is.TypeOf<SacrificeCardAbility>());
            Assert.That(ability.Description, Is.EqualTo("Sacrifice TestCard"));
        }

        [Test]
        public void SacrificeAbilityFactory_CreateInvokeSpecificCard_ReturnsCorrectAbility()
        {
            var factory = new SacrificeAbilityFactory(_playerRepository);

            var ability = factory.CreateInvokeSpecificCard(AbilityName.Default, "TestCard");

            Assert.That(ability, Is.TypeOf<InvokeSpecificCardAbility>());
            Assert.That(ability.Description, Is.EqualTo("Invoke TestCard from deck"));
        }

        [Test]
        public void SacrificeAbilityFactory_CreateSacrificeToInvoke_ReturnsCorrectAbility()
        {
            var factory = new SacrificeAbilityFactory(_playerRepository);

            var ability = factory.CreateSacrificeToInvoke();

            Assert.That(ability, Is.TypeOf<SacrificeToInvokeAbility>());
            Assert.That(ability.Description, Is.EqualTo("Sacrifice a card to invoke another"));
        }

        [Test]
        public void SacrificeAbilityFactory_AllFactoryMethods_ReturnValidAbilities()
        {
            var factory = new SacrificeAbilityFactory(_playerRepository);

            var abilities = new IAbility[]
            {
                factory.CreateSacrificeCard(AbilityName.Default, "Card1"),
                factory.CreateInvokeSpecificCard(AbilityName.Default, "Card2"),
                factory.CreateSacrificeToInvoke()
            };

            foreach (var ability in abilities)
            {
                Assert.That(ability, Is.Not.Null);
                Assert.That(ability.Description, Is.Not.Null.And.Not.Empty);
            }
        }

        #endregion
    }
}
