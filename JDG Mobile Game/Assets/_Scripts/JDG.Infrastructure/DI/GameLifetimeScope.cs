using VContainer;
using VContainer.Unity;
using JDG.Application;
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
            // Phase 13: Service Layer Foundation
            builder.Register<IAudioService, AudioService>(Lifetime.Singleton);
            builder.Register<ILocalizationService, LocalizationService>(Lifetime.Singleton);
            builder.Register<IDialogService, DialogService>(Lifetime.Singleton);
            builder.Register<IInputService, InputService>(Lifetime.Singleton);

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

            // TODO: Add ability use cases when implemented
            // builder.Register<ActivateAbilityUseCase>(Lifetime.Transient);
        }
    }
}
