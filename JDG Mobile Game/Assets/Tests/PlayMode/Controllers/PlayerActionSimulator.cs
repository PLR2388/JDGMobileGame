using System;
using System.Collections;
using System.Collections.Generic;
using JDG.Application.Services;
using JDG.Domain;
using DomainCardOwner = JDG.Domain.CardOwner;
using JDG.Domain.Events;
using JDG.PlayMode.Tests.TestHelpers;
using UnityEngine;

namespace JDG.PlayMode.Tests.Controllers
{
    #region Test Card Configurations

    /// <summary>
    /// Configuration for a test invocation card.
    /// Used to create test cards without needing ScriptableObjects.
    /// </summary>
    public class TestInvocationCardConfig
    {
        public string Title { get; set; } = "TestCard";
        public float Attack { get; set; } = 5f;
        public float Defense { get; set; } = 5f;
        public bool CanDirectAttack { get; set; } = false;
        public bool CantBeAttacked { get; set; } = false;
        public bool HasAggro { get; set; } = false;
        public List<AbilityName> Abilities { get; set; } = new List<AbilityName>();

        public static TestInvocationCardConfig Create(string title, float atk, float def)
        {
            return new TestInvocationCardConfig
            {
                Title = title,
                Attack = atk,
                Defense = def
            };
        }

        public static TestInvocationCardConfig CreateWithAbility(string title, float atk, float def, AbilityName ability)
        {
            return new TestInvocationCardConfig
            {
                Title = title,
                Attack = atk,
                Defense = def,
                Abilities = new List<AbilityName> { ability }
            };
        }
    }

    /// <summary>
    /// Configuration for a test effect card.
    /// </summary>
    public class TestEffectCardConfig
    {
        public string Title { get; set; } = "TestEffectCard";
        public List<AbilityName> Abilities { get; set; } = new List<AbilityName>();
    }

    /// <summary>
    /// Configuration for a test equipment card.
    /// </summary>
    public class TestEquipmentCardConfig
    {
        public string Title { get; set; } = "TestEquipment";
        public float AttackBonus { get; set; } = 0f;
        public float DefenseBonus { get; set; } = 0f;
        public List<AbilityName> Abilities { get; set; } = new List<AbilityName>();
    }

    /// <summary>
    /// Configuration for a test field card.
    /// </summary>
    public class TestFieldCardConfig
    {
        public string Title { get; set; } = "TestField";
        public List<AbilityName> Abilities { get; set; } = new List<AbilityName>();
    }

    /// <summary>
    /// Represents a simulated combat card for target validation.
    /// </summary>
    public class SimulatedCombatCard
    {
        public string Title { get; set; }
        public float Attack { get; set; }
        public float Defense { get; set; }
        public bool CantBeAttacked { get; set; }
        public bool HasAggro { get; set; }
        public bool CanDirectAttack { get; set; }
        public bool IsPlayerEntity { get; set; }
    }

    #endregion

    /// <summary>
    /// Simulates player actions without UI interaction.
    /// Works directly with services to perform game actions.
    /// </summary>
    public class PlayerActionSimulator
    {
        #region Dependencies

        private readonly GameTestController _controller;
        private readonly TestEventBus _eventBus;
        private readonly TestCardCollectionService _cardCollectionService;
        private readonly TestPlayerStatusProvider _playerStatusProvider;
        private readonly ICombatLogic _combatLogic;

        // Simulated field state (since we can't create real InGameCards easily)
        private List<SimulatedCombatCard> _player1Field = new List<SimulatedCombatCard>();
        private List<SimulatedCombatCard> _player2Field = new List<SimulatedCombatCard>();
        private SimulatedCombatCard _currentAttacker;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new PlayerActionSimulator using the given test controller.
        /// </summary>
        /// <param name="controller">The game test controller.</param>
        public PlayerActionSimulator(GameTestController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _eventBus = controller.EventBus;
            _cardCollectionService = controller.CardCollectionService;
            _playerStatusProvider = controller.PlayerStatusProvider;
            _combatLogic = controller.CombatLogic;
        }

        #endregion

        #region Card Placement

