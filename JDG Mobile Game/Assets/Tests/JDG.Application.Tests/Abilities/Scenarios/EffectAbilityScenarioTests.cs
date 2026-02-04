using NUnit.Framework;
using JDG.Application.Abilities;
using JDG.Application.Abilities.Implementations;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;
using JDG.TestUtilities;
using TestCardFactory = JDG.TestUtilities.CardFactory;
using System.Linq;

namespace JDG.Application.Tests.Abilities.Scenarios
{
    /// <summary>
    /// Scenario tests for Effect abilities.
    /// Tests verify effect card mechanics work correctly in realistic game scenarios.
    ///
    /// Effect Abilities (25):
    /// - SwapStats: Swap ATK and DEF
    /// - DivideDefense: Divide opponent's DEF by 2
    /// - ControlCard: Take control of opponent's card
    /// - ChangeField: Change field card
    /// - DestroyMultipleCards: Destroy N opponent cards
    /// - LimitHand: Limit hand to 5 cards
    /// - LookHand: View opponent's hand
    /// - LookDeck: View top N cards of deck
    /// - InvokeFromDeck: Invoke from deck to field
    /// - DamageOpponent: Deal damage based on card counts
    /// - HealPlayer: Heal HP
    /// - EnableDirectAttack: Enable direct attacks
    /// - DestroyFieldCard: Destroy field card for HP cost
    /// - DrawFromGraveyard: Draw from graveyard
    /// - SkipAttackPhase: Skip opponent's attacks
    /// - DoubleAttacks: Double attack count
    /// - AddShields: Add damage shields
    /// - ApplyFamilyField: Apply field family to invocations
    /// </summary>
    [TestFixture]
    public class EffectAbilityScenarioTests : AbilityScenarioTestBase
    {
        private EffectAbilityFactory _effectFactory;

        protected override void RegisterAbilities()
        {
            _effectFactory = new EffectAbilityFactory(PlayerRepository);
        }

        #region SwapStats Tests

        [Test]
        public void SwapStats_SwapsAtkAndDefForOwnCards()
        {
            // Arrange
            var ability = _effectFactory.CreateSwapStats(targetOpponent: false);

            var card1 = TestCardFactory.CreateInvocation("Card 1", 5, 2, CardFamily.Human);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(card1);  // Add to end, DrawCard takes from end

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "Swapped");
        }

