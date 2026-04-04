using System.Collections.Generic;
using System.Linq;
using JDG.Application.Abilities;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;
using NUnit.Framework;

namespace JDG.TestUtilities
{
    /// <summary>
    /// Custom assertions for ability scenario tests.
    /// Provides readable, domain-specific assertions.
    /// </summary>
    public static class AbilityTestAssertions
    {
        #region Card Location Assertions

        /// <summary>
        /// Asserts that a card with the given name is on the player's field.
        /// </summary>
        public static void AssertCardOnField(Player player, string cardName)
        {
            var cardOnField = player.Field.Any(c => c.Title == cardName);
            Assert.IsTrue(cardOnField,
                $"Expected card '{cardName}' to be on field for {player.Id}, but it was not found. " +
                $"Cards on field: [{string.Join(", ", player.Field.Select(c => c.Title))}]");
        }

        /// <summary>
        /// Asserts that a card with the given name is NOT on the player's field.
        /// </summary>
        public static void AssertCardNotOnField(Player player, string cardName)
        {
            var cardOnField = player.Field.Any(c => c.Title == cardName);
            Assert.IsFalse(cardOnField,
                $"Expected card '{cardName}' to NOT be on field for {player.Id}, but it was found.");
        }

        /// <summary>
        /// Asserts that a card with the given name is in the player's hand.
        /// </summary>
        public static void AssertCardInHand(Player player, string cardName)
        {
            var cardInHand = player.Hand.Any(c => c.Title == cardName);
            Assert.IsTrue(cardInHand,
                $"Expected card '{cardName}' to be in hand for {player.Id}, but it was not found. " +
                $"Cards in hand: [{string.Join(", ", player.Hand.Select(c => c.Title))}]");
        }

        /// <summary>
        /// Asserts that a card with the given name is NOT in the player's hand.
        /// </summary>
        public static void AssertCardNotInHand(Player player, string cardName)
        {
            var cardInHand = player.Hand.Any(c => c.Title == cardName);
            Assert.IsFalse(cardInHand,
                $"Expected card '{cardName}' to NOT be in hand for {player.Id}, but it was found.");
        }

        /// <summary>
        /// Asserts that a card with the given name is in the player's graveyard.
        /// </summary>
        public static void AssertCardInGraveyard(Player player, string cardName)
        {
            var cardInGraveyard = player.Graveyard.Any(c => c.Title == cardName);
            Assert.IsTrue(cardInGraveyard,
                $"Expected card '{cardName}' to be in graveyard for {player.Id}, but it was not found. " +
                $"Cards in graveyard: [{string.Join(", ", player.Graveyard.Select(c => c.Title))}]");
        }

        /// <summary>
        /// Asserts that a card with the given name is NOT in the player's graveyard.
        /// </summary>
        public static void AssertCardNotInGraveyard(Player player, string cardName)
        {
            var cardInGraveyard = player.Graveyard.Any(c => c.Title == cardName);
            Assert.IsFalse(cardInGraveyard,
                $"Expected card '{cardName}' to NOT be in graveyard for {player.Id}, but it was found.");
        }

        /// <summary>
        /// Asserts that a card with the given name is in the player's deck.
        /// </summary>
        public static void AssertCardInDeck(Player player, string cardName)
        {
            var cardInDeck = player.Deck.Any(c => c.Title == cardName);
            Assert.IsTrue(cardInDeck,
                $"Expected card '{cardName}' to be in deck for {player.Id}, but it was not found.");
        }

        /// <summary>
        /// Asserts that a card with the given name is NOT in the player's deck.
        /// </summary>
        public static void AssertCardNotInDeck(Player player, string cardName)
        {
            var cardInDeck = player.Deck.Any(c => c.Title == cardName);
            Assert.IsFalse(cardInDeck,
                $"Expected card '{cardName}' to NOT be in deck for {player.Id}, but it was found.");
        }

        #endregion

        #region Player State Assertions

        /// <summary>
        /// Asserts the player has a specific amount of health.
        /// </summary>
        public static void AssertPlayerHealth(Player player, float expectedHealth)
        {
            Assert.AreEqual(expectedHealth, player.Health,
                $"Expected {player.Id} to have {expectedHealth} HP, but had {player.Health} HP");
        }