        /// <summary>
        /// Synchronously places an invocation card on the field.
        /// Use this in setup methods that aren't coroutines.
        /// </summary>
        /// <param name="card">The card configuration.</param>
        /// <param name="isPlayer1">Whether it's player 1's card.</param>
        public void PlayInvocationCardSync(TestInvocationCardConfig card, bool isPlayer1 = true)
        {
            var simulatedCard = new SimulatedCombatCard
            {
                Title = card.Title,
                Attack = card.Attack,
                Defense = card.Defense,
                CantBeAttacked = card.CantBeAttacked,
                HasAggro = card.HasAggro,
                CanDirectAttack = card.CanDirectAttack,
                IsPlayerEntity = false
            };

            var field = isPlayer1 ? _player1Field : _player2Field;

            // Check field limit (max 4 invocations)
            if (field.Count >= 4)
            {
                Debug.LogWarning($"Cannot place {card.Title}: Field is full (max 4 invocations)");
                return;
            }

            field.Add(simulatedCard);

            // Publish card played event
            _eventBus.Publish(new CardPlayedEvent
            {
                CardTitle = card.Title,
                Owner = isPlayer1 ? DomainCardOwner.Player1 : DomainCardOwner.Player2
            });
        }

        /// <summary>
        /// Simulates playing an invocation card from hand to field.
        /// </summary>
        /// <param name="card">The card configuration.</param>
        /// <param name="isPlayer1">Whether it's player 1's card.</param>
        /// <returns>IEnumerator for coroutine.</returns>
        public IEnumerator PlayInvocationCard(TestInvocationCardConfig card, bool isPlayer1 = true)
        {
            PlayInvocationCardSync(card, isPlayer1);
            yield return null;
        }

        /// <summary>
        /// Synchronously plays an effect card.
        /// Use this in setup methods that aren't coroutines.
        /// </summary>
        /// <param name="card">The effect card configuration.</param>
        /// <param name="isPlayer1">Whether it's player 1's card.</param>
        public void PlayEffectCardSync(TestEffectCardConfig card, bool isPlayer1 = true)
        {
            _eventBus.Publish(new CardPlayedEvent
            {
                CardTitle = card.Title,
                Owner = isPlayer1 ? DomainCardOwner.Player1 : DomainCardOwner.Player2
            });
        }

        /// <summary>
        /// Simulates playing an effect card.
        /// </summary>
        /// <param name="card">The effect card configuration.</param>
        /// <param name="isPlayer1">Whether it's player 1's card.</param>
        /// <returns>IEnumerator for coroutine.</returns>
        public IEnumerator PlayEffectCard(TestEffectCardConfig card, bool isPlayer1 = true)
        {
            PlayEffectCardSync(card, isPlayer1);
            yield return null;
        }

        /// <summary>
        /// Synchronously plays a field card.
        /// Use this in setup methods that aren't coroutines.
        /// </summary>
        /// <param name="card">The field card configuration.</param>
        /// <param name="isPlayer1">Whether it's player 1's card.</param>
        public void PlayFieldCardSync(TestFieldCardConfig card, bool isPlayer1 = true)
        {
            _eventBus.Publish(new CardPlayedEvent
            {
                CardTitle = card.Title,
                Owner = isPlayer1 ? DomainCardOwner.Player1 : DomainCardOwner.Player2
            });
        }

        /// <summary>
        /// Simulates playing a field card.
        /// </summary>
        /// <param name="card">The field card configuration.</param>
        /// <param name="isPlayer1">Whether it's player 1's card.</param>
        /// <returns>IEnumerator for coroutine.</returns>
        public IEnumerator PlayFieldCard(TestFieldCardConfig card, bool isPlayer1 = true)
        {
            PlayFieldCardSync(card, isPlayer1);
            yield return null;
        }

        /// <summary>
        /// Sets up the player entity card (the "Player" card that can be attacked directly).
        /// </summary>
        /// <param name="title">Title of the player entity.</param>
        /// <param name="isPlayer1">Whether it's player 1's entity.</param>
        public void SetupPlayerEntity(string title, bool isPlayer1 = true)
        {
            var playerEntity = new SimulatedCombatCard
            {
                Title = title,
                Attack = 0,
                Defense = 0,
                IsPlayerEntity = true
            };

            var field = isPlayer1 ? _player1Field : _player2Field;
            // Remove any existing player entity
            field.RemoveAll(c => c.IsPlayerEntity);
            field.Add(playerEntity);
        }

        #endregion

        #region Combat

