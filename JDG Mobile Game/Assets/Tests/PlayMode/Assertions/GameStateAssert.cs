using System;
using System.Collections.Generic;
using System.Text;
using JDG.Domain;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Services;
using JDG.PlayMode.Tests.TestHelpers;
using NUnit.Framework;

namespace JDG.PlayMode.Tests.Assertions
{
    #region Game State Snapshots

    /// <summary>
    /// Represents a snapshot of player state for debugging.
    /// </summary>
    public class PlayerSnapshot
    {
        public float Health { get; set; }
        public int DeckCount { get; set; }
        public int HandCount { get; set; }
        public List<string> FieldCards { get; set; } = new List<string>();
        public List<string> GraveyardCards { get; set; } = new List<string>();
        public int ShieldCount { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"HP={Health}, Deck={DeckCount}, Hand={HandCount}");
            if (FieldCards.Count > 0)
            {
                sb.Append($", Field=[{string.Join(", ", FieldCards)}]");
            }
            if (ShieldCount > 0)
            {
                sb.Append($", Shields={ShieldCount}");
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Captures complete game state for failure debugging.
    /// Provides detailed output when tests fail.
    /// </summary>
    public class GameStateSnapshot
    {
        public Phase CurrentPhase { get; set; }
        public int TurnNumber { get; set; }
        public PlayerId CurrentPlayer { get; set; }
        public PlayerSnapshot Player1 { get; set; } = new PlayerSnapshot();
        public PlayerSnapshot Player2 { get; set; } = new PlayerSnapshot();
        public List<string> RecentEvents { get; set; } = new List<string>();
        public DateTime CapturedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Creates a snapshot from the current game state.
        /// </summary>
        public static GameStateSnapshot Capture(
            GameStateService gameStateService,
            TestPlayerStatusProvider playerStatusProvider = null,
            TestCardCollectionService cardCollectionService = null,
            TestEventBus eventBus = null)
        {
            var snapshot = new GameStateSnapshot
            {
                CurrentPhase = gameStateService.CurrentPhase,
                TurnNumber = gameStateService.TurnNumber,
                CurrentPlayer = gameStateService.CurrentPlayer,
                CapturedAt = DateTime.Now
            };

            // Capture player status if available
            if (playerStatusProvider != null)
            {
                snapshot.Player1.Health = playerStatusProvider.CurrentPlayerStatus?.CurrentHealth ?? 0;
                snapshot.Player1.ShieldCount = playerStatusProvider.CurrentPlayerStatus?.NumberShield ?? 0;
                snapshot.Player2.Health = playerStatusProvider.OpponentPlayerStatus?.CurrentHealth ?? 0;
                snapshot.Player2.ShieldCount = playerStatusProvider.OpponentPlayerStatus?.NumberShield ?? 0;
            }

            // Capture card state if available
            if (cardCollectionService != null)
            {
                var p1Cards = cardCollectionService.CurrentPlayerCards;
                var p2Cards = cardCollectionService.OpponentPlayerCards;

                if (p1Cards != null)
                {
                    snapshot.Player1.DeckCount = p1Cards.Deck?.Count ?? 0;
                    snapshot.Player1.HandCount = p1Cards.HandCards?.Count ?? 0;
                    foreach (var card in p1Cards.InvocationCards)
                    {
                        if (card != null)
                            snapshot.Player1.FieldCards.Add(card.Title);
                    }
                }

                if (p2Cards != null)
                {
                    snapshot.Player2.DeckCount = p2Cards.Deck?.Count ?? 0;
                    snapshot.Player2.HandCount = p2Cards.HandCards?.Count ?? 0;
                    foreach (var card in p2Cards.InvocationCards)
                    {
                        if (card != null)
                            snapshot.Player2.FieldCards.Add(card.Title);
                    }
                }
            }

            // Capture recent events
            if (eventBus != null)
            {
                int eventCount = Math.Min(10, eventBus.EventHistory.Count);
                for (int i = eventBus.EventHistory.Count - eventCount; i < eventBus.EventHistory.Count; i++)
                {
                    snapshot.RecentEvents.Add(eventBus.EventHistory[i].ToString());
                }
            }

            return snapshot;
        }

        /// <summary>
        /// Returns a detailed string representation for debugging.
        /// </summary>
        public string ToDetailedString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== Game State Snapshot ===");
            sb.AppendLine($"Captured At: {CapturedAt:HH:mm:ss.fff}");
            sb.AppendLine($"Phase: {CurrentPhase}");
            sb.AppendLine($"Turn: {TurnNumber}");
            sb.AppendLine($"Current Player: {CurrentPlayer}");
            sb.AppendLine();
            sb.AppendLine($"Player 1: {Player1}");
            sb.AppendLine($"Player 2: {Player2}");

            if (RecentEvents.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Recent Events:");
                foreach (var evt in RecentEvents)
                {
                    sb.AppendLine($"  {evt}");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns a JSON-like representation for logging.
        /// </summary>
        public string ToJson()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine($"  \"capturedAt\": \"{CapturedAt:O}\",");
            sb.AppendLine($"  \"phase\": \"{CurrentPhase}\",");
            sb.AppendLine($"  \"turn\": {TurnNumber},");
            sb.AppendLine($"  \"currentPlayer\": \"{CurrentPlayer}\",");
            sb.AppendLine($"  \"player1\": {{ \"health\": {Player1.Health}, \"deck\": {Player1.DeckCount}, \"hand\": {Player1.HandCount} }},");
            sb.AppendLine($"  \"player2\": {{ \"health\": {Player2.Health}, \"deck\": {Player2.DeckCount}, \"hand\": {Player2.HandCount} }},");
            sb.AppendLine($"  \"recentEvents\": [{string.Join(", ", RecentEvents.ConvertAll(e => $"\"{e}\""))}]");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }

    #endregion

    #region Custom Assertions

    /// <summary>
    /// Custom assertions for game state validation.
    /// Provides detailed failure messages with state snapshots.
    /// </summary>
    public static class GameStateAssert
    {
        /// <summary>
        /// Asserts that the game is in the expected phase.
        /// </summary>
        public static void IsInPhase(GameStateService state, Phase expected, string context = null)
        {
            if (state.CurrentPhase != expected)
            {
                var message = BuildAssertionMessage(
                    $"Phase assertion failed{(context != null ? $" ({context})" : "")}",
                    $"Expected: {expected}",
                    $"Actual: {state.CurrentPhase}",
                    $"Turn: {state.TurnNumber}, Player: {state.CurrentPlayer}"
                );
                throw new AssertionException(message);
            }
        }

        /// <summary>
        /// Asserts that it's the expected player's turn.
        /// </summary>
        public static void IsPlayerTurn(GameStateService state, PlayerId expected)
        {
            if (state.CurrentPlayer != expected)
            {
                var message = BuildAssertionMessage(
                    "Player turn assertion failed",
                    $"Expected: {expected}",
                    $"Actual: {state.CurrentPlayer}",
                    $"Turn: {state.TurnNumber}, Phase: {state.CurrentPhase}"
                );
                throw new AssertionException(message);
            }
        }

        /// <summary>
        /// Asserts that it's the expected turn number.
        /// </summary>
        public static void IsTurnNumber(GameStateService state, int expected)
        {
            if (state.TurnNumber != expected)
            {
                var message = BuildAssertionMessage(
                    "Turn number assertion failed",
                    $"Expected: {expected}",
                    $"Actual: {state.TurnNumber}",
                    $"Phase: {state.CurrentPhase}, Player: {state.CurrentPlayer}"
                );
                throw new AssertionException(message);
            }
        }

        /// <summary>
        /// Asserts that player health equals expected value.
        /// </summary>
        public static void PlayerHealthEquals(TestPlayerStatus status, float expected, float tolerance = 0.01f)
        {
            float actual = status.CurrentHealth;
            if (Math.Abs(actual - expected) > tolerance)
            {
                var message = BuildAssertionMessage(
                    "Player health assertion failed",
                    $"Expected: {expected}",
                    $"Actual: {actual}",
                    $"Tolerance: {tolerance}"
                );
                throw new AssertionException(message);
            }
        }

        /// <summary>
        /// Asserts that player health is less than or equal to expected value.
        /// </summary>
        public static void PlayerHealthLessThanOrEqual(TestPlayerStatus status, float expected)
        {
            float actual = status.CurrentHealth;
            if (actual > expected)
            {
                var message = BuildAssertionMessage(
                    "Player health assertion failed",
                    $"Expected: <= {expected}",
                    $"Actual: {actual}"
                );
                throw new AssertionException(message);
            }
        }

        /// <summary>
        /// Asserts that a card with the given title is on the player's field.
        /// </summary>
        public static void HasCardOnField(TestPlayerCards cards, string cardTitle)
        {
            foreach (var card in cards.InvocationCards)
            {
                if (card?.Title == cardTitle)
                    return;
            }

            var fieldCards = new List<string>();
            foreach (var card in cards.InvocationCards)
            {
                if (card != null)
                    fieldCards.Add(card.Title);
            }

            var message = BuildAssertionMessage(
                "Card on field assertion failed",
                $"Expected card: {cardTitle}",
                $"Field cards: [{string.Join(", ", fieldCards)}]"
            );
            throw new AssertionException(message);
        }

        /// <summary>
        /// Asserts that the player's field has the expected number of invocation cards.
        /// </summary>
        public static void FieldCardCount(TestPlayerCards cards, int expected)
        {
            int actual = cards.InvocationCards.Count;
            if (actual != expected)
            {
                var message = BuildAssertionMessage(
                    "Field card count assertion failed",
                    $"Expected: {expected}",
                    $"Actual: {actual}"
                );
                throw new AssertionException(message);
            }
        }

        /// <summary>
        /// Asserts that the player's hand has the expected number of cards.
        /// </summary>
        public static void HandCardCount(TestPlayerCards cards, int expected)
        {
            int actual = cards.HandCards.Count;
            if (actual != expected)
            {
                var message = BuildAssertionMessage(
                    "Hand card count assertion failed",
                    $"Expected: {expected}",
                    $"Actual: {actual}"
                );
                throw new AssertionException(message);
            }
        }

        /// <summary>
        /// Asserts that the player's deck has at least the expected number of cards.
        /// </summary>
        public static void DeckHasAtLeast(TestPlayerCards cards, int minCards)
        {
            int actual = cards.Deck.Count;
            if (actual < minCards)
            {
                var message = BuildAssertionMessage(
                    "Deck card count assertion failed",
                    $"Expected: >= {minCards}",
                    $"Actual: {actual}"
                );
                throw new AssertionException(message);
            }
        }

        /// <summary>
        /// Asserts that the game is in GameOver phase.
        /// </summary>
        public static void IsGameOver(GameStateService state)
        {
            if (state.CurrentPhase != Phase.GameOver)
            {
                var message = BuildAssertionMessage(
                    "Game over assertion failed",
                    "Expected: Phase.GameOver",
                    $"Actual: {state.CurrentPhase}",
                    $"Turn: {state.TurnNumber}, Player: {state.CurrentPlayer}"
                );
                throw new AssertionException(message);
            }
        }

        /// <summary>
        /// Asserts that an event of type T was published to the event bus.
        /// </summary>
        public static void EventWasPublished<T>(TestEventBus bus, string context = null) where T : struct
        {
            if (!bus.HasEvent<T>())
            {
                var message = BuildAssertionMessage(
                    $"Event assertion failed{(context != null ? $" ({context})" : "")}",
                    $"Expected event: {typeof(T).Name}",
                    "Event was NOT published",
                    $"Published events: {bus.EventHistory.Count}"
                );
                throw new AssertionException(message);
            }
        }

        /// <summary>
        /// Asserts that no event of type T was published.
        /// </summary>
        public static void NoEventWasPublished<T>(TestEventBus bus, string context = null) where T : struct
        {
            if (bus.HasEvent<T>())
            {
                var message = BuildAssertionMessage(
                    $"Event assertion failed{(context != null ? $" ({context})" : "")}",
                    $"Did NOT expect event: {typeof(T).Name}",
                    "Event WAS published"
                );
                throw new AssertionException(message);
            }
        }

        /// <summary>
        /// Asserts that attack phase should be skipped (Player 1, Turn 1).
        /// </summary>
        public static void AttackPhaseShouldBeSkipped(GameStateService state)
        {
            if (!state.ShouldSkipAttackPhase)
            {
                var message = BuildAssertionMessage(
                    "Attack phase skip assertion failed",
                    "Expected: ShouldSkipAttackPhase = true",
                    $"Actual: ShouldSkipAttackPhase = {state.ShouldSkipAttackPhase}",
                    $"Turn: {state.TurnNumber}, Player: {state.CurrentPlayer}"
                );
                throw new AssertionException(message);
            }
        }

        /// <summary>
        /// Asserts that attack phase should NOT be skipped.
        /// </summary>
        public static void AttackPhaseNotSkipped(GameStateService state)
        {
            if (state.ShouldSkipAttackPhase)
            {
                var message = BuildAssertionMessage(
                    "Attack phase skip assertion failed",
                    "Expected: ShouldSkipAttackPhase = false",
                    $"Actual: ShouldSkipAttackPhase = {state.ShouldSkipAttackPhase}",
                    $"Turn: {state.TurnNumber}, Player: {state.CurrentPlayer}"
                );
                throw new AssertionException(message);
            }
        }

        #region Helper Methods

        private static string BuildAssertionMessage(params string[] lines)
        {
            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                sb.AppendLine(line);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Builds an assertion message with full game state snapshot.
        /// Use this for complex failures that need full context.
        /// </summary>
        public static string BuildAssertionMessageWithSnapshot(
            string message,
            GameStateSnapshot snapshot)
        {
            var sb = new StringBuilder();
            sb.AppendLine(message);
            sb.AppendLine();
            sb.AppendLine(snapshot.ToDetailedString());
            return sb.ToString();
        }

        #endregion
    }

    #endregion
}