        /// <summary>
        /// Asserts the player has health within a range (for floating point comparisons).
        /// </summary>
        public static void AssertPlayerHealthApproximate(Player player, float expectedHealth, float tolerance = 0.1f)
        {
            Assert.That(player.Health, Is.EqualTo(expectedHealth).Within(tolerance),
                $"Expected {player.Id} to have approximately {expectedHealth} HP, but had {player.Health} HP");
        }

        /// <summary>
        /// Asserts the player's deck has a specific count.
        /// </summary>
        public static void AssertDeckCount(Player player, int expectedCount)
        {
            Assert.AreEqual(expectedCount, player.DeckCount,
                $"Expected {player.Id} to have {expectedCount} cards in deck, but had {player.DeckCount}");
        }

        /// <summary>
        /// Asserts the player's hand has a specific count.
        /// </summary>
        public static void AssertHandCount(Player player, int expectedCount)
        {
            Assert.AreEqual(expectedCount, player.HandCount,
                $"Expected {player.Id} to have {expectedCount} cards in hand, but had {player.HandCount}");
        }

        /// <summary>
        /// Asserts the player's field has a specific count.
        /// </summary>
        public static void AssertFieldCount(Player player, int expectedCount)
        {
            Assert.AreEqual(expectedCount, player.Field.Count,
                $"Expected {player.Id} to have {expectedCount} cards on field, but had {player.Field.Count}");
        }

        /// <summary>
        /// Asserts the player's graveyard has a specific count.
        /// </summary>
        public static void AssertGraveyardCount(Player player, int expectedCount)
        {
            Assert.AreEqual(expectedCount, player.Graveyard.Count,
                $"Expected {player.Id} to have {expectedCount} cards in graveyard, but had {player.Graveyard.Count}");
        }

        #endregion

        #region Card Stats Assertions

        /// <summary>
        /// Asserts a card has specific ATK and DEF values.
        /// </summary>
        public static void AssertCardStats(Card card, float expectedAtk, float expectedDef)
        {
            Assert.AreEqual(expectedAtk, card.Stats?.Attack ?? 0,
                $"Expected card '{card.Title}' to have {expectedAtk} ATK, but had {card.Stats?.Attack ?? 0}");
            Assert.AreEqual(expectedDef, card.Stats?.Defense ?? 0,
                $"Expected card '{card.Title}' to have {expectedDef} DEF, but had {card.Stats?.Defense ?? 0}");
        }

        /// <summary>
        /// Asserts a card's ATK value.
        /// </summary>
        public static void AssertCardAttack(Card card, float expectedAtk)
        {
            Assert.AreEqual(expectedAtk, card.Stats?.Attack ?? 0,
                $"Expected card '{card.Title}' to have {expectedAtk} ATK, but had {card.Stats?.Attack ?? 0}");
        }

        /// <summary>
        /// Asserts a card's DEF value.
        /// </summary>
        public static void AssertCardDefense(Card card, float expectedDef)
        {
            Assert.AreEqual(expectedDef, card.Stats?.Defense ?? 0,
                $"Expected card '{card.Title}' to have {expectedDef} DEF, but had {card.Stats?.Defense ?? 0}");
        }

        /// <summary>
        /// Asserts card stats with floating point tolerance.
        /// </summary>
        public static void AssertCardStatsApproximate(Card card, float expectedAtk, float expectedDef, float tolerance = 0.1f)
        {
            Assert.That(card.Stats?.Attack ?? 0, Is.EqualTo(expectedAtk).Within(tolerance),
                $"Expected card '{card.Title}' to have approximately {expectedAtk} ATK");
            Assert.That(card.Stats?.Defense ?? 0, Is.EqualTo(expectedDef).Within(tolerance),
                $"Expected card '{card.Title}' to have approximately {expectedDef} DEF");
        }

        #endregion

        #region Ability Result Assertions

        /// <summary>
        /// Asserts that an ability result indicates success.
        /// </summary>
        public static void AssertAbilitySuccess(AbilityResult result, string expectedMessageContains = null)
        {
            Assert.IsTrue(result.IsSuccess,
                $"Expected ability to succeed, but it failed with message: {result.Message}");

            if (expectedMessageContains != null)
            {
                Assert.IsTrue(result.Message.Contains(expectedMessageContains),
                    $"Expected success message to contain '{expectedMessageContains}', but was: {result.Message}");
            }
        }