        /// <summary>
        /// Selects an attacker for the next combat action.
        /// </summary>
        /// <param name="attackerTitle">Title of the attacking card.</param>
        /// <param name="isPlayer1">Whether the attacker belongs to player 1.</param>
        public void SelectAttacker(string attackerTitle, bool isPlayer1 = true)
        {
            var field = isPlayer1 ? _player1Field : _player2Field;
            _currentAttacker = field.Find(c => c.Title == attackerTitle && !c.IsPlayerEntity);

            if (_currentAttacker == null)
            {
                throw new InvalidOperationException($"Attacker '{attackerTitle}' not found on field");
            }
        }

        /// <summary>
        /// Gets valid targets for the current attacker.
        /// </summary>
        /// <param name="isPlayer1Attacking">Whether player 1 is attacking.</param>
        /// <returns>List of valid target cards.</returns>
        public List<SimulatedCombatCard> GetValidTargets(bool isPlayer1Attacking)
        {
            if (_currentAttacker == null)
            {
                throw new InvalidOperationException("No attacker selected. Call SelectAttacker first.");
            }

            var opponentField = isPlayer1Attacking ? _player2Field : _player1Field;
            var validTargets = new List<SimulatedCombatCard>();

            // Check for aggro cards first
            var aggroCards = opponentField.FindAll(c => c.HasAggro && !c.CantBeAttacked && !c.IsPlayerEntity);

            if (aggroCards.Count > 0 && !_currentAttacker.CanDirectAttack)
            {
                // Normal attackers must target aggro cards only
                return aggroCards;
            }

            if (aggroCards.Count > 0 && _currentAttacker.CanDirectAttack)
            {
                // Direct attackers can target aggro cards OR the player
                validTargets.AddRange(aggroCards);
                var playerEntity = opponentField.Find(c => c.IsPlayerEntity);
                if (playerEntity != null)
                {
                    validTargets.Add(playerEntity);
                }
                return validTargets;
            }

            // No aggro cards - check for attackable cards
            foreach (var card in opponentField)
            {
                if (!card.CantBeAttacked && !card.IsPlayerEntity)
                {
                    validTargets.Add(card);
                }
            }

            // If no valid card targets and attacker can direct attack (or field is empty)
            bool canDirectAttack = _currentAttacker.CanDirectAttack || validTargets.Count == 0;
            if (canDirectAttack)
            {
                var playerEntity = opponentField.Find(c => c.IsPlayerEntity);
                if (playerEntity != null)
                {
                    validTargets.Add(playerEntity);
                }
            }

            return validTargets;
        }

        /// <summary>
        /// Executes an attack on a target card.
        /// </summary>
        /// <param name="targetTitle">Title of the target card.</param>
        /// <param name="isPlayer1Attacking">Whether player 1 is attacking.</param>
        /// <returns>IEnumerator for coroutine.</returns>
        public IEnumerator AttackTarget(string targetTitle, bool isPlayer1Attacking = true)
        {
            if (_currentAttacker == null)
            {
                throw new InvalidOperationException("No attacker selected. Call SelectAttacker first.");
            }

            var opponentField = isPlayer1Attacking ? _player2Field : _player1Field;
            var target = opponentField.Find(c => c.Title == targetTitle);

            if (target == null)
            {
                throw new InvalidOperationException($"Target '{targetTitle}' not found on opponent's field");
            }

            // Is this a direct attack on the player?
            if (target.IsPlayerEntity)
            {
                yield return ExecuteDirectAttack(isPlayer1Attacking);
                yield break;
            }

            // Combat calculation
            float damage = _combatLogic.ComputeDamage(_currentAttacker.Attack, target.Defense);

            if (damage < 0)
            {
                // Attacker wins - defender destroyed
                opponentField.Remove(target);
                _eventBus.Publish(new CardDestroyedEvent
                {
                    Owner = isPlayer1Attacking ? DomainCardOwner.Player2 : DomainCardOwner.Player1,
                    Reason = $"Destroyed by {_currentAttacker.Title}"
                });
            }
            else if (damage > 0)
            {
                // Defender wins - attacker destroyed
                var attackerField = isPlayer1Attacking ? _player1Field : _player2Field;
                attackerField.Remove(_currentAttacker);
                _eventBus.Publish(new CardDestroyedEvent
                {
                    Owner = isPlayer1Attacking ? DomainCardOwner.Player1 : DomainCardOwner.Player2,
                    Reason = $"Destroyed by {target.Title}"
                });
            }
            else
            {
                // Tie - both destroyed
                opponentField.Remove(target);
                var attackerField = isPlayer1Attacking ? _player1Field : _player2Field;
                attackerField.Remove(_currentAttacker);
                _eventBus.Publish(new CardDestroyedEvent
                {
                    Owner = isPlayer1Attacking ? DomainCardOwner.Player2 : DomainCardOwner.Player1,
                    Reason = "Mutual destruction"
                });
                _eventBus.Publish(new CardDestroyedEvent
                {
                    Owner = isPlayer1Attacking ? DomainCardOwner.Player1 : DomainCardOwner.Player2,
                    Reason = "Mutual destruction"
                });
            }

            _currentAttacker = null;
            yield return null;
        }

