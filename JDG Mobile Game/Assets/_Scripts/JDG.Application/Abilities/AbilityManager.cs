using System;
using System.Collections.Generic;
using JDG.Domain;
using JDG.Domain.Events;

namespace JDG.Application.Abilities
{
    /// <summary>
    /// Manages ability execution and passive ability triggers.
    /// Central orchestrator for the ability system.
    /// </summary>
    public class AbilityManager
    {
        private readonly AbilityRegistry _registry;
        private readonly IEventBus _eventBus;
        private readonly Dictionary<AbilityTrigger, List<IPassiveAbility>> _passiveAbilities;

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
        /// </summary>
        /// <param name="trigger">The trigger type</param>
        /// <param name="context">Context for ability execution</param>
        public void TriggerPassiveAbilities(AbilityTrigger trigger, AbilityContext context)
        {
            var abilities = _passiveAbilities[trigger];

            foreach (var ability in abilities)
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
        }

        /// <summary>
        /// Subscribes to game events to automatically trigger passive abilities.
        /// </summary>
        private void SubscribeToGameEvents()
        {
            // OnCardPlayed -> OnSummon trigger
            _eventBus.Subscribe<CardPlayedEvent>(evt =>
            {
                // Create context from event (simplified - would need more info in practice)
                var context = new AbilityContext(
                    PlayerId.FromCardOwner(evt.Owner),
                    PlayerId.FromCardOwner(evt.Owner == CardOwner.Player1 ? CardOwner.Player2 : CardOwner.Player1),
                    null, // Would need the actual card
                    AbilityName.Default
                );

                TriggerPassiveAbilities(AbilityTrigger.OnSummon, context);
            });

            // OnCardDestroyed -> OnDeath trigger
            _eventBus.Subscribe<CardDestroyedEvent>(evt =>
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
            _eventBus.Subscribe<CardDrawnEvent>(evt =>
            {
                var context = new AbilityContext(
                    PlayerId.FromCardOwner(evt.Owner),
                    PlayerId.FromCardOwner(evt.Owner == CardOwner.Player1 ? CardOwner.Player2 : CardOwner.Player1),
                    null,
                    AbilityName.Default
                );

                TriggerPassiveAbilities(AbilityTrigger.OnCardDrawn, context);
            });

            // PhaseChanged -> OnTurnStart/OnTurnEnd triggers
            _eventBus.Subscribe<PhaseChangedEvent>(evt =>
            {
                if (evt.NewPhase == Phase.Draw)
                {
                    // Turn start
                    var context = new AbilityContext(
                        PlayerId.Player1, // Would need current player from event
                        PlayerId.Player2,
                        null,
                        AbilityName.Default
                    );

                    TriggerPassiveAbilities(AbilityTrigger.OnTurnStart, context);
                }
                else if (evt.NewPhase == Phase.End)
                {
                    // Turn end
                    var context = new AbilityContext(
                        PlayerId.Player1,
                        PlayerId.Player2,
                        null,
                        AbilityName.Default
                    );

                    TriggerPassiveAbilities(AbilityTrigger.OnTurnEnd, context);
                }
            });
        }

        /// <summary>
        /// Gets all passive abilities registered for a specific trigger.
        /// </summary>
        public IReadOnlyList<IPassiveAbility> GetPassiveAbilitiesForTrigger(AbilityTrigger trigger)
        {
            return _passiveAbilities[trigger].AsReadOnly();
        }
    }
}
