using System;
using System.Collections.Generic;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;

namespace JDG.Application.Abilities
{
    /// <summary>
    /// Manages ability execution and passive ability triggers.
    /// Central orchestrator for the ability system.
    /// Phase 144: Implements IDisposable for proper subscription cleanup.
    /// </summary>
    public class AbilityManager : IDisposable
    {
        private readonly AbilityRegistry _registry;
        private readonly IEventBus _eventBus;
        private readonly Dictionary<AbilityTrigger, List<IPassiveAbility>> _passiveAbilities;

        // Phase 144: Store subscriptions for proper disposal
        private IDisposable _cardPlayedSubscription;
        private IDisposable _cardDestroyedSubscription;
        private IDisposable _cardDrawnSubscription;
        private IDisposable _phaseChangedSubscription;
        private IDisposable _turnStartSubscription;
        private IDisposable _turnEndSubscription;
        private bool _disposed;

        public AbilityManager(AbilityRegistry registry, IEventBus eventBus)
        {
            _registry = registry;
            _eventBus = eventBus;
            _passiveAbilities = new Dictionary<AbilityTrigger, List<IPassiveAbility>>();

            // Initialize trigger lists
            foreach (AbilityTrigger trigger in Enum.GetValues(typeof(AbilityTrigger)))
            {
                _passiveAbilities[trigger] = new List<IPassiveAbility>();
            }

            SubscribeToGameEvents();
        }

        /// <summary>
        /// Executes an ability by name with the given context.
        /// </summary>
        /// <param name="abilityName">The ability to execute</param>
        /// <param name="context">Context containing game state</param>
        /// <returns>Result of the ability execution</returns>
        public AbilityResult ExecuteAbility(AbilityName abilityName, AbilityContext context)
        {
            try
            {
                var ability = _registry.GetAbility(abilityName);

                if (!ability.CanActivate(context))
                {
                    return AbilityResult.Failure($"Cannot activate ability: {ability.Description}");
                }

                var result = ability.Execute(context);

                // Publish ability executed event
                _eventBus.Publish(new AbilityExecutedEvent
                {
                    AbilityName = abilityName,
                    PlayerId = context.CurrentPlayerId.ToCardOwner(),
                    IsSuccess = result.IsSuccess,
                    Message = result.Message
                });

                return result;
            }
            catch (KeyNotFoundException)
            {
                return AbilityResult.Failure($"Ability '{abilityName}' not found");
            }
            catch (Exception ex)
            {
                return AbilityResult.Failure($"Error executing ability: {ex.Message}");
            }
        }

        /// <summary>
        /// Registers a passive ability that triggers automatically.
        /// </summary>
        /// <param name="passiveAbility">The passive ability to register</param>
        public void RegisterPassiveAbility(IPassiveAbility passiveAbility)
        {
            if (!_passiveAbilities[passiveAbility.Trigger].Contains(passiveAbility))
            {
                _passiveAbilities[passiveAbility.Trigger].Add(passiveAbility);
            }
        }

        /// <summary>
        /// Unregisters a passive ability.
        /// </summary>
        public void UnregisterPassiveAbility(IPassiveAbility passiveAbility)
        {
            _passiveAbilities[passiveAbility.Trigger].Remove(passiveAbility);
        }

