using System.Collections.Generic;
using NUnit.Framework;
using JDG.Application.Abilities;
using JDG.Application.Repositories;
using JDG.Domain;
using JDG.Domain.Entities;
using JDG.Domain.Enums;
using JDG.Domain.ValueObjects;
using JDG.TestUtilities;

namespace JDG.Application.Tests.Abilities.Scenarios
{
    /// <summary>
    /// Base class for ability scenario tests.
    /// Provides common setup, test doubles, and helper methods for realistic game scenario testing.
    /// </summary>
    public abstract class AbilityScenarioTestBase
    {
        #region Test Infrastructure

        protected ScenarioPlayerRepository PlayerRepository { get; private set; }
        protected ScenarioEventBus EventBus { get; private set; }
        protected AbilityRegistry AbilityRegistry { get; private set; }

        protected Player Player1 { get; set; }
        protected Player Player2 { get; set; }

        [SetUp]
        public virtual void SetUp()
        {
            PlayerRepository = new ScenarioPlayerRepository();
            EventBus = new ScenarioEventBus();
            AbilityRegistry = new AbilityRegistry();

            // Setup default players - derived classes can override
            SetupDefaultPlayers();

            // Register abilities - derived classes can add more
            RegisterAbilities();
        }

        [TearDown]
        public virtual void TearDown()
        {
            EventBus.ClearAllSubscriptions();
            AbilityRegistry.ClearCache();
        }

        /// <summary>
        /// Override to customize player setup.
        /// </summary>
        protected virtual void SetupDefaultPlayers()
        {
            var (p1, p2) = GameStateFixtures.CreateBasicGameSetup();
            Player1 = p1;
            Player2 = p2;
            PlayerRepository.AddPlayer(Player1);
            PlayerRepository.AddPlayer(Player2);
        }

        /// <summary>
        /// Override to register additional abilities needed for tests.
        /// </summary>
        protected virtual void RegisterAbilities()
        {
            // Base class registers no abilities - derived classes should register what they need
        }

        #endregion

        #region Helper Methods - Card Creation

        /// <summary>
        /// Creates an invocation card with specified parameters.
        /// </summary>
        protected Card CreateCard(string name, int atk, int def, CardFamily family = CardFamily.Human)
        {
            return CardFactory.CreateInvocation(name, atk, def, family);
        }

        /// <summary>
        /// Creates an effect card.
        /// </summary>
        protected Card CreateEffectCard(string name, params EffectAbilityName[] abilities)
        {
            return CardFactory.CreateEffect(name, abilities);
        }

        /// <summary>
        /// Creates a field card.
        /// </summary>
        protected Card CreateFieldCard(string name, CardFamily family)
        {
            return CardFactory.CreateField(name, family);
        }

        /// <summary>
        /// Creates an equipment card.
        /// </summary>
        protected Card CreateEquipmentCard(string name)
        {
            return Card.CreateEquipment(
                CardId.New(),
                name,
                $"Description of {name}",
                $"Detailed description of {name}",
                new EquipmentAbilityName[0],
                false
            );
        }

        #endregion

        #region Helper Methods - Player Actions

        /// <summary>
        /// Places cards on a player's field.
        /// </summary>
        protected void PlaceOnField(Player player, params Card[] cards)
        {
            foreach (var card in cards)
            {
                // Add to deck, draw, then play
                if (!player.Deck.Contains(card))
                {
                    // Create a method to add card to hand directly for testing
                    // For now, we'll assume the card is already in hand or deck
                }

                if (player.Hand.Contains(card))
                {
                    player.PlayCard(card);
                }
            }
        }

        /// <summary>
        /// Note: Player.Hand is read-only. Cards must go through deck → draw.
        /// Use SearchDeckAndDraw or setup cards in deck during player creation.
        /// </summary>
        protected Card SearchAndDrawCard(Player player, string cardTitle)
        {
            return player.SearchDeckAndDraw(c => c.Title == cardTitle);
        }

        /// <summary>
        /// Simulates drawing cards from deck.
        /// </summary>
        protected void DrawCards(Player player, int count)
        {
            for (int i = 0; i < count && player.DeckCount > 0; i++)
            {
                player.DrawCard();
            }
        }

        /// <summary>
        /// Simulates removing a card from field (e.g., destruction).
        /// </summary>
        protected void RemoveFromField(Player player, Card card)
        {
            player.DestroyCardFromField(card);
        }

        #endregion

        #region Helper Methods - Ability Execution

        /// <summary>
        /// Gets an ability from the registry.
        /// </summary>
        protected IAbility GetAbility(AbilityName abilityName)
        {
            return AbilityRegistry.GetAbility(abilityName);
        }

        /// <summary>
        /// Creates an ability context for the given player and source card.
        /// Defaults to Player1 as current player.
        /// </summary>
        protected AbilityContext CreateContext(Card sourceCard, AbilityName abilityName)
        {
            return CreateContext(sourceCard, abilityName, PlayerId.Player1, null);
        }

        /// <summary>
        /// Creates an ability context with specific current player.
        /// </summary>
        protected AbilityContext CreateContext(Card sourceCard, AbilityName abilityName, PlayerId currentPlayer)
        {
            return CreateContext(sourceCard, abilityName, currentPlayer, null);
        }

