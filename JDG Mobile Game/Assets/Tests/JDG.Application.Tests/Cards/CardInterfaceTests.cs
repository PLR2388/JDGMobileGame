using System.Collections.Generic;
using JDG.Application.Cards;
using JDG.Domain;
using JDG.Domain.Enums;
using NUnit.Framework;

namespace JDG.Application.Tests.Cards
{
    /// <summary>
    /// Unit tests for card interfaces created in Phase 41.
    /// Tests verify interface contracts and test double implementations.
    /// </summary>
    [TestFixture]
    public class CardInterfaceTests
    {
        #region IInGameCard Tests

        [Test]
        public void TestInGameCard_ImplementsIInGameCard()
        {
            // Arrange & Act
            IInGameCard card = new TestInGameCard("Test Card", CardOwner.Player1);

            // Assert
            Assert.IsNotNull(card);
            Assert.AreEqual("Test Card", card.Title);
            Assert.AreEqual(CardOwner.Player1, card.CardOwner);
        }

        [Test]
        public void TestInGameCard_SupportsPlayer2()
        {
            // Arrange & Act
            IInGameCard card = new TestInGameCard("Player2 Card", CardOwner.Player2);

            // Assert
            Assert.AreEqual(CardOwner.Player2, card.CardOwner);
        }

        [Test]
        public void TestInGameCard_SupportsNotDefined()
        {
            // Arrange & Act
            IInGameCard card = new TestInGameCard("Undefined Card", CardOwner.NotDefined);

            // Assert
            Assert.AreEqual(CardOwner.NotDefined, card.CardOwner);
        }

        #endregion

        #region IInGameInvocationCard Tests

        [Test]
        public void TestInvocationCard_ImplementsIInGameInvocationCard()
        {
            // Arrange & Act
            IInGameInvocationCard card = new TestInGameInvocationCard(
                "Invocation Card",
                CardOwner.Player1,
                attack: 100,
                defense: 50
            );

            // Assert
            Assert.IsNotNull(card);
            Assert.AreEqual("Invocation Card", card.Title);
            Assert.AreEqual(100, card.Attack);
            Assert.AreEqual(50, card.Defense);
        }

        [Test]
        public void TestInvocationCard_AttackAndDefenseAreSettable()
        {
            // Arrange
            var card = new TestInGameInvocationCard("Test", CardOwner.Player1, 100, 50);

            // Act
            card.Attack = 150;
            card.Defense = 75;

            // Assert
            Assert.AreEqual(150, card.Attack);
            Assert.AreEqual(75, card.Defense);
        }

        [Test]
        public void TestInvocationCard_ResetNewTurn_TracksCall()
        {
            // Arrange
            var card = new TestInGameInvocationCard("Test", CardOwner.Player1, 100, 50);

            // Act
            card.ResetNewTurn();

            // Assert
            Assert.IsTrue(card.ResetNewTurnCalled);
        }

        [Test]
        public void TestInvocationCard_UnblockAttack_TracksCall()
        {
            // Arrange
            var card = new TestInGameInvocationCard("Test", CardOwner.Player1, 100, 50);

            // Act
            card.UnblockAttack();

            // Assert
            Assert.IsTrue(card.UnblockAttackCalled);
        }

        [Test]
        public void TestInvocationCard_FreeCard_TracksCall()
        {
            // Arrange
            var card = new TestInGameInvocationCard("Test", CardOwner.Player1, 100, 50);

            // Act
            card.FreeCard();

            // Assert
            Assert.IsTrue(card.FreeCardCalled);
        }

        [Test]
        public void TestInvocationCard_BaseStats_AreImmutable()
        {
            // Arrange
            var card = new TestInGameInvocationCard("Test", CardOwner.Player1, 100, 50);

            // Act - Modify current stats
            card.Attack = 200;
            card.Defense = 100;

            // Assert - Base stats unchanged
            Assert.AreEqual(100, card.BaseAttack);
            Assert.AreEqual(50, card.BaseDefense);
        }

        #endregion

        #region IPlayerCardCollection Tests

        [Test]
        public void TestPlayerCardCollection_ImplementsInterface()
        {
            // Arrange & Act
            IPlayerCardCollection collection = new TestPlayerCardCollection(isPlayerOne: true);

            // Assert
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.IsPlayerOne);
            Assert.AreEqual(CardOwner.Player1, collection.Owner);
        }

        [Test]
        public void TestPlayerCardCollection_Player2_HasCorrectOwner()
        {
            // Arrange & Act
            IPlayerCardCollection collection = new TestPlayerCardCollection(isPlayerOne: false);

            // Assert
            Assert.IsFalse(collection.IsPlayerOne);
            Assert.AreEqual(CardOwner.Player2, collection.Owner);
        }

        [Test]
        public void TestPlayerCardCollection_HasEmptyCollections()
        {
            // Arrange & Act
            IPlayerCardCollection collection = new TestPlayerCardCollection(isPlayerOne: true);

            // Assert
            Assert.IsNotNull(collection.InvocationCards);
            Assert.IsNotNull(collection.EffectCards);
            Assert.IsNotNull(collection.HandCards);
            Assert.AreEqual(0, collection.HandCardCount);
        }

