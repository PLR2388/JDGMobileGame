using VContainer;
using VContainer.Unity;
using JDG.Application;
using JDG.Application.Abilities;
using JDG.Application.Abilities.Implementations;
using JDG.Application.Repositories;
using JDG.Application.Services;
using JDG.Application.UseCases;
using JDG.Infrastructure.Events;
using JDG.Infrastructure.Repositories;
using JDG.Infrastructure.Services;

namespace JDG.Infrastructure.DI
{
    /// <summary>
    /// Main VContainer lifetime scope for the game.
    /// Registers all repositories, use cases, and services for dependency injection.
    /// </summary>
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // ============================================
            // INFRASTRUCTURE LAYER - Repositories & Services
            // ============================================

            // Event Bus (Singleton)
            builder.Register<IEventBus, EventBus>(Lifetime.Singleton);

            // Repositories (Singleton - maintain state throughout game session)
            builder.Register<ICardRepository, CardRepository>(Lifetime.Singleton);
            builder.Register<IDeckRepository, DeckRepository>(Lifetime.Singleton);
            builder.Register<IPlayerRepository, PlayerRepository>(Lifetime.Singleton);
            builder.Register<IGameStateRepository, GameStateRepository>(Lifetime.Singleton);

            // Services (Singleton - application-wide services)
            // Phase 15: Event-Driven Game State
            builder.Register<GameStateService>(Lifetime.Singleton);

            // Note: Legacy wrapper services (AudioService, DialogService, InputService, LocalizationService, RaycastService)
            // are registered in a separate Legacy assembly scope to avoid circular dependencies.

            // ============================================
            // APPLICATION LAYER - Use Cases
            // ============================================

            // Game Flow Use Cases (Transient - create new instance per use)
            builder.Register<StartGameUseCase>(Lifetime.Transient);
            builder.Register<EndTurnUseCase>(Lifetime.Transient);

            // Card Use Cases (Transient)
            builder.Register<DrawCardUseCase>(Lifetime.Transient);
            builder.Register<PlayCardUseCase>(Lifetime.Transient);

            // Combat Use Cases (Transient)
            builder.Register<AttackUseCase>(Lifetime.Transient);

            // ============================================
            // ABILITY SYSTEM - Phase 14
            // ============================================

            // Ability Core (Singleton)
            builder.Register<AbilityRegistry>(Lifetime.Singleton);
            builder.Register<AbilityManager>(Lifetime.Singleton);

            // Ability Factories (Singleton - can be reused to create abilities)
            // Phase 7: Comprehensive ability migration
            builder.Register<DrawCardsAbilityFactory>(Lifetime.Singleton);
            builder.Register<DestroyCardAbilityFactory>(Lifetime.Singleton);
            builder.Register<DeckSearchAbilityFactory>(Lifetime.Singleton);
            builder.Register<SacrificeAbilityFactory>(Lifetime.Singleton);
            builder.Register<StatModifierAbilityFactory>(Lifetime.Singleton);
            builder.Register<ProtectionAbilityFactory>(Lifetime.Singleton);
            builder.Register<CombatAbilityFactory>(Lifetime.Singleton);
            builder.Register<EffectAbilityFactory>(Lifetime.Singleton);
            builder.Register<EquipmentAbilityFactory>(Lifetime.Singleton);
            builder.Register<FieldAbilityFactory>(Lifetime.Singleton);
            builder.Register<SpecialAbilityFactory>(Lifetime.Singleton);

            // Register abilities after container is built
            builder.RegisterBuildCallback(container =>
            {
                var registry = container.Resolve<AbilityRegistry>();
                var drawFactory = container.Resolve<DrawCardsAbilityFactory>();
                var destroyFactory = container.Resolve<DestroyCardAbilityFactory>();

                // Register Draw abilities
                registry.Register(Domain.AbilityName.Draw2Cards, () => drawFactory.CreateDraw2Cards());
                registry.Register(Domain.AbilityName.Draw1Card, () => drawFactory.CreateDrawNCards(1));
                registry.Register(Domain.AbilityName.Draw3Cards, () => drawFactory.CreateDrawNCards(3));

                // Register Destroy abilities
                registry.Register(Domain.AbilityName.KillOpponentInvocation, () => destroyFactory.CreateKillOpponentInvocation());
                registry.Register(Domain.AbilityName.DestroyFieldATK, () => destroyFactory.CreateDestroyField());
            });
        }
    }
}
