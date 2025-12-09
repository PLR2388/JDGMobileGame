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

            // Phase 19-20: Register InputManager MonoBehaviour from scene, then InputService
            builder.RegisterComponentInHierarchy<InputManager>();
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
            // Phase 19-20: Register CardPoolManager MonoBehaviour from scene, then adapter service
            builder.RegisterComponentInHierarchy<CardPoolManager>();
            builder.Register<ICardPoolService, CardPoolService>(Lifetime.Singleton);

            // Phase 9: Card Selection Service
            // Phase 19-20: Register CardSelectionManager MonoBehaviour from scene, then adapter service
            builder.RegisterComponentInHierarchy<CardSelectionManager>();
            builder.Register<ICardSelectionService, CardSelectionService>(Lifetime.Singleton);

            // Phase 9: Invocation Menu Service
            // Phase 19-20: Register InvocationMenuManager MonoBehaviour from scene, then adapter service
            builder.RegisterComponentInHierarchy<InvocationMenuManager>();
            builder.Register<IInvocationMenuService, InvocationMenuService>(Lifetime.Singleton);

            // Phase 9: Round Display Service
            // Phase 19-20: Register RoundDisplayManager MonoBehaviour from scene, then adapter service
            builder.RegisterComponentInHierarchy<RoundDisplayManager>();
            builder.Register<IRoundDisplayService, RoundDisplayService>(Lifetime.Singleton);

            // Phase 19-20: UI Manager
            // UIManager delegates to presenters (CardDisplayPresenter, DialogPresenter, CardSelectorPresenter)
            // Registered for use by GameLoop during transition to full MVP pattern
            builder.RegisterComponentInHierarchy<UIManager>();

            // Phase 17-18: Deck Management Service
            // DeckManagementService replaces GameState singleton for deck data storage
            builder.Register<IDeckManagementService, DeckManagementService>(Lifetime.Singleton);

            // Phase 17-18: Card Instantiation Service
            // CardInstantiationService replaces UnitManager singleton for GameObject creation
            builder.Register<ICardInstantiationService, CardInstantiationService>(Lifetime.Singleton);

            // Phase 21-22: Player & Card Management Use Cases
            // These are in the default assembly because they depend on legacy card types
            builder.Register<SummonPlayerEntityUseCase>(Lifetime.Transient);
            builder.Register<ResetCardsForNewTurnUseCase>(Lifetime.Transient);
            builder.Register<HandleCardDeathUseCase>(Lifetime.Transient);
            builder.Register<HandleCardAddedToFieldUseCase>(Lifetime.Transient);
            builder.Register<HandleCardRemovedFromFieldUseCase>(Lifetime.Transient);
            builder.Register<HandleHandCardsChangeUseCase>(Lifetime.Transient);
            builder.Register<HandleFieldCardChangedUseCase>(Lifetime.Transient);
        }
    }
}
