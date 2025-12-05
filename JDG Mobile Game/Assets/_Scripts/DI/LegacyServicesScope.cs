using VContainer;
using VContainer.Unity;
using JDG.Application.Services;
using JDG.Infrastructure.Services;

namespace JDG.DI
{
    /// <summary>
    /// VContainer lifetime scope for legacy wrapper services.
    /// Registers services that wrap old singleton managers during migration.
    /// </summary>
    public class LegacyServicesScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Legacy wrapper services (wrap old singleton managers)
            // These will be removed once migration to new architecture is complete
            builder.Register<IAudioService, AudioService>(Lifetime.Singleton);
            builder.Register<ILocalizationService, LocalizationService>(Lifetime.Singleton);
            builder.Register<IDialogService, DialogService>(Lifetime.Singleton);
            builder.Register<IInputService, InputService>(Lifetime.Singleton);

            // Phase 1: Input System Services
            // RaycastService is here (not in GameLifetimeScope) because it depends on
            // legacy types (InGameCard, PhysicalCardDisplay) that live in the default assembly
            builder.Register<IRaycastService, RaycastService>(Lifetime.Singleton);

            // Phase 3: Player Management Services
            // PlayerService is here because it manages state that will eventually sync with
            // legacy PlayerStatus MonoBehaviours during the transition period
            builder.Register<IPlayerService, PlayerService>(Lifetime.Singleton);

            // Phase 4: Card Management Services
            // CardCollectionServiceAdapter bridges to CardManager singleton during migration
            builder.Register<ICardCollectionService, CardCollectionServiceAdapter>(Lifetime.Singleton);

            // Phase 6: Card Placement Services
            // CardPlacementService extracts business logic from *Functions MonoBehaviours
            builder.Register<ICardPlacementService, CardPlacementService>(Lifetime.Singleton);

            // Phase 8: Deck Initialization Service
            // DeckInitializationService removes GameState/UnitManager singleton access from PlayerCards
            builder.Register<IDeckInitializationService, DeckInitializationService>(Lifetime.Singleton);

            // Phase 9: Card Pool Service
            // CardPoolService removes CardPoolManager singleton access from UI components
            builder.Register<ICardPoolService, CardPoolService>(Lifetime.Singleton);
        }
    }
}