        #endregion
    }

    #region Test Implementations

    /// <summary>
    /// Test implementation of IInGameCard.
    /// Phase 39: Updated with new interface properties.
    /// </summary>
    public class TestInGameCard : IInGameCard
    {
        public string Title { get; }
        public CardOwner CardOwner { get; }
        public CardType Type { get; }
        public bool Collector { get; }
        public string Description { get; }
        public string DetailedDescription { get; }
        public string VisualId => Title;

        public TestInGameCard(string title, CardOwner owner, CardType type = CardType.Invocation)
        {
            Title = title;
            CardOwner = owner;
            Type = type;
            Collector = false;
            Description = "";
            DetailedDescription = "";
        }
    }

    /// <summary>
    /// Test implementation of IInGameInvocationCard.
    /// Tracks method calls for test verification.
    /// Phase 39: Updated with new IInGameCard interface properties.
    /// Phase 68: Updated with all Phase 72 interface members.
    /// </summary>
    public class TestInGameInvocationCard : IInGameInvocationCard
    {
        // IInGameCard members
        public string Title { get; }
        public CardOwner CardOwner { get; }
        public CardType Type => CardType.Invocation;
        public bool Collector { get; }
        public string Description { get; }
        public string DetailedDescription { get; }
        public string VisualId => Title;

        // Stats
        public float Attack { get; set; }
        public float Defense { get; set; }
        public float BaseAttack { get; }
        public float BaseDefense { get; }
        public CardFamily[] Families { get; set; } = System.Array.Empty<CardFamily>();

        // Combat State
        public bool CanDirectAttack { get; set; }
        public bool CantBeAttack { get; set; }
        public bool Aggro { get; set; }

        // Ability State
        public IReadOnlyList<object> Abilities => new List<object>();
        public bool CancelEffect { get; set; }
        public bool IsAffectedByEffectCard { get; set; } = true;

        // Equipment
        public IInGameEquipmentCard EquipmentCard { get; private set; }

        // Control
        public bool IsControlled { get; private set; }

        // Turn/Field Tracking
        public int NumberOfTurnOnField { get; private set; }
        public int NumberOfDeaths { get; private set; }

        // Tracking properties for tests
        public bool ResetNewTurnCalled { get; private set; }
        public bool UnblockAttackCalled { get; private set; }
        public bool FreeCardCalled { get; private set; }
        public bool BlockAttackCalled { get; private set; }
        public bool AttackTurnDoneCalled { get; private set; }
        public bool ControlCardCalled { get; private set; }
        public int LastSetRemainedAttack { get; private set; }

        public TestInGameInvocationCard(string title, CardOwner owner, float attack, float defense)
        {
            Title = title;
            CardOwner = owner;
            Attack = attack;
            Defense = defense;
            BaseAttack = attack;
            BaseDefense = defense;
            Collector = false;
            Description = "";
            DetailedDescription = "";
        }

        // Combat methods
        public bool CanAttack() => true;

        public void BlockAttack()
        {
            BlockAttackCalled = true;
        }

        public void AttackTurnDone()
        {
            AttackTurnDoneCalled = true;
        }

        public void SetRemainedAttackThisTurn(int number)
        {
            LastSetRemainedAttack = number;
        }

        // Ability methods
        public bool HasAction() => false;

        // Equipment methods
        public void SetEquipmentCard(IInGameEquipmentCard card)
        {
            EquipmentCard = card;
        }

        // Control methods
        public void ControlCard()
        {
            IsControlled = true;
            ControlCardCalled = true;
        }

        public void FreeCard()
        {
            IsControlled = false;
            FreeCardCalled = true;
        }

        // Turn/Field methods
        public void IncrementNumberTurnOnField()
        {
            NumberOfTurnOnField++;
        }

        public void IncrementNumberDeaths()
        {
            NumberOfDeaths++;
        }

        public void ResetNewTurn()
        {
            ResetNewTurnCalled = true;
        }

        public void UnblockAttack()
        {
            UnblockAttackCalled = true;
        }
    }

    /// <summary>
    /// Test implementation of IPlayerCardCollection.
    /// </summary>
    public class TestPlayerCardCollection : IPlayerCardCollection
    {
        public bool IsPlayerOne { get; }
        public CardOwner Owner => IsPlayerOne ? CardOwner.Player1 : CardOwner.Player2;
        public IReadOnlyList<IInGameInvocationCard> InvocationCards { get; } = new List<IInGameInvocationCard>();
        public IReadOnlyList<IInGameEffectCard> EffectCards { get; } = new List<IInGameEffectCard>();
        public IInGameFieldCard FieldCard => null;
        public IReadOnlyList<IInGameCard> HandCards { get; } = new List<IInGameCard>();
        public int HandCardCount => HandCards.Count;

        public TestPlayerCardCollection(bool isPlayerOne)
        {
            IsPlayerOne = isPlayerOne;
        }
    }

    #endregion
}