        /// <summary>
        /// Triggers all passive abilities for a specific trigger.
        /// Phase 144: Added try/catch to prevent one failed ability from stopping others.
        /// </summary>
        /// <param name="trigger">The trigger type</param>
        /// <param name="context">Context for ability execution</param>
        public void TriggerPassiveAbilities(AbilityTrigger trigger, AbilityContext context)
        {
            var abilities = _passiveAbilities[trigger];

            foreach (var ability in abilities)
            {
                // Phase 144: Wrap in try/catch so one failed ability doesn't stop others
                try
                {
                    if (ability.CanActivate(context))
                    {
                        var result = ability.Execute(context);

                        // Publish event for passive ability trigger
                        _eventBus.Publish(new AbilityExecutedEvent
                        {
                            AbilityName = ability.Name,
                            PlayerId = context.CurrentPlayerId.ToCardOwner(),
                            IsSuccess = result.IsSuccess,
                            Message = result.Message
                        });
                    }
                }
                catch (System.Exception ex)
                {
                    // Log error but continue executing remaining abilities
                    // Phase 144: Use Console.Error since JDG.Application is Unity-independent
                    var abilityName = ability != null ? ability.Name.ToString() : "unknown";
                    System.Console.Error.WriteLine($"[AbilityManager] Exception in ability '{abilityName}': {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Subscribes to game events to automatically trigger passive abilities.
        /// Phase 144: Store subscriptions for disposal.
        ///
        /// Phase 156 Note: These event-triggered contexts have null SourceCard by design.
        /// AbilityManager handles GLOBAL passive abilities (registered game-wide effects),
        /// not card-specific abilities. Card-specific abilities with proper SourceCard are
        /// handled by AbilityExecutorAdapter in the infrastructure layer.
        ///
        /// Global abilities that need card info should use event data (CardId, CardTitle)
        /// or be redesigned as card-specific abilities in AbilityExecutorAdapter.
        /// </summary>
        private void SubscribeToGameEvents()
        {
            // OnCardPlayed -> OnSummon trigger
            // Phase 156: SourceCard is null - this is for global abilities that respond to ANY card being played
            _cardPlayedSubscription = _eventBus.Subscribe<CardPlayedEvent>(evt =>
            {
                var context = new AbilityContext(
                    PlayerId.FromCardOwner(evt.Owner),
                    PlayerId.FromCardOwner(evt.Owner == CardOwner.Player1 ? CardOwner.Player2 : CardOwner.Player1),
                    null, // Global abilities don't have a specific source card
                    AbilityName.Default
                );

                TriggerPassiveAbilities(AbilityTrigger.OnSummon, context);
            });

            // OnCardDestroyed -> OnDeath trigger
            _cardDestroyedSubscription = _eventBus.Subscribe<CardDestroyedEvent>(evt =>
            {
                var context = new AbilityContext(
                    PlayerId.FromCardOwner(evt.Owner),
                    PlayerId.FromCardOwner(evt.Owner == CardOwner.Player1 ? CardOwner.Player2 : CardOwner.Player1),
                    null,
                    AbilityName.Default
                );

                TriggerPassiveAbilities(AbilityTrigger.OnDeath, context);
            });

            // OnCardDrawn -> OnCardDrawn trigger
            _cardDrawnSubscription = _eventBus.Subscribe<CardDrawnEvent>(evt =>
            {
                var context = new AbilityContext(
                    PlayerId.FromCardOwner(evt.Owner),
                    PlayerId.FromCardOwner(evt.Owner == CardOwner.Player1 ? CardOwner.Player2 : CardOwner.Player1),
                    null,
                    AbilityName.Default
                );

                TriggerPassiveAbilities(AbilityTrigger.OnCardDrawn, context);
            });

            // PhaseChanged -> Additional phase-based triggers (kept for backward compatibility)
            _phaseChangedSubscription = _eventBus.Subscribe<PhaseChangedEvent>(evt =>
            {
                // Phase-specific logic if needed in the future
                // Note: Turn start/end now handled by dedicated events below
            });

            // Phase 144: TurnStartEvent -> OnTurnStart trigger with proper player info
            _turnStartSubscription = _eventBus.Subscribe<TurnStartEvent>(evt =>
            {
                var currentPlayerId = PlayerId.FromCardOwner(evt.CurrentPlayer);
                var opponentId = PlayerId.FromCardOwner(
                    evt.CurrentPlayer == CardOwner.Player1 ? CardOwner.Player2 : CardOwner.Player1);

                var context = new AbilityContext(
                    currentPlayerId,
                    opponentId,
                    null,
                    AbilityName.Default
                );

                TriggerPassiveAbilities(AbilityTrigger.OnTurnStart, context);
            });

            // Phase 144: TurnEndEvent -> OnTurnEnd trigger with proper player info
            _turnEndSubscription = _eventBus.Subscribe<TurnEndEvent>(evt =>
            {
                var currentPlayerId = PlayerId.FromCardOwner(evt.CurrentPlayer);
                var opponentId = PlayerId.FromCardOwner(
                    evt.CurrentPlayer == CardOwner.Player1 ? CardOwner.Player2 : CardOwner.Player1);

                var context = new AbilityContext(
                    currentPlayerId,
                    opponentId,
                    null,
                    AbilityName.Default
                );

                TriggerPassiveAbilities(AbilityTrigger.OnTurnEnd, context);
            });
        }

        /// <summary>
        /// Gets all passive abilities registered for a specific trigger.
        /// </summary>
        public IReadOnlyList<IPassiveAbility> GetPassiveAbilitiesForTrigger(AbilityTrigger trigger)
        {
            return _passiveAbilities[trigger].AsReadOnly();
        }

        /// <summary>
        /// Phase 144: Disposes all EventBus subscriptions to prevent memory leaks.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            _cardPlayedSubscription?.Dispose();
            _cardDestroyedSubscription?.Dispose();
            _cardDrawnSubscription?.Dispose();
            _phaseChangedSubscription?.Dispose();
            _turnStartSubscription?.Dispose();
            _turnEndSubscription?.Dispose();

            _disposed = true;
        }
    }
}
