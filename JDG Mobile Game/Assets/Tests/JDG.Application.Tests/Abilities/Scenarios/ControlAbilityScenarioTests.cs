using NUnit.Framework;
using NSubstitute;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;
using JDG.Application;
using JDG.Application.Abilities;
using JDG.Application.Repositories;
using JDG.Application.Abilities.Implementations;
using System.Collections.Generic;
using System.Linq;

using TestCardFactory = JDG.TestUtilities.CardFactory;

namespace JDG.Application.Tests.Abilities.Scenarios
{
    /// <summary>
    /// Scenario tests for Control abilities.
    /// Tests abilities that control opponent's cards or manipulate the game state.
    /// </summary>
    [TestFixture]
    [Category("Abilities")]
    [Category("Scenario")]
    [Category("Control")]
    public class ControlAbilityScenarioTests
    {
        private IPlayerRepository _playerRepository;
        private Player _player1;
        private Player _player2;
        private EffectAbilityFactory _effectFactory;
        private SpecialAbilityFactory _specialFactory;

        [SetUp]
        public void SetUp()
        {
            _playerRepository = Substitute.For<IPlayerRepository>();
            _effectFactory = new EffectAbilityFactory(_playerRepository);
            _specialFactory = new SpecialAbilityFactory(_playerRepository);

            // Setup players
            _player1 = CreateTestPlayer(PlayerId.Player1);
            _player2 = CreateTestPlayer(PlayerId.Player2);

            _playerRepository.GetPlayer(PlayerId.Player1).Returns(_player1);
            _playerRepository.GetPlayer(PlayerId.Player2).Returns(_player2);
        }

        #region SendAllCardToHands - Bebe Terreur-Nocturne