        /// <summary>
        /// Asserts that an ability result indicates failure.
        /// </summary>
        public static void AssertAbilityFailure(AbilityResult result, string expectedMessageContains = null)
        {
            Assert.IsFalse(result.IsSuccess,
                $"Expected ability to fail, but it succeeded with message: {result.Message}");

            if (expectedMessageContains != null)
            {
                Assert.IsTrue(result.Message.Contains(expectedMessageContains),
                    $"Expected failure message to contain '{expectedMessageContains}', but was: {result.Message}");
            }
        }

        /// <summary>
        /// Asserts that an ability result requires user input.
        /// </summary>
        public static void AssertAbilityNeedsUserInput(AbilityResult result)
        {
            Assert.IsTrue(result.RequiresUserInput,
                $"Expected ability to require user input, but RequiresUserInput was false");
        }

        #endregion

        #region Event Assertions

        /// <summary>
        /// Asserts that a specific number of events were published.
        /// </summary>
        public static void AssertEventCount(List<object> publishedEvents, int expectedCount)
        {
            Assert.AreEqual(expectedCount, publishedEvents.Count,
                $"Expected {expectedCount} events to be published, but {publishedEvents.Count} were published");
        }

        /// <summary>
        /// Asserts that an event of a specific type was published.
        /// </summary>
        public static void AssertEventPublished<TEvent>(List<object> publishedEvents) where TEvent : struct
        {
            var hasEvent = publishedEvents.Any(e => e is TEvent);
            Assert.IsTrue(hasEvent,
                $"Expected event of type {typeof(TEvent).Name} to be published, but it was not found. " +
                $"Published events: [{string.Join(", ", publishedEvents.Select(e => e.GetType().Name))}]");
        }

        /// <summary>
        /// Asserts that an event of a specific type was NOT published.
        /// </summary>
        public static void AssertEventNotPublished<TEvent>(List<object> publishedEvents) where TEvent : struct
        {
            var hasEvent = publishedEvents.Any(e => e is TEvent);
            Assert.IsFalse(hasEvent,
                $"Expected event of type {typeof(TEvent).Name} to NOT be published, but it was found.");
        }

        /// <summary>
        /// Gets the count of events of a specific type.
        /// </summary>
        public static int GetEventCount<TEvent>(List<object> publishedEvents) where TEvent : struct
        {
            return publishedEvents.Count(e => e is TEvent);
        }

        #endregion

        #region Card Family Assertions

        /// <summary>
        /// Asserts that a card belongs to a specific family.
        /// </summary>
        public static void AssertCardHasFamily(Card card, CardFamily expectedFamily)
        {
            Assert.IsTrue(card.Families.Contains(expectedFamily),
                $"Expected card '{card.Title}' to have family {expectedFamily}, " +
                $"but it has: [{string.Join(", ", card.Families)}]");
        }

        /// <summary>
        /// Asserts that a card does NOT belong to a specific family.
        /// </summary>
        public static void AssertCardDoesNotHaveFamily(Card card, CardFamily unexpectedFamily)
        {
            Assert.IsFalse(card.Families.Contains(unexpectedFamily),
                $"Expected card '{card.Title}' to NOT have family {unexpectedFamily}, but it does.");
        }

        #endregion

        #region Field Card Assertions

        /// <summary>
        /// Asserts that a player has a field card (CardType.Field) on their field.
        /// </summary>
        public static void AssertHasFieldCard(Player player, string fieldCardName = null)
        {
            var fieldCard = player.Field.FirstOrDefault(c => c.Type == CardType.Field);
            Assert.IsNotNull(fieldCard,
                $"Expected {player.Id} to have a field card, but no Field type card found on field");

            if (fieldCardName != null)
            {
                Assert.AreEqual(fieldCardName, fieldCard.Title,
                    $"Expected field card to be '{fieldCardName}', but was '{fieldCard.Title}'");
            }
        }

        /// <summary>
        /// Asserts that a player does NOT have a field card (CardType.Field) active.
        /// </summary>
        public static void AssertNoFieldCard(Player player)
        {
            var fieldCard = player.Field.FirstOrDefault(c => c.Type == CardType.Field);
            Assert.IsNull(fieldCard,
                $"Expected {player.Id} to have no field card, but had '{fieldCard?.Title}'");
        }

        #endregion
    }

}