        /// <summary>
        /// Executes a direct attack on the opponent player.
        /// </summary>
        /// <param name="isPlayer1Attacking">Whether player 1 is attacking.</param>
        /// <returns>IEnumerator for coroutine.</returns>
        public IEnumerator ExecuteDirectAttack(bool isPlayer1Attacking = true)
        {
            if (_currentAttacker == null)
            {
                throw new InvalidOperationException("No attacker selected. Call SelectAttacker first.");
            }

            float damage = -_currentAttacker.Attack;

            if (isPlayer1Attacking)
            {
                _playerStatusProvider.OpponentPlayerStatus.ChangePv(damage);
            }
            else
            {
                _playerStatusProvider.CurrentPlayerStatus.ChangePv(damage);
            }

            _eventBus.Publish(new PlayerHealthChangedEvent
            {
                Player = isPlayer1Attacking ? DomainCardOwner.Player2 : DomainCardOwner.Player1,
                NewHealth = isPlayer1Attacking
                    ? _playerStatusProvider.OpponentPlayerStatus.CurrentHealth
                    : _playerStatusProvider.CurrentPlayerStatus.CurrentHealth
            });

            _currentAttacker = null;
            yield return null;
        }

        #endregion

        #region Field State

        /// <summary>
        /// Gets the number of cards on player 1's field.
        /// </summary>
        public int Player1FieldCount => _player1Field.Count(c => !c.IsPlayerEntity);

        /// <summary>
        /// Gets the number of cards on player 2's field.
        /// </summary>
        public int Player2FieldCount => _player2Field.Count(c => !c.IsPlayerEntity);

        /// <summary>
        /// Checks if a card is on player 1's field.
        /// </summary>
        public bool IsCardOnPlayer1Field(string title)
        {
            return _player1Field.Exists(c => c.Title == title && !c.IsPlayerEntity);
        }

        /// <summary>
        /// Checks if a card is on player 2's field.
        /// </summary>
        public bool IsCardOnPlayer2Field(string title)
        {
            return _player2Field.Exists(c => c.Title == title && !c.IsPlayerEntity);
        }

        /// <summary>
        /// Clears all cards from both fields.
        /// </summary>
        public void ClearFields()
        {
            _player1Field.Clear();
            _player2Field.Clear();
            _currentAttacker = null;
        }

        /// <summary>
        /// Gets player 1's field cards.
        /// </summary>
        public List<SimulatedCombatCard> GetPlayer1Field()
        {
            return _player1Field.FindAll(c => !c.IsPlayerEntity);
        }

        /// <summary>
        /// Gets player 2's field cards.
        /// </summary>
        public List<SimulatedCombatCard> GetPlayer2Field()
        {
            return _player2Field.FindAll(c => !c.IsPlayerEntity);
        }

        #endregion

        #region Helper Extension

        // Helper method for List since we're using an older C# version
        private int Count(List<SimulatedCombatCard> list, Func<SimulatedCombatCard, bool> predicate)
        {
            int count = 0;
            foreach (var item in list)
            {
                if (predicate(item))
                    count++;
            }
            return count;
        }

        #endregion
    }

    // Extension method helper class
    internal static class ListExtensions
    {
        public static int Count<T>(this List<T> list, Func<T, bool> predicate)
        {
            int count = 0;
            foreach (var item in list)
            {
                if (predicate(item))
                    count++;
            }
            return count;
        }
    }
}
