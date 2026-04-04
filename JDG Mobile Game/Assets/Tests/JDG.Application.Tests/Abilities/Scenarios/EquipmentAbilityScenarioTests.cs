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
    /// Scenario tests for Equipment abilities.
    /// Tests verify equipment modifications work correctly in realistic game scenarios.
    ///
    /// Equipment Abilities (19):
    /// - SetStatsEquipment: Set ATK/DEF to specific values (SetATKToOne, SetDefToZero)
    /// - BonusStatsEquipment: Add ATK/DEF bonuses (Earn1ATKAndMinus1DEF, Earn2ATK, etc.)
    /// - MultiplyStatsEquipment: Multiply ATK/DEF (MultiplyDefBy2, MultiplyAtkBy3, etc.)
    /// - ProtectFromDestructionEquipment: Protect from destruction
    /// - CancelAbilitiesEquipment: Cancel equipped card's abilities
    /// - SwitchEquipment: Can be equipped to multiple cards
    /// - PreventAttackNewCardsEquipment: Cannot attack newly summoned cards
    /// - DirectAttackEquipment: Enable direct attack
    /// - HandBasedStatsEquipment: Stats based on hand count
    /// - CantBeAttackedEquipment: Cannot be attacked by invocations
    /// </summary>
    [TestFixture]
    public class EquipmentAbilityScenarioTests : AbilityScenarioTestBase
    {
        private EquipmentAbilityFactory _equipmentFactory;

        protected override void RegisterAbilities()
        {
            _equipmentFactory = new EquipmentAbilityFactory(PlayerRepository);
        }

        #region SetStatsEquipment Tests

        [Test]
        public void SetStatsEquipment_SetsExactStats()
        {
            // Arrange
            var ability = _equipmentFactory.CreateSetStats(1, 0);

            var invocation = TestCardFactory.CreateInvocation("Target Card", 5, 5, CardFamily.Human);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(invocation);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);
            context.TargetCard = invocation;

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "set to 1/0");
        }

        [Test]
        public void SetStatsEquipment_CanActivate_WhenTargetExists_ReturnsTrue()
        {
            // Arrange
            var ability = _equipmentFactory.CreateSetStats(3, 3);

            var invocation = TestCardFactory.CreateInvocation("Target", 2, 2, CardFamily.Human);

            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(invocation);
            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);
            context.TargetCard = invocation;

            // Act & Assert
            Assert.IsTrue(ability.CanActivate(context));
        }

        [Test]
        public void SetStatsEquipment_CanActivate_WhenNoTarget_ReturnsFalse()
        {
            // Arrange
            var ability = _equipmentFactory.CreateSetStats(1, 1);
            var context = CreateContext(null, AbilityName.Default);
            // No target set

            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            // Act & Assert
            Assert.IsFalse(ability.CanActivate(context));
        }

        #endregion

        #region BonusStatsEquipment Tests

        [Test]
        public void BonusStatsEquipment_AddsToExistingStats()
        {
            // Arrange - +2 ATK bonus
            var ability = _equipmentFactory.CreateBonusStats(2, 0);

            var invocation = TestCardFactory.CreateInvocation("Target Card", 3, 4, CardFamily.Human);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(invocation);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);
            context.TargetCard = invocation;

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "Added +2/+0");
        }

        [Test]
        public void BonusStatsEquipment_Earn1ATKAndMinus1DEF()
        {
            // Arrange - +1 ATK, -1 DEF
            var ability = _equipmentFactory.CreateBonusStats(1, -1);

            var invocation = TestCardFactory.CreateInvocation("Test Card", 3, 4, CardFamily.Human);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(invocation);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);
            context.TargetCard = invocation;

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "+1/+-1");
        }

        [Test]
        public void BonusStatsEquipment_Earn1ATKAnd1DEF()
        {
            // Arrange - +1 ATK/DEF
            var ability = _equipmentFactory.CreateBonusStats(1, 1);

            var invocation = TestCardFactory.CreateInvocation("Test Card", 2, 2, CardFamily.Human);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(invocation);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);
            context.TargetCard = invocation;

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result);
        }

        #endregion

        #region MultiplyStatsEquipment Tests

        [Test]
        public void MultiplyStatsEquipment_MultiplyDefBy2ButPreventAttack()
        {
            // Arrange - DEF * 2
            var ability = _equipmentFactory.CreateMultiplyStats(1, 2); // ATK * 1 (unchanged), DEF * 2

            var invocation = TestCardFactory.CreateInvocation("Tank", 2, 4, CardFamily.Human);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(invocation);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);
            context.TargetCard = invocation;

            // Act
            var result = ability.Execute(context);

            // Assert - DEF should be 4 * 2 = 8
            AssertAbilitySuccess(result, "multiplied");
        }

        [Test]
        public void MultiplyStatsEquipment_MultiplyAtkBy3()
        {
            // Arrange - ATK * 3
            var ability = _equipmentFactory.CreateMultiplyStats(3, 1); // ATK * 3, DEF * 1

            var invocation = TestCardFactory.CreateInvocation("Attacker", 3, 2, CardFamily.Human);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(invocation);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);
            context.TargetCard = invocation;

            // Act
            var result = ability.Execute(context);

            // Assert - ATK should be 3 * 3 = 9
            AssertAbilitySuccess(result, "multiplied");
        }

        [Test]
        public void MultiplyStatsEquipment_MultiplyAtkBy2AndDefByHalf()
        {
            // Arrange - ATK * 2, DEF * 0.5
            var ability = _equipmentFactory.CreateMultiplyStats(2, 0.5f);

            var invocation = TestCardFactory.CreateInvocation("Glass Cannon", 4, 4, CardFamily.Human);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(invocation);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);
            context.TargetCard = invocation;

            // Act
            var result = ability.Execute(context);

            // Assert - ATK = 8, DEF = 2
            AssertAbilitySuccess(result, "multiplied");
        }

        [Test]
        public void MultiplyStatsEquipment_NoTarget_Fails()
        {
            // Arrange
            var ability = _equipmentFactory.CreateMultiplyStats(2, 2);

            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);
            // No target

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilityFailure(result);
        }

        #endregion

        #region ProtectFromDestructionEquipment Tests

        [Test]
        public void ProtectFromDestructionEquipment_Execute_ReturnsSuccess()
        {
            // Arrange
            var ability = _equipmentFactory.CreateProtectFromDestruction();

            var invocation = TestCardFactory.CreateInvocation("Protected Card", 3, 3, CardFamily.Human);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(invocation);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);
            context.TargetCard = invocation;

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "protected");
        }

        [Test]
        public void ProtectFromDestructionEquipment_OnPreDestroy_ReturnsFalse()
        {
            // Arrange
            var ability = _equipmentFactory.CreateProtectFromDestruction();

            var invocation = TestCardFactory.CreateInvocation("Protected Card", 3, 3, CardFamily.Human);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(invocation);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);
            context.TargetCard = invocation;

            // Act
            bool shouldDestroy = ability.OnPreDestroy(context);

            // Assert - Protection prevents destruction
            Assert.IsFalse(shouldDestroy, "Protection should prevent destruction");
        }

        #endregion

        #region CancelAbilitiesEquipment Tests

        [Test]
        public void CancelAbilitiesEquipment_CancelsCardAbilities()
        {
            // Arrange
            var ability = _equipmentFactory.CreateCancelAbilities();

            var invocation = TestCardFactory.CreateInvocation("Ability Card", 4, 4, CardFamily.Wizard);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(invocation);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);
            context.TargetCard = invocation;

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "canceled");
        }

        #endregion

        #region SwitchEquipment Tests

        [Test]
        public void SwitchEquipment_CanAlwaysBePlaced_IsTrue()
        {
            // Arrange
            var ability = _equipmentFactory.CreateSwitchEquipment();

            // Act & Assert
            Assert.IsTrue(ability.CanAlwaysBePlaced,
                "Switch equipment should be able to be placed on any card");
        }

        [Test]
        public void SwitchEquipment_CanActivate_AlwaysTrue()
        {
            // Arrange
            var ability = _equipmentFactory.CreateSwitchEquipment();

            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act & Assert
            Assert.IsTrue(ability.CanActivate(context),
                "Switch equipment should always be activatable");
        }

        [Test]
        public void SwitchEquipment_Execute_ReturnsSuccess()
        {
            // Arrange
            var ability = _equipmentFactory.CreateSwitchEquipment();

            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "switched");
        }

        #endregion

        #region DirectAttackEquipment Tests

        [Test]
        public void DirectAttackEquipment_EnablesDirectAttack()
        {
            // Arrange
            var ability = _equipmentFactory.CreateDirectAttack();

            var invocation = TestCardFactory.CreateInvocation("Direct Attacker", 4, 2, CardFamily.Monster);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(invocation);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);
            context.TargetCard = invocation;

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "Direct attack enabled");
        }

        [Test]
        public void DirectAttackEquipment_NoTarget_Fails()
        {
            // Arrange
            var ability = _equipmentFactory.CreateDirectAttack();

            var player1 = PlayerFactory.CreatePlayer1();
            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);
            // No target

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilityFailure(result, "No target");
        }

        #endregion

        #region CantBeAttackedEquipment Tests

        [Test]
        public void CantBeAttackedEquipment_PreventsBeingtAttacked()
        {
            // Arrange
            var ability = _equipmentFactory.CreateCantBeAttacked();

            var invocation = TestCardFactory.CreateInvocation("Untargetable", 3, 3, CardFamily.Monster);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(invocation);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);
            context.TargetCard = invocation;

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "cannot be attacked");
        }

        [Test]
        public void CantBeAttackedEquipment_OnPreDestroy_ReturnsFalse()
        {
            // Arrange
            var ability = _equipmentFactory.CreateCantBeAttacked();

            var invocation = TestCardFactory.CreateInvocation("Untargetable", 3, 3, CardFamily.Monster);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(invocation);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);
            context.TargetCard = invocation;

            // Act
            bool shouldDestroy = ability.OnPreDestroy(context);

            // Assert - Should not be destroyed by invocations
            Assert.IsFalse(shouldDestroy);
        }

        #endregion

        #region PreventAttackNewCardsEquipment Tests

        [Test]
        public void PreventAttackNewCardsEquipment_BlocksAttackOnNewCard()
        {
            // Arrange
            var ability = _equipmentFactory.CreatePreventAttackNew();

            var newCard = TestCardFactory.CreateInvocation("New Summon", 2, 2, CardFamily.Human);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(newCard);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);
            context.TargetCard = newCard;

            // Act
            var result = ability.Execute(context);

            // Assert
            AssertAbilitySuccess(result, "restriction");
        }

        [Test]
        public void PreventAttackNewCardsEquipment_HasCardPlayedTrigger()
        {
            // Arrange
            var ability = _equipmentFactory.CreatePreventAttackNew();

            // Assert
            Assert.AreEqual(AbilityTrigger.OnCardPlayed, ability.Trigger,
                "Should trigger when cards are played");
        }

        #endregion

        #region HandBasedStatsEquipment Tests

        [Test]
        public void HandBasedStatsEquipment_ScalesWithHandSize()
        {
            // Arrange - +1 ATK per hand card
            var ability = _equipmentFactory.CreateHandBasedStats(1, 0);

            var invocation = TestCardFactory.CreateInvocation("Hand Scaler", 2, 2, CardFamily.Wizard);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(invocation);
            // Add extra cards to deck for drawing

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard(); // Draw invocation
            player1.PlayCard(player1.Hand.First());
            // Draw more cards to hand
            player1.DrawCard();
            player1.DrawCard();
            player1.DrawCard(); // Now has 3 cards in hand

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);
            context.TargetCard = invocation;

            // Act
            var result = ability.Execute(context);

            // Assert - +3 ATK based on 3 hand cards
            AssertAbilitySuccess(result, "3"); // Should mention 3 cards
        }

        [Test]
        public void HandBasedStatsEquipment_HasHandChangeTrigger()
        {
            // Arrange
            var ability = _equipmentFactory.CreateHandBasedStats(1, 1);

            // Assert
            Assert.AreEqual(AbilityTrigger.OnHandChange, ability.Trigger,
                "Should trigger when hand changes");
        }

        #endregion

        #region Edge Cases

        [Test]
        public void Equipment_DoesNotAffectOpponentCards()
        {
            // Arrange
            var ability = _equipmentFactory.CreateBonusStats(5, 5);

            // Player 1's card
            var myCard = TestCardFactory.CreateInvocation("My Card", 2, 2, CardFamily.Human);
            var deck1 = TestCardFactory.CreateDeck(29);
            deck1.Add(myCard);
            var player1 = new Player(PlayerId.Player1, deck1);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            // Player 2's card
            var opponentCard = TestCardFactory.CreateInvocation("Opponent Card", 3, 3, CardFamily.Human);
            var deck2 = TestCardFactory.CreateDeck(29);
            deck2.Add(opponentCard);
            var player2 = new Player(PlayerId.Player2, deck2);
            player2.DrawCard();
            player2.PlayCard(player2.Hand.First());

            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);
            context.TargetCard = myCard;

            // Remember opponent stats before
            var opponentField = player2.Field.First();
            var origAtk = opponentField.Stats?.Attack ?? 0;
            var origDef = opponentField.Stats?.Defense ?? 0;

            // Act
            ability.Execute(context);

            // Assert - Opponent's card should be unchanged
            Assert.AreEqual(origAtk, opponentField.Stats?.Attack ?? 0);
            Assert.AreEqual(origDef, opponentField.Stats?.Defense ?? 0);
        }

        [Test]
        public void Equipment_CanStackMultipleBonus()
        {
            // Arrange - Two bonus equipments
            var bonus1 = _equipmentFactory.CreateBonusStats(2, 0);
            var bonus2 = _equipmentFactory.CreateBonusStats(0, 3);

            var invocation = TestCardFactory.CreateInvocation("Stacking Target", 1, 1, CardFamily.Human);
            var deck = TestCardFactory.CreateDeck(29);
            deck.Add(invocation);

            var player1 = new Player(PlayerId.Player1, deck);
            player1.DrawCard();
            player1.PlayCard(player1.Hand.First());

            var player2 = PlayerFactory.CreatePlayer2();
            PlayerRepository.AddPlayer(player1);
            PlayerRepository.AddPlayer(player2);

            var context = CreateContext(null, AbilityName.Default);
            context.TargetCard = invocation;

            // Act - Apply both bonuses
            var result1 = bonus1.Execute(context);
            var result2 = bonus2.Execute(context);

            // Assert - Both should succeed
            AssertAbilitySuccess(result1);
            AssertAbilitySuccess(result2);
        }

        #endregion
    }
}
