using JDG.Application.Abilities;
using JDG.Domain;

namespace JDG.Infrastructure.Bridge
{
    /// <summary>
    /// Adapter that wraps a legacy Ability to work with the new IAbility interface.
    /// This enables gradual migration from the old ability system to the new one
    /// using the Strangler Fig pattern.
    ///
    /// Phase 7.1: Part of ability migration infrastructure.
    ///
    /// Usage:
    /// - Legacy abilities can be wrapped to work in new ability system
    /// - New code interacts via IAbility interface
    /// - Legacy code continues to work unchanged
    ///
    /// Note: This adapter requires Unity context (Transform, PlayerCards) which
    /// limits testability. Use this only during migration; prefer native IAbility
    /// implementations for new abilities.
    /// </summary>
    public class LegacyAbilityAdapter : IAbility
    {
        private readonly Ability _legacyAbility;
        private readonly bool _hasAction;

        /// <summary>
        /// Creates an adapter for a legacy ability.
        /// </summary>
        /// <param name="legacyAbility">The legacy ability to wrap</param>
#pragma warning disable CS0618 // Type or member is obsolete
        public LegacyAbilityAdapter(Ability legacyAbility)
        {
            _legacyAbility = legacyAbility;
            _hasAction = legacyAbility.IsAction;
        }
#pragma warning restore CS0618

        public AbilityName Name => _legacyAbility.Name;
        public string Description => $"Legacy: {_legacyAbility.Name}";

        /// <summary>
        /// Checks if the legacy ability can be activated.
        /// Note: Legacy abilities often have complex activation requirements
        /// that depend on game state accessed through PlayerCards.
        /// </summary>
        public bool CanActivate(AbilityContext context)
        {
            // Legacy abilities often require specific game state
            // We can't fully determine this without the full legacy context
            // Return true to allow the Execute method to handle validation
            return true;
        }

        /// <summary>
        /// Executes the legacy ability using the new context system.
        /// This method bridges the gap between the new pure context-based system
        /// and the legacy Unity-dependent system.
        ///
        /// IMPORTANT: This method cannot fully execute legacy abilities because
        /// they require Unity-specific objects (Transform canvas, PlayerCards).
        /// Use this adapter primarily for:
        /// 1. Tracking which abilities need migration
        /// 2. Testing the ability routing system
        /// 3. Gradual migration where some paths use new system
        /// </summary>
        public AbilityResult Execute(AbilityContext context)
        {
            // Legacy abilities need Unity context that we don't have here
            // Return a result indicating this ability needs legacy execution
            return AbilityResult.NeedsLegacyExecution(
                $"Legacy ability '{Name}' requires Unity context. " +
                "Use AbilityMigrationService.ExecuteLegacy() instead."
            );
        }

        /// <summary>
        /// Gets the wrapped legacy ability for use in legacy code paths.
        /// </summary>
#pragma warning disable CS0618 // Type or member is obsolete
        public Ability GetLegacyAbility() => _legacyAbility;
#pragma warning restore CS0618

        /// <summary>
        /// Returns whether this is an action ability (requires user interaction).
        /// </summary>
        public bool HasAction => _hasAction;
    }
}
