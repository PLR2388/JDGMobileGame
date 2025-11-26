using UnityEngine;
using VContainer;
using VContainer.Unity;
using JDG.Application.Repositories;
using JDG.Application.UseCases;
using JDG.Infrastructure.Events;
using JDG.Infrastructure.Repositories;

namespace JDG.Infrastructure.DI
{
    /// <summary>
    /// VContainer LifetimeScope for the main game.
    /// Registers all dependencies for dependency injection.
    /// </summary>
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Register EventBus as singleton
            builder.Register<EventBus>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            // Register Repositories as singletons
            builder.Register<CardRepository>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .AsSelf(); // Also register as concrete type for initialization

            builder.Register<PlayerRepository>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            builder.Register<DeckRepository>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            builder.Register<GameStateRepository>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            // Register Use Cases as transient (new instance per request)
            builder.Register<DrawCardUseCase>(Lifetime.Transient);
            builder.Register<PlayCardUseCase>(Lifetime.Transient);
            builder.Register<AttackUseCase>(Lifetime.Transient);
            builder.Register<EndTurnUseCase>(Lifetime.Transient);
            builder.Register<StartGameUseCase>(Lifetime.Transient);

            Debug.Log("GameLifetimeScope: Dependencies registered");
        }

        protected override void Awake()
        {
            base.Awake();

            // Initialize CardRepository after container is built
            var cardRepo = Container.Resolve<CardRepository>();
            cardRepo.Initialize();
        }
    }
}