        /// <summary>
        /// Creates an ability context for the given player and source card with optional target.
        /// </summary>
        protected AbilityContext CreateContext(
            Card sourceCard,
            AbilityName abilityName,
            PlayerId currentPlayer,
            Card targetCard)
        {
            var opponentId = currentPlayer == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1;

            var context = new AbilityContext(currentPlayer, opponentId, sourceCard, abilityName);
            if (targetCard != null)
            {
                context.TargetCard = targetCard;
            }

            return context;
        }

        /// <summary>
        /// Executes an ability and returns the result.
        /// </summary>
        protected AbilityResult ExecuteAbility(IAbility ability, AbilityContext context)
        {
            return ability.Execute(context);
        }

        /// <summary>
        /// Checks if an ability can be activated.
        /// </summary>
        protected bool CanActivateAbility(IAbility ability, AbilityContext context)
        {
            return ability.CanActivate(context);
        }

        #endregion

        #region Helper Methods - Assertions (delegating to AbilityTestAssertions)

        protected void AssertCardOnField(Player player, string cardName)
            => AbilityTestAssertions.AssertCardOnField(player, cardName);

        protected void AssertCardNotOnField(Player player, string cardName)
            => AbilityTestAssertions.AssertCardNotOnField(player, cardName);

        protected void AssertCardInHand(Player player, string cardName)
            => AbilityTestAssertions.AssertCardInHand(player, cardName);

        protected void AssertCardInGraveyard(Player player, string cardName)
            => AbilityTestAssertions.AssertCardInGraveyard(player, cardName);

        protected void AssertCardInDeck(Player player, string cardName)
            => AbilityTestAssertions.AssertCardInDeck(player, cardName);

        protected void AssertPlayerHealth(Player player, float expectedHealth)
            => AbilityTestAssertions.AssertPlayerHealth(player, expectedHealth);

        protected void AssertDeckCount(Player player, int expectedCount)
            => AbilityTestAssertions.AssertDeckCount(player, expectedCount);

        protected void AssertHandCount(Player player, int expectedCount)
            => AbilityTestAssertions.AssertHandCount(player, expectedCount);

        protected void AssertFieldCount(Player player, int expectedCount)
            => AbilityTestAssertions.AssertFieldCount(player, expectedCount);

        protected void AssertGraveyardCount(Player player, int expectedCount)
            => AbilityTestAssertions.AssertGraveyardCount(player, expectedCount);

        protected void AssertAbilitySuccess(AbilityResult result, string expectedMessageContains = null)
            => AbilityTestAssertions.AssertAbilitySuccess(result, expectedMessageContains);

        protected void AssertAbilityFailure(AbilityResult result, string expectedMessageContains = null)
            => AbilityTestAssertions.AssertAbilityFailure(result, expectedMessageContains);

        protected void AssertEventPublished<TEvent>() where TEvent : struct
            => AbilityTestAssertions.AssertEventPublished<TEvent>(EventBus.PublishedEvents);

        protected void AssertEventCount(int expectedCount)
            => AbilityTestAssertions.AssertEventCount(EventBus.PublishedEvents, expectedCount);

        #endregion
    }

    #region Test Doubles

    /// <summary>
    /// Test double for IPlayerRepository that supports scenario testing.
    /// </summary>
    public class ScenarioPlayerRepository : IPlayerRepository
    {
        private readonly Dictionary<PlayerId, Player> _players = new Dictionary<PlayerId, Player>();

        public void AddPlayer(Player player) => _players[player.Id] = player;

        public Player GetPlayer(PlayerId playerId) =>
            _players.ContainsKey(playerId) ? _players[playerId] : null;

        public void SavePlayer(Player player) => _players[player.Id] = player;

        public Player CreatePlayer(PlayerId playerId, CardId[] deckCardIds, int maxHealth = 30)
        {
            throw new System.NotImplementedException("Use AddPlayer for scenario tests");
        }

        public void ResetPlayer(PlayerId playerId)
        {
            _players.Remove(playerId);
        }

        public IEnumerable<Player> GetAllPlayers() => _players.Values;
    }

    /// <summary>
    /// Test double for IEventBus that tracks published events.
    /// </summary>
    public class ScenarioEventBus : IEventBus
    {
        public List<object> PublishedEvents { get; } = new List<object>();

        public void Publish<T>(T eventData) where T : struct
        {
            PublishedEvents.Add(eventData);
        }

        public System.IDisposable Subscribe<T>(System.Action<T> handler) where T : struct
        {
            return new TestDisposable();
        }

        public void ClearSubscriptions<T>() where T : struct { }

        public void ClearAllSubscriptions() => PublishedEvents.Clear();

        public List<T> GetEventsOfType<T>() where T : struct
        {
            var events = new List<T>();
            foreach (var e in PublishedEvents)
            {
                if (e is T typedEvent)
                {
                    events.Add(typedEvent);
                }
            }
            return events;
        }

        public int CountEventsOfType<T>() where T : struct
        {
            return GetEventsOfType<T>().Count;
        }

        private class TestDisposable : System.IDisposable
        {
            public void Dispose() { }
        }
    }

    #endregion
}