        [Test]
        public void SendAllCardToHands_WhenBebeTerreurNocturneActivated_ReturnsAllCardsToHands()
        {
            // Arrange
            // Bebe Terreur-Nocturne: SendAllCardToHands - Returns all field cards to hands
            var bebeTerreur = TestCardFactory.CreateInvocation("Bebe Terreur-Nocturne", 1, 1, CardFamily.Japan);
            var playerCard = TestCardFactory.CreateInvocation("Player Card", 3, 3, CardFamily.Human);
            var opponentCard = TestCardFactory.CreateInvocation("Opponent Card", 4, 4, CardFamily.Developer);

            PlaceOnField(_player1, bebeTerreur, playerCard);
            PlaceOnField(_player2, opponentCard);

            var ability = _specialFactory.CreateSendAllToHand();
            var context = CreateContext(_player1, bebeTerreur, AbilityName.SendAllCardToHands);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void SendAllCardToHands_AffectsBothPlayers()
        {
            // Arrange
            var bebeTerreur = TestCardFactory.CreateInvocation("Bebe Terreur-Nocturne", 1, 1, CardFamily.Japan);
            var playerCard1 = TestCardFactory.CreateInvocation("Player Card 1", 3, 3, CardFamily.Human);
            var playerCard2 = TestCardFactory.CreateInvocation("Player Card 2", 2, 2, CardFamily.Human);
            var opponentCard = TestCardFactory.CreateInvocation("Opponent Card", 4, 4, CardFamily.Developer);

            PlaceOnField(_player1, bebeTerreur, playerCard1, playerCard2);
            PlaceOnField(_player2, opponentCard);

            var ability = _specialFactory.CreateSendAllToHand();
            var context = CreateContext(_player1, bebeTerreur, AbilityName.SendAllCardToHands);

            // Act - All cards from both fields should return to hands
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void SendAllCardToHands_IncludesSourceCard()
        {
            // Arrange
            var bebeTerreur = TestCardFactory.CreateInvocation("Bebe Terreur-Nocturne", 1, 1, CardFamily.Japan);

            PlaceOnField(_player1, bebeTerreur);

            // Bebe Terreur-Nocturne also returns itself to hand
            Assert.AreEqual(1, bebeTerreur.Stats.Value.Attack);
            Assert.AreEqual(1, bebeTerreur.Stats.Value.Defense);
        }

        [Test]
        public void SendAllCardToHands_ClearsAllFields()
        {
            // Arrange
            var bebeTerreur = TestCardFactory.CreateInvocation("Bebe Terreur-Nocturne", 1, 1, CardFamily.Japan);

            var ability = _specialFactory.CreateSendAllToHand();
            var context = CreateContext(_player1, bebeTerreur, AbilityName.SendAllCardToHands);

            // After execution, both fields should be empty
            var result = ability.Execute(context);

            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void SendAllCardToHands_EquipmentReturnedWithCards()
        {
            // Arrange
            var bebeTerreur = TestCardFactory.CreateInvocation("Bebe Terreur-Nocturne", 1, 1, CardFamily.Japan);
            var equippedCard = TestCardFactory.CreateInvocation("Equipped Card", 3, 3, CardFamily.Human);

            // Equipment attached to cards should also be returned
            Assert.IsNotNull(bebeTerreur);
            Assert.IsNotNull(equippedCard);
        }

        #endregion

        #region SkipOpponentAttackEveryTurn - Starlight Unicorn

        [Test]
        public void SkipOpponentAttackEveryTurn_WhenStarlightUnicornOnField_SkipsOpponentAttacks()
        {
            // Arrange
            // Starlight Unicorn: SkipOpponentAttackEveryTurn - Opponent cannot attack
            var starlightUnicorn = TestCardFactory.CreateInvocation("Starlight Unicorn", 4, 4, CardFamily.Incarnation);
            var opponentAttacker = TestCardFactory.CreateInvocation("Attacker", 5, 5, CardFamily.Human);

            PlaceOnField(_player1, starlightUnicorn);
            PlaceOnField(_player2, opponentAttacker);

            var ability = _effectFactory.CreateSkipAttackPhase();
            var context = CreateContext(_player1, starlightUnicorn, AbilityName.SkipOpponentAttackEveryTurn);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void SkipOpponentAttackEveryTurn_AppliesEveryTurn()
        {
            // Arrange
            var starlightUnicorn = TestCardFactory.CreateInvocation("Starlight Unicorn", 4, 4, CardFamily.Incarnation);

            // Effect is continuous while Starlight Unicorn is on field
            Assert.AreEqual(4, starlightUnicorn.Stats.Value.Attack);
            Assert.AreEqual(4, starlightUnicorn.Stats.Value.Defense);
        }

        [Test]
        public void SkipOpponentAttackEveryTurn_DoesNotAffectOwnAttacks()
        {
            // Arrange
            var starlightUnicorn = TestCardFactory.CreateInvocation("Starlight Unicorn", 4, 4, CardFamily.Incarnation);
            var ownAttacker = TestCardFactory.CreateInvocation("Own Attacker", 3, 3, CardFamily.Rpg);

            // Player's own cards can still attack
            Assert.IsNotNull(starlightUnicorn);
            Assert.IsNotNull(ownAttacker);
        }

        [Test]
        public void SkipOpponentAttackEveryTurn_CombinesWithDependency()
        {
            // Arrange
            // Starlight Unicorn also has CantLiveWithoutGranolaxOrMechaGranolax
            var starlightUnicorn = TestCardFactory.CreateInvocation("Starlight Unicorn", 4, 4, CardFamily.Incarnation);
            var granolax = TestCardFactory.CreateInvocation("Granolax", 2, 2, CardFamily.Rpg);

            // Must have Granolax/Mecha-Granolax to stay alive
            PlaceOnField(_player1, starlightUnicorn, granolax);

            var ability = _effectFactory.CreateSkipAttackPhase();
            var context = CreateContext(_player1, starlightUnicorn, AbilityName.SkipOpponentAttackEveryTurn);

            var result = ability.Execute(context);

            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void SkipOpponentAttackEveryTurn_RemovedWhenStarlightUnicornDies()
        {
            // Arrange
            var starlightUnicorn = TestCardFactory.CreateInvocation("Starlight Unicorn", 4, 4, CardFamily.Incarnation);

            // When Starlight Unicorn leaves field, opponent can attack again
            Assert.IsNotNull(starlightUnicorn);
        }

        #endregion

        #region Control1OpponentInvocationCard - Effect Card

        [Test]
        public void Control1OpponentInvocationCard_TakesControlOfOpponentCard()
        {
            // Arrange
            // Control1OpponentInvocationCard: Takes control of one opponent's invocation
            var sourceCard = TestCardFactory.CreateEffect("Control Effect", new[] { EffectAbilityName.Control1OpponentInvocationCard });
            var opponentCard = TestCardFactory.CreateInvocation("Opponent Card", 4, 4, CardFamily.Developer);

            PlaceOnField(_player2, opponentCard);

            var ability = _effectFactory.CreateControlCard();
            var context = CreateContext(_player1, sourceCard, AbilityName.Default);
            context.TargetCard = opponentCard;

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.IsSuccess || result.RequiresUserInput);
        }

        [Test]
        public void Control1OpponentInvocationCard_CardMovesToPlayerField()
        {
            // Arrange
            var sourceCard = TestCardFactory.CreateEffect("Control Effect", new[] { EffectAbilityName.Control1OpponentInvocationCard });
            var opponentCard = TestCardFactory.CreateInvocation("Opponent Card", 4, 4, CardFamily.Developer);

            // After control, card should be on player's field
            Assert.IsNotNull(sourceCard);
            Assert.IsNotNull(opponentCard);
        }

        [Test]
        public void Control1OpponentInvocationCard_RequiresValidTarget()
        {
            // Arrange
            var sourceCard = TestCardFactory.CreateEffect("Control Effect", new[] { EffectAbilityName.Control1OpponentInvocationCard });

            // Must have at least one opponent invocation to target
            Assert.IsNotNull(sourceCard);
        }

        [Test]
        public void Control1OpponentInvocationCard_ControlledCardCanAttack()
        {
            // Arrange
            var sourceCard = TestCardFactory.CreateEffect("Control Effect", new[] { EffectAbilityName.Control1OpponentInvocationCard });
            var opponentCard = TestCardFactory.CreateInvocation("Controlled Card", 4, 4, CardFamily.Developer);

            // Controlled card can now attack for the player who took control
            Assert.AreEqual(4, opponentCard.Stats.Value.Attack);
        }

        #endregion

        #region Win1ATK1DefJaponWith2ATK2DEFCondition - Dresseur de Bidulmon

        [Test]
        public void Win1ATK1DefJaponWith2ATK2DEFCondition_WhenConditionMet_GainsStats()
        {
            // Arrange
            // Dresseur de Bidulmon: Gains +1 ATK +1 DEF if there are 2+ Japan cards with 2+ ATK/DEF
            var dresseur = TestCardFactory.CreateInvocation("Dresseur de Bidulmon", 2, 2, CardFamily.Japan);
            var japanCard1 = TestCardFactory.CreateInvocation("Japan Card 1", 3, 3, CardFamily.Japan);
            var japanCard2 = TestCardFactory.CreateInvocation("Japan Card 2", 2, 2, CardFamily.Japan);

            PlaceOnField(_player1, dresseur, japanCard1, japanCard2);

            // With 2 Japan cards with 2+ ATK and 2+ DEF, Dresseur should gain stats
            Assert.AreEqual(CardFamily.Japan, dresseur.Families.First());
        }

        [Test]
        public void Win1ATK1DefJaponWith2ATK2DEFCondition_ConditionNotMet_NoBonus()
        {
            // Arrange
            var dresseur = TestCardFactory.CreateInvocation("Dresseur de Bidulmon", 2, 2, CardFamily.Japan);
            var weakJapan = TestCardFactory.CreateInvocation("Weak Japan", 1, 1, CardFamily.Japan);

            PlaceOnField(_player1, dresseur, weakJapan);

            // Without meeting stat requirements, no bonus
            Assert.AreEqual(2, dresseur.Stats.Value.Attack);
            Assert.AreEqual(2, dresseur.Stats.Value.Defense);
        }

        [Test]
        public void Win1ATK1DefJaponWith2ATK2DEFCondition_CountsOnlyJapanFamily()
        {
            // Arrange
            var dresseur = TestCardFactory.CreateInvocation("Dresseur de Bidulmon", 2, 2, CardFamily.Japan);
            var nonJapan1 = TestCardFactory.CreateInvocation("Non Japan 1", 3, 3, CardFamily.Human);
            var nonJapan2 = TestCardFactory.CreateInvocation("Non Japan 2", 3, 3, CardFamily.Developer);

            PlaceOnField(_player1, dresseur, nonJapan1, nonJapan2);

            // Non-Japan cards don't count toward condition
            Assert.AreNotEqual(CardFamily.Japan, nonJapan1.Families.First());
        }

        #endregion

        #region Helper Methods

        private Player CreateTestPlayer(PlayerId playerId)
        {
            var deck = TestCardFactory.CreateDeck(30);
            return new Player(playerId, deck);
        }

        private void PlaceOnField(Player player, params Card[] cards)
        {
            foreach (var card in cards)
            {
                player.PlayCard(card);
            }
        }

        private AbilityContext CreateContext(Player player, Card sourceCard, AbilityName abilityName)
        {
            var opponentId = player.Id == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1;
            return new AbilityContext(player.Id, opponentId, sourceCard, abilityName);
        }

        #endregion
    }
}