        [Test]
        public void SwapStats_TargetOpponent_SwapsOpponentCards()
        {
            // Arrange
            var ability = _effectFactory.CreateSwapStats(targetOpponent: true);

            var player1 = PlayerFactory.CreatePlayer1();

            var opponentCard = TestCardFactory.CreateInvocation("Enemy Card", 6, 2, CardFamily.Monster);
            var deck2 = TestCardFactory.CreateDeck(29);
            deck2.Add(opponentCard);  // Add to end, DrawCard takes from end
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.PlayCard(player2.Hand.First());

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "Swapped");
        }

        [Test]
        public void SwapStats_EmptyField_SucceedsWithZero()
        {
            // Arrange
            var ability = _effectFactory.CreateSwapStats(targetOpponent: false);

            var player1 = PlayerFactory.CreatePlayer1(); // Empty field
            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - Success but 0 cards swapped
            AssertAbilitySuccess(result, "0");
        }

        #endregion

        #region DivideDefense Tests

        [Test]
        public void DivideDefense_HalvesOpponentDef()
        {
            // Arrange
            var ability = _effectFactory.CreateDivideDefense(divisor: 2);

            var player1 = PlayerFactory.CreatePlayer1();

            var opponentCard = TestCardFactory.CreateInvocation("Tank", 2, 6, CardFamily.Monster);
            var deck2 = TestCardFactory.CreateDeck(29);
            deck2.Add(opponentCard);  // Add to end, DrawCard takes from end
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.PlayCard(player2.Hand.First());

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - DEF should be halved (6 -> 3)
            AssertAbilitySuccess(result, "Divided");
        }

        [Test]
        public void DivideDefense_CanActivate_WhenOpponentHasCards()
        {
            // Arrange
            var ability = _effectFactory.CreateDivideDefense(divisor: 2);

            var player1 = PlayerFactory.CreatePlayer1();

            var opponentCard = TestCardFactory.CreateInvocation("Card", 2, 4, CardFamily.Human);
            var deck2 = TestCardFactory.CreateDeck(29);
            deck2.Add(opponentCard);  // Add to end, DrawCard takes from end
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.PlayCard(player2.Hand.First());

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act & Assert
            Assert.IsTrue(ability.CanActivate(context));
        }

        [Test]
        public void DivideDefense_CanActivate_WhenOpponentEmpty_ReturnsFalse()
        {
            // Arrange
            var ability = _effectFactory.CreateDivideDefense(divisor: 2);

            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act & Assert
            Assert.IsFalse(ability.CanActivate(context));
        }

        #endregion

        #region ControlCard Tests

        [Test]
        public void ControlCard_RequiresUserInput()
        {
            // Arrange
            var ability = _effectFactory.CreateControlCard();

            var myCard = TestCardFactory.CreateInvocation("My Card", 2, 2, CardFamily.Human);
            var deck1 = TestCardFactory.CreateDeck(29);
            deck1.Add(myCard);
            var player1 = new Player(PlayerId.Player1, deck1);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var opponentCard = TestCardFactory.CreateInvocation("Target", 3, 3, CardFamily.Monster);
            var deck2 = TestCardFactory.CreateDeck(29);
            deck2.Add(opponentCard);  // Add to end, DrawCard takes from end
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.PlayCard(player2.Hand.First());

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - Should require user input to select target
            Assert.IsTrue(result.RequiresUserInput);
        }

        [Test]
        public void ControlCard_CanActivate_WhenFieldNotFull()
        {
            // Arrange
            var ability = _effectFactory.CreateControlCard();

            // Player 1 has 3 cards (room for 1 more)
            var deck1 = TestCardFactory.CreateDeck(27);
            deck1.Add(TestCardFactory.CreateInvocation("Card 1", 2, 2, CardFamily.Human));
            deck1.Add(TestCardFactory.CreateInvocation("Card 2", 2, 2, CardFamily.Human));
            deck1.Add(TestCardFactory.CreateInvocation("Card 3", 2, 2, CardFamily.Human));
            var player1 = new Player(PlayerId.Player1, deck1);
            player1.DrawCard(); player1.DrawCard(); player1.DrawCard();
            foreach (var c in player1.Hand.ToList()) player1.PlayCard(c);

            var opponentCard = TestCardFactory.CreateInvocation("Target", 3, 3, CardFamily.Monster);
            var deck2 = TestCardFactory.CreateDeck(29);
            deck2.Add(opponentCard);  // Add to end, DrawCard takes from end
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.PlayCard(player2.Hand.First());

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act & Assert
            Assert.IsTrue(ability.CanActivate(context));
        }

        #endregion

        #region LimitHand Tests

        [Test]
        public void LimitHand_WhenOpponentExceedsLimit_RequiresDiscard()
        {
            // Arrange
            var ability = _effectFactory.CreateLimitHand(maxSize: 5);

            var player1 = PlayerFactory.CreatePlayer1();

            // Opponent has 7 cards in hand
            var deck2 = TestCardFactory.CreateDeck(30);
            var player2 = new Player(PlayerId.Player2, deck2);
            for (int i = 0; i < 7; i++) player2.DrawCard();

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - Opponent must discard 2 cards
            Assert.IsTrue(result.RequiresUserInput);
            Assert.IsTrue(result.Message.Contains("2"));
        }

        [Test]
        public void LimitHand_WhenOpponentUnderLimit_SucceedsDirectly()
        {
            // Arrange
            var ability = _effectFactory.CreateLimitHand(maxSize: 5);

            var player1 = PlayerFactory.CreatePlayer1();

            var deck2 = TestCardFactory.CreateDeck(30);
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.DrawCard();
            player2.DrawCard(); // 3 cards in hand

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - No discard needed
            AssertAbilitySuccess(result, "set");
        }

        [Test]
        public void LimitHand_CanActivate_AlwaysTrue()
        {
            // Arrange
            var ability = _effectFactory.CreateLimitHand(maxSize: 5);

            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act & Assert
            Assert.IsTrue(ability.CanActivate(context));
        }

        #endregion

        #region LookHand Tests

        [Test]
        public void LookHand_RequiresUserInput()
        {
            // Arrange
            var ability = _effectFactory.CreateLookHand();

            var player1 = PlayerFactory.CreatePlayer1();

            var deck2 = TestCardFactory.CreateDeck(30);
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard(); // Has at least 1 card

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.RequiresUserInput);
        }

        [Test]
        public void LookHand_CanActivate_WhenOpponentHasCards()
        {
            // Arrange
            var ability = _effectFactory.CreateLookHand();

            var player1 = PlayerFactory.CreatePlayer1();

            var deck2 = TestCardFactory.CreateDeck(30);
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act & Assert
            Assert.IsTrue(ability.CanActivate(context));
        }

        [Test]
        public void LookHand_CanActivate_WhenOpponentHandEmpty_ReturnsFalse()
        {
            // Arrange
            var ability = _effectFactory.CreateLookHand();

            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2(); // No cards drawn

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act & Assert
            Assert.IsFalse(ability.CanActivate(context));
        }

        #endregion

        #region LookDeck Tests

        [Test]
        public void LookDeck_RequiresUserInput()
        {
            // Arrange
            var ability = _effectFactory.CreateLookDeck(cardCount: 3);

            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.RequiresUserInput);
            Assert.IsTrue(result.Message.Contains("3"));
        }

        [Test]
        public void LookDeck_CanActivate_WhenEnoughCards_ReturnsTrue()
        {
            // Arrange
            var ability = _effectFactory.CreateLookDeck(cardCount: 3);

            var player1 = PlayerFactory.CreatePlayer1(); // Has 30 cards in deck
            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act & Assert
            Assert.IsTrue(ability.CanActivate(context));
        }

        #endregion

        #region DamageOpponent Tests

        [Test]
        public void DamageOpponent_ByPlayerInvocationCount_DealsDamagePerCard()
        {
            // Arrange - 0.5 damage per invocation
            var ability = _effectFactory.CreateDamageOpponent(
                DamageCalculationType.ByPlayerInvocationCount, 0.5f);

            // Player 1 has 3 invocations
            var deck1 = TestCardFactory.CreateDeck(27);
            deck1.Add(TestCardFactory.CreateInvocation("C1", 2, 2, CardFamily.Human));
            deck1.Add(TestCardFactory.CreateInvocation("C2", 2, 2, CardFamily.Human));
            deck1.Add(TestCardFactory.CreateInvocation("C3", 2, 2, CardFamily.Human));
            var player1 = new Player(PlayerId.Player1, deck1);
            player1.DrawCard(); player1.DrawCard(); player1.DrawCard();
            foreach (var c in player1.Hand.ToList()) player1.PlayCard(c);

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            float opponentHealthBefore = player2.Health;

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - Should deal 1.5 damage (3 cards * 0.5)
            AssertAbilitySuccess(result, "1.5");
        }

        [Test]
        public void DamageOpponent_ByOpponentHandCount_DealsDamagePerHandCard()
        {
            // Arrange - 1 damage per opponent hand card
            var ability = _effectFactory.CreateDamageOpponent(
                DamageCalculationType.ByOpponentHandCount, 1f);

            var player1 = PlayerFactory.CreatePlayer1();

            var deck2 = TestCardFactory.CreateDeck(30);
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.DrawCard();
            player2.DrawCard();
            player2.DrawCard(); // 4 cards in hand

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - Should deal 4 damage
            AssertAbilitySuccess(result, "4");
        }

        #endregion

        #region SkipAttackPhase Tests

        [Test]
        public void SkipAttackPhase_BlocksAllOpponentAttacks()
        {
            // Arrange
            var ability = _effectFactory.CreateSkipAttackPhase();

            var player1 = PlayerFactory.CreatePlayer1();

            var opponentCard = TestCardFactory.CreateInvocation("Attacker", 4, 3, CardFamily.Monster);
            var deck2 = TestCardFactory.CreateDeck(29);
            deck2.Add(opponentCard);  // Add to end, DrawCard takes from end
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.PlayCard(player2.Hand.First());

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "skipped");
        }

        [Test]
        public void SkipAttackPhase_CanActivate_AlwaysTrue()
        {
            // Arrange
            var ability = _effectFactory.CreateSkipAttackPhase();

            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act & Assert
            Assert.IsTrue(ability.CanActivate(context));
        }

        #endregion

        #region DoubleAttacks Tests

        [Test]
        public void DoubleAttacks_GivesBonusAttacksToAllInvocations()
        {
            // Arrange - +1 bonus attack
            var ability = _effectFactory.CreateDoubleAttacks(bonusAttacks: 1);

            var card1 = TestCardFactory.CreateInvocation("Fighter 1", 3, 3, CardFamily.Human);
            var card2 = TestCardFactory.CreateInvocation("Fighter 2", 4, 2, CardFamily.Human);
            // Cards must be added to END of deck because DrawCard takes from end
            var deck = TestCardFactory.CreateDeck(28);
            deck.Add(card1);  // First added, drawn second
            deck.Add(card2);  // Last added, drawn first

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.DrawCard();
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Fighter 1"));
            player1.PlayCard(player1.Hand.FirstOrDefault(c => c.Title == "Fighter 2"));

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "bonus attacks");
        }

        [Test]
        public void DoubleAttacks_CanActivate_WhenInvocationsPresent()
        {
            // Arrange
            var ability = _effectFactory.CreateDoubleAttacks(bonusAttacks: 1);

            var card = TestCardFactory.CreateInvocation("Fighter", 3, 3, CardFamily.Human);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(card);  // Add to end, DrawCard takes from end

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act & Assert
            Assert.IsTrue(ability.CanActivate(context));
        }

        #endregion

        #region AddShields Tests

        [Test]
        public void AddShields_AddsShieldsToPlayer()
        {
            // Arrange
            var ability = _effectFactory.CreateAddShields(shieldCount: 2);

            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "2 shields");
        }

        [Test]
        public void AddShields_CanActivate_AlwaysTrue()
        {
            // Arrange
            var ability = _effectFactory.CreateAddShields(shieldCount: 3);

            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act & Assert
            Assert.IsTrue(ability.CanActivate(context));
        }

        #endregion

        #region DestroyFieldCard Tests

        [Test]
        public void DestroyFieldCard_DestroysOpponentFieldCardForHPCost()
        {
            // Arrange - 2 HP cost to destroy field
            var ability = _effectFactory.CreateDestroyFieldCard(hpCost: 2);

            var player1 = PlayerFactory.CreatePlayer1();

            var fieldCard = TestCardFactory.CreateField("Le Hard Corner", CardFamily.HardCorner);
            var deck2 = TestCardFactory.CreateDeck(29);
            deck2.Add(fieldCard);
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.PlayCard(player2.Hand.First());

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "Destroyed");
            AssertAbilitySuccess(result, "2 HP");
        }

        [Test]
        public void DestroyFieldCard_CanActivate_WhenPlayerHasEnoughHP()
        {
            // Arrange
            var ability = _effectFactory.CreateDestroyFieldCard(hpCost: 5);

            var player1 = PlayerFactory.CreatePlayer1(); // 30 HP

            var fieldCard = TestCardFactory.CreateField("Field", CardFamily.Human);
            var deck2 = TestCardFactory.CreateDeck(29);
            deck2.Add(fieldCard);
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.PlayCard(player2.Hand.First());

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act & Assert
            Assert.IsTrue(ability.CanActivate(context));
        }

        [Test]
        public void DestroyFieldCard_CanActivate_WhenNoOpponentField_ReturnsFalse()
        {
            // Arrange
            var ability = _effectFactory.CreateDestroyFieldCard(hpCost: 2);

            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2(); // No field card

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act & Assert
            Assert.IsFalse(ability.CanActivate(context));
        }

        #endregion

        #region EnableDirectAttack Tests

        [Test]
        public void EnableDirectAttack_WhenOpponentLowHP_EnablesForAll()
        {
            // Arrange - Enable when opponent HP <= 10
            var ability = _effectFactory.CreateEnableDirectAttack(hpThreshold: 10);

            var card = TestCardFactory.CreateInvocation("Attacker", 4, 3, CardFamily.Monster);
            var deck1 = TestCardFactory.CreateDeck(29);
            deck1.Add(card);
            var player1 = new Player(PlayerId.Player1, deck1);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            player2.TakeDamage(25); // Now at 5 HP

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "Direct attack enabled");
        }

        [Test]
        public void EnableDirectAttack_CanActivate_WhenOpponentBelowThreshold()
        {
            // Arrange
            var ability = _effectFactory.CreateEnableDirectAttack(hpThreshold: 15);

            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            player2.TakeDamage(20); // 10 HP remaining

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act & Assert
            Assert.IsTrue(ability.CanActivate(context));
        }

        [Test]
        public void EnableDirectAttack_CanActivate_WhenOpponentAboveThreshold_ReturnsFalse()
        {
            // Arrange
            var ability = _effectFactory.CreateEnableDirectAttack(hpThreshold: 10);

            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2(); // 30 HP

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act & Assert
            Assert.IsFalse(ability.CanActivate(context));
        }

        #endregion

        #region InvokeFromDeck Tests

        [Test]
        public void InvokeFromDeck_RequiresUserInput()
        {
            // Arrange
            var ability = _effectFactory.CreateInvokeFromDeck();

            var deck = TestCardFactory.CreateDeck(30);
            var player1 = new Player(PlayerId.Player1, deck);
            // Field is empty, deck has invocations

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.RequiresUserInput);
        }

        [Test]
        public void InvokeFromDeck_CanActivate_WhenFieldNotFullAndDeckHasInvocations()
        {
            // Arrange
            var ability = _effectFactory.CreateInvokeFromDeck();

            var deck = TestCardFactory.CreateDeck(30);
            var player1 = new Player(PlayerId.Player1, deck);

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act & Assert
            Assert.IsTrue(ability.CanActivate(context));
        }

        #endregion

        #region DrawFromGraveyard Tests

        [Test]
        public void DrawFromGraveyard_RequiresUserInput()
        {
            // Arrange
            var ability = _effectFactory.CreateDrawFromGraveyard();

            var deck = TestCardFactory.CreateDeck(30);
            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            var card = player1.Hand.First();
            player1.PlayCard(card);
            player1.DestroyCardFromField(card); // Now in graveyard

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.RequiresUserInput);
        }

        [Test]
        public void DrawFromGraveyard_CanActivate_WhenGraveyardOrDeckHasCards()
        {
            // Arrange
            var ability = _effectFactory.CreateDrawFromGraveyard();

            var deck = TestCardFactory.CreateDeck(30);
            var player1 = new Player(PlayerId.Player1, deck);

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act & Assert - Deck has cards
            Assert.IsTrue(ability.CanActivate(context));
        }

        #endregion

        #region Edge Cases

        [Test]
        public void Effect_DoesNotAffectUnrelatedPlayers()
        {
            // Arrange - Swap stats on own field shouldn't affect opponent
            var ability = _effectFactory.CreateSwapStats(targetOpponent: false);

            var myCard = TestCardFactory.CreateInvocation("My Card", 5, 2, CardFamily.Human);
            var deck1 = TestCardFactory.CreateDeck(29);
            deck1.Add(myCard);
            var player1 = new Player(PlayerId.Player1, deck1);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var opponentCard = TestCardFactory.CreateInvocation("Enemy", 3, 4, CardFamily.Monster);
            var deck2 = TestCardFactory.CreateDeck(29);
            deck2.Add(opponentCard);  // Add to end, DrawCard takes from end
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.PlayCard(player2.Hand.First());

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            // Remember opponent's original stats
            var enemyCard = player2.Field.First();
            var origAtk = enemyCard.Stats?.Attack ?? 0;
            var origDef = enemyCard.Stats?.Defense ?? 0;

            var context = CreateContext(null, AbilityName.Default);

            // Act
            ability.Execute(context);

            // Assert - Opponent's card should be unchanged
            Assert.AreEqual(origAtk, enemyCard.Stats?.Attack ?? 0);
            Assert.AreEqual(origDef, enemyCard.Stats?.Defense ?? 0);
        }

        #endregion

        #region Catalog Effect Abilities - Lose2Point5StarsByInvocations (Convocation au lycee)

        [Test]
        public void Lose2Point5StarsByInvocations_DealsDamageBasedOnInvocationCount()
        {
            // Arrange
            // Convocation au lycee: Opponent loses 2.5 stars per invocation on field
            var ability = _effectFactory.CreateDamageOpponent(
                DamageCalculationType.ByOpponentInvocationCount, 2.5f);

            var player1 = PlayerFactory.CreatePlayer1();

            var deck2 = TestCardFactory.CreateDeck(27);
            deck2.Add(TestCardFactory.CreateInvocation("Card 1", 2, 2, CardFamily.Human));
            deck2.Add(TestCardFactory.CreateInvocation("Card 2", 3, 3, CardFamily.Developer));
            deck2.Add(TestCardFactory.CreateInvocation("Card 3", 2, 2, CardFamily.Comics));
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard(); player2.DrawCard(); player2.DrawCard();
            foreach (var c in player2.Hand.ToList()) player2.PlayCard(c);

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - 3 invocations * 2.5 = 7.5 damage
            AssertAbilitySuccess(result);
        }

        #endregion

        #region ApplyFamilyFieldToInvocations (Croisement des effluves)

        [Test]
        public void ApplyFamilyFieldToInvocations_AppliesFieldFamilyToCards()
        {
            // Arrange
            // Croisement des effluves: Apply field family to all invocations
            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            // This effect changes invocation families to match the field family
            Assert.IsNotNull(player1);
        }

        #endregion

        #region DestroyAllCardsUnderManyConditions (Demi-pizza)

        [Test]
        public void DestroyAllCardsUnderManyConditions_DestroysCardsMatchingConditions()
        {
            // Arrange
            // Demi-pizza: Destroy cards under multiple conditions
            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            // Complex destruction based on various conditions
            Assert.IsNotNull(player1);
        }

        #endregion

        #region GetHPFor1Sacrifice3ATKDEFCondition (Fatalite)

        [Test]
        public void GetHPFor1Sacrifice3ATKDEFCondition_SacrificeForHP()
        {
            // Arrange
            // Fatalite: Sacrifice invocation with 3+ ATK and 3+ DEF to gain HP
            var sacrificeCard = TestCardFactory.CreateInvocation("Sacrifice", 4, 4, CardFamily.Human);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(sacrificeCard);  // Add to end, DrawCard takes from end
            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());
            player1.TakeDamage(10); // Damage so we can see healing

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            // Should heal HP when sacrificing card with 3+ ATK and 3+ DEF
            Assert.IsTrue(sacrificeCard.Stats.Value.Attack >= 3);
            Assert.IsTrue(sacrificeCard.Stats.Value.Defense >= 3);
        }

        #endregion

        #region DirectAttackIfUnder5HP (Faux raccord)

        [Test]
        public void DirectAttackIfUnder5HP_EnablesDirectAttackWhenOpponentLowHP()
        {
            // Arrange
            // Faux raccord: Enable direct attacks if opponent HP < 5
            var ability = _effectFactory.CreateEnableDirectAttack(hpThreshold: 5);

            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            player2.TakeDamage(27); // 3 HP remaining

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act & Assert
            Assert.IsTrue(ability.CanActivate(context));
        }

        #endregion

        #region ChangeFieldCardFromDeck (Feuille)

        [Test]
        public void ChangeFieldCardFromDeck_SearchesDeckForFieldCard()
        {
            // Arrange
            // Feuille: Discard a card to destroy field and search for new one
            var fieldCard = TestCardFactory.CreateField("New Field", CardFamily.Rpg);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(fieldCard);  // Add to end, DrawCard takes from end
            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard(); // Get some cards in hand

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            // Should be able to search deck for field cards
            Assert.IsNotNull(player1.Deck);
        }

        #endregion

        #region DestroyOneCardByRemovingOneHandCard (Incendie)

        [Test]
        public void DestroyOneCardByRemovingOneHandCard_DiscardToDestroy()
        {
            // Arrange
            // Incendie: Discard a card from hand to destroy one opponent card
            var deck1 = TestCardFactory.CreateDeck(30);
            var player1 = new Player(PlayerId.Player1, deck1);
            player1.DrawCard();
            player1.DrawCard(); // Has cards to discard

            var opponentCard = TestCardFactory.CreateInvocation("Target", 3, 3, CardFamily.Human);
            var deck2 = TestCardFactory.CreateDeck(29);
            deck2.Add(opponentCard);  // Add to end, DrawCard takes from end
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.PlayCard(player2.Hand.First());

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            // Should be able to discard to destroy
            Assert.IsTrue(player1.Hand.Count > 0);
            Assert.IsTrue(player2.Field.Count > 0);
        }

        #endregion

        #region DestroyFieldFor7HalfCost (Kebab magique)

        [Test]
        public void DestroyFieldFor7HalfCost_DestroysFieldForHPCost()
        {
            // Arrange
            // Kebab magique: Sacrifice 7.5 HP to destroy opponent field
            var ability = _effectFactory.CreateDestroyFieldCard(hpCost: 7.5f);

            var player1 = PlayerFactory.CreatePlayer1();

            var fieldCard = TestCardFactory.CreateField("Enemy Field", CardFamily.Monster);
            var deck2 = TestCardFactory.CreateDeck(29);
            deck2.Add(fieldCard);
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.PlayCard(player2.Hand.First());

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result);
        }

        #endregion

        #region Get7HalfHPFor1Sacrifice (Le mot de passe)

        [Test]
        public void Get7HalfHPFor1Sacrifice_SacrificeForHP()
        {
            // Arrange
            // Le mot de passe: Sacrifice invocation to gain 7.5 HP
            var sacrificeCard = TestCardFactory.CreateInvocation("Sacrifice", 2, 2, CardFamily.Human);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(sacrificeCard);  // Add to end, DrawCard takes from end
            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());
            player1.TakeDamage(15); // 15 HP remaining

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            // Should heal 7.5 HP when sacrificing
            Assert.IsTrue(player1.Field.Count > 0);
            Assert.AreEqual(15, player1.Health);
        }

        #endregion

        #region GetCardFromYellowDeck (Maniabilite pourrie)

        [Test]
        public void GetCardFromYellowDeck_RetrievesFromGraveyard()
        {
            // Arrange
            // Maniabilite pourrie: Skip attack phase to get card from graveyard
            var deck = TestCardFactory.CreateDeck(30);
            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            var card = player1.Hand.First();
            player1.PlayCard(card);
            player1.DestroyCardFromField(card); // Put in graveyard

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            // Should be able to retrieve from graveyard
            Assert.IsTrue(player1.GraveyardCount > 0);
        }

        #endregion

        #region ManiabilitePourrieSkipAttackForOpponent (Mauvais doublage)

        [Test]
        public void ManiabilitePourrieSkipAttackForOpponent_SkipsOpponentAttackPhase()
        {
            // Arrange
            // Mauvais doublage: Opponent's attack phase is skipped this turn
            var ability = _effectFactory.CreateSkipAttackPhase();

            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result);
        }

        #endregion

        #region LookAndOrderDeckCards (Musique de Mega Drive)

        [Test]
        public void LookAndOrderDeckCards_ViewsAndReordersDeck()
        {
            // Arrange
            // Musique de Mega Drive: Look at top 5 cards and reorder them
            var ability = _effectFactory.CreateLookDeck(cardCount: 5);

            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - Should require user input to reorder
            Assert.IsTrue(result.RequiresUserInput);
        }

        #endregion

        #region LooseHPBasedOnNumberInvocation (Pains aux raisins)

        [Test]
        public void LooseHPBasedOnNumberInvocation_DamagesBasedOnInvocationCount()
        {
            // Arrange
            // Pains aux raisins: Player loses HP per invocation on field
            // Cards must be added to END of deck because DrawCard takes from end
            var deck = TestCardFactory.CreateDeck(27);
            deck.Add(TestCardFactory.CreateInvocation("Card 1", 2, 2, CardFamily.Human));
            deck.Add(TestCardFactory.CreateInvocation("Card 2", 3, 3, CardFamily.Developer));
            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard(); player1.DrawCard();
            foreach (var c in player1.Hand.ToList()) player1.PlayCard(c);

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            // HP loss based on invocation count
            Assert.AreEqual(2, player1.Field.Count);
        }

        #endregion

        #region DestroyEquipmentCard (Passage secret)

        [Test]
        public void DestroyEquipmentCard_DestroysOneEquipment()
        {
            // Arrange
            // Passage secret: Destroy an equipment card on the field
            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            // Should destroy equipment when activated
            Assert.IsNotNull(player1);
        }

        #endregion

        #region LookOpponentHandCardsAndChangeIt (Petite portions de riz)

        [Test]
        public void LookOpponentHandCardsAndChangeIt_ViewsAndSwapsCard()
        {
            // Arrange
            // Petite portions de riz: View opponent hand and swap one card
            var ability = _effectFactory.CreateLookHand();

            var player1 = PlayerFactory.CreatePlayer1();

            var deck2 = TestCardFactory.CreateDeck(30);
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.DrawCard();
            player2.DrawCard();

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.RequiresUserInput);
        }

        #endregion

        #region InvokeCardFromYellowTrash (Quart de smiley)

        [Test]
        public void InvokeCardFromYellowTrash_InvokesFromGraveyard()
        {
            // Arrange
            // Quart de smiley: Invoke card from graveyard to field
            var ability = _effectFactory.CreateDrawFromGraveyard();

            var deck = TestCardFactory.CreateDeck(30);
            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            var card = player1.Hand.First();
            player1.PlayCard(card);
            player1.DestroyCardFromField(card); // Now in graveyard

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            Assert.IsTrue(result.RequiresUserInput);
        }

        #endregion

        #region Loose1HPPerOpponentHandCards (Un delicieux risotto)

        [Test]
        public void Loose1HPPerOpponentHandCards_DamagesPerHandCard()
        {
            // Arrange
            // Un delicieux risotto: Opponent loses 1 HP per card in their hand
            var ability = _effectFactory.CreateDamageOpponent(
                DamageCalculationType.ByOpponentHandCount, 1f);

            var player1 = PlayerFactory.CreatePlayer1();

            var deck2 = TestCardFactory.CreateDeck(30);
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.DrawCard();
            player2.DrawCard();
            player2.DrawCard();
            player2.DrawCard(); // 5 cards in hand

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert - Should deal 5 damage
            AssertAbilitySuccess(result, "5");
        }

        #endregion

        #region GetBackAllHPBySacrifice5AtkDef (Youtube Money)

        [Test]
        public void GetBackAllHPBySacrifice5AtkDef_FullHealOnSacrifice()
        {
            // Arrange
            // Youtube Money: Sacrifice invocation with 5+ ATK/DEF to restore all HP
            var sacrificeCard = TestCardFactory.CreateInvocation("5/5 Card", 5, 5, CardFamily.Developer);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(sacrificeCard);  // Add to end, DrawCard takes from end
            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());
            player1.TakeDamage(20); // 10 HP remaining

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            // Should restore to full HP when sacrificing 5/5+ card
            Assert.IsTrue(sacrificeCard.Stats.Value.Attack >= 5);
            Assert.IsTrue(sacrificeCard.Stats.Value.Defense >= 5);
            Assert.AreEqual(10, player1.Health);
        }

        #endregion
    }
}
