using System;
using System.Collections.Generic;
using System.Linq;
using JDG.Domain.ValueObjects;

namespace JDG.Domain.Entities
{
    /// <summary>
    /// Domain entity representing a player in the game.
    /// Pure C# - no Unity dependencies.
    /// ECS-ready: Structured to easily convert to ECS components.
    /// </summary>
    public class Player
    {
        // Identity
        public PlayerId Id { get; }

        // Card Collections (ECS: can become buffers)
        private readonly List<Card> _deck;
        private readonly List<Card> _hand;
        private readonly List<Card> _field;
        private readonly List<Card> _graveyard;

        // Player State (ECS: can become components)
        public int Health { get; private set; }
        public int MaxHealth { get; }
        public int Shields { get; private set; }
        public bool BlockAttack { get; private set; }
        public bool SkipCurrentDraw { get; set; }

        // Public read-only access to collections
        public IReadOnlyList<Card> Deck => _deck.AsReadOnly();
        public IReadOnlyList<Card> Hand => _hand.AsReadOnly();
        public IReadOnlyList<Card> Field => _field.AsReadOnly();
        public IReadOnlyList<Card> Graveyard => _graveyard.AsReadOnly();

        // Counts (for UI and logic)
        public int DeckCount => _deck.Count;
        public int HandCount => _hand.Count;
        public int FieldCount => _field.Count;
        public int GraveyardCount => _graveyard.Count;

        public Player(PlayerId id, IEnumerable<Card> deck, int maxHealth = 30)
        {
            Id = id;
            MaxHealth = maxHealth;
            Health = maxHealth;
            Shields = 0;
            BlockAttack = false;
            SkipCurrentDraw = false;

            _deck = new List<Card>(deck);
            _hand = new List<Card>();
            _field = new List<Card>();
            _graveyard = new List<Card>();
        }

        #region Card Operations

        /// <summary>
        /// Draws a card from the deck to hand.
        /// Returns the drawn card, or null if deck is empty.
        /// </summary>
        public Card DrawCard()
        {
            if (_deck.Count == 0)
                return null;

            var card = _deck[^1]; // Take from top of deck
            _deck.RemoveAt(_deck.Count - 1);
            _hand.Add(card);

            return card;
        }

        /// <summary>
        /// Plays a card from hand to field.
        /// Returns true if successful, false if card not in hand.
        /// </summary>
        public bool PlayCard(Card card)
        {
            if (!_hand.Contains(card))
                return false;

            _hand.Remove(card);
            _field.Add(card);

            return true;
        }

        /// <summary>
        /// Removes a card from field to graveyard.
        /// </summary>
        public bool DestroyCardFromField(Card card)
        {
            if (!_field.Contains(card))
                return false;

            _field.Remove(card);
            _graveyard.Add(card);

            return true;
        }

        /// <summary>
        /// Discards a card from hand to graveyard.
        /// </summary>
        public bool DiscardCard(Card card)
        {
            if (!_hand.Contains(card))
                return false;

            _hand.Remove(card);
            _graveyard.Add(card);

            return true;
        }

        /// <summary>
        /// Returns a card from graveyard to hand.
        /// </summary>
        public bool ReturnCardToHand(Card card)
        {
            if (!_graveyard.Contains(card))
                return false;

            _graveyard.Remove(card);
            _hand.Add(card);

            return true;
        }

        /// <summary>
        /// Searches deck for a card matching predicate and adds to hand.
        /// </summary>
        public Card SearchDeckAndDraw(Func<Card, bool> predicate)
        {
            var card = _deck.FirstOrDefault(predicate);
            if (card == null)
                return null;

            _deck.Remove(card);
            _hand.Add(card);

            return card;
        }

        /// <summary>
        /// Returns a card from graveyard to field.
        /// Used for resurrection abilities.
        /// </summary>
        public bool ReturnCardToField(Card card)
        {
            if (!_graveyard.Contains(card) || _field.Count >= 4)
                return false;

            _graveyard.Remove(card);
            _field.Add(card);

            return true;
        }

        /// <summary>
        /// Returns a card from field to hand.
        /// Used for bounce/return effects.
        /// </summary>
        public bool ReturnFieldCardToHand(Card card)
        {
            if (!_field.Contains(card))
                return false;

            _field.Remove(card);
            _hand.Add(card);

            return true;
        }

        #endregion

        #region Health & Shields

        /// <summary>
        /// Takes damage, reducing shields first, then health.
        /// Returns actual damage dealt to health (after shields).
        /// </summary>
        public int TakeDamage(int damage)
        {
            if (damage <= 0)
                return 0;

            int healthDamage = 0;

            if (Shields > 0)
            {
                int shieldsRemoved = Math.Min(Shields, damage);
                Shields -= shieldsRemoved;
                damage -= shieldsRemoved;
            }

            if (damage > 0)
            {
                healthDamage = damage;
                Health = Math.Max(0, Health - damage);
            }

            return healthDamage;
        }

        /// <summary>
        /// Heals the player up to max health.
        /// Returns actual amount healed.
        /// </summary>
        public int Heal(int amount)
        {
            if (amount <= 0)
                return 0;

            int oldHealth = Health;
            Health = Math.Min(MaxHealth, Health + amount);
            return Health - oldHealth;
        }

        /// <summary>
        /// Adds shields to the player.
        /// </summary>
        public void AddShields(int amount)
        {
            if (amount > 0)
                Shields += amount;
        }

        /// <summary>
        /// Removes shields from the player.
        /// </summary>
        public void RemoveShields(int amount)
        {
            if (amount > 0)
                Shields = Math.Max(0, Shields - amount);
        }

        /// <summary>
        /// Sets block attack status (prevents attack phase).
        /// </summary>
        public void SetBlockAttack(bool blocked)
        {
            BlockAttack = blocked;
        }

        /// <summary>
        /// Returns true if player is defeated (health <= 0).
        /// </summary>
        public bool IsDefeated => Health <= 0;

        #endregion

        #region ECS Snapshot (for future ECS migration)

        /// <summary>
        /// Creates a snapshot of player state (for ECS conversion or serialization).
        /// </summary>
        public PlayerSnapshot CreateSnapshot()
        {
            return new PlayerSnapshot
            {
                Id = Id,
                Health = Health,
                MaxHealth = MaxHealth,
                Shields = Shields,
                BlockAttack = BlockAttack,
                DeckCount = DeckCount,
                HandCount = HandCount,
                FieldCount = FieldCount,
                GraveyardCount = GraveyardCount
            };
        }

        #endregion
    }

    /// <summary>
    /// Read-only snapshot of player state.
    /// Can be used for UI, serialization, or ECS conversion.
    /// </summary>
    public struct PlayerSnapshot
    {
        public PlayerId Id;
        public int Health;
        public int MaxHealth;
        public int Shields;
        public bool BlockAttack;
        public int DeckCount;
        public int HandCount;
        public int FieldCount;
        public int GraveyardCount;
    }
}
