using VContainer;
using VContainer.Unity;
using JDG.Application.Services;
using JDG.Infrastructure.Services;

namespace JDG.DI
{
    /// <summary>
    /// VContainer lifetime scope for shared services that don't depend on scene MonoBehaviours.
    /// Place this in the preload scene as a child of GameLifetimeScope.
    /// Phase 46: Split from LegacyServicesScope for proper scene-based DI.
    /// </summary>
    public class SharedServicesScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // ============================================
            // PURE SERVICES - No scene MonoBehaviour dependencies
            // ============================================

            // Audio, Localization, Dialog services (wrap singletons)
            builder.Register<IAudioService, AudioService>(Lifetime.Singleton);
            builder.Register<ILocalizationService, LocalizationService>(Lifetime.Singleton);
            builder.Register<IDialogService, DialogService>(Lifetime.Singleton);

            // Card Visual Service - abstracts Unity Material dependencies
            builder.Register<ICardVisualService, CardVisualService>(Lifetime.Singleton);

            // Raycast Service - depends on legacy types but not scene MonoBehaviours
            builder.Register<IRaycastService, RaycastService>(Lifetime.Singleton);

            // Player Service - manages player state
            builder.Register<IPlayerService, PlayerService>(Lifetime.Singleton);

            // Card Placement Service - business logic for card placement
            builder.Register<ICardPlacementService, CardPlacementService>(Lifetime.Singleton);

            // Deck Initialization Service
            builder.Register<IDeckInitializationService, DeckInitializationService>(Lifetime.Singleton);

            // Card Data Provider - replaces ResourceSystem.Instance
            builder.Register<JDG.Application.Services.ICardDataProvider, JDG.Infrastructure.Services.CardDataProvider>(Lifetime.Singleton);

            // Deck Management Service - deck data storage
            builder.Register<IDeckManagementService, DeckManagementService>(Lifetime.Singleton);

            // Card Instantiation Service - GameObject creation
            builder.Register<ICardInstantiationService, CardInstantiationService>(Lifetime.Singleton);

            // Combat Service - combat operations
            builder.Register<CombatService>(Lifetime.Singleton);
            builder.Register<ICombatService>(c => c.Resolve<CombatService>(), Lifetime.Singleton);
            builder.Register<ICombatQueryService>(c => c.Resolve<CombatService>(), Lifetime.Singleton);

            // ============================================
            // USE CASES - Transient instances
            // ============================================
            builder.Register<SummonPlayerEntityUseCase>(Lifetime.Transient);
            builder.Register<ResetCardsForNewTurnUseCase>(Lifetime.Transient);
            builder.Register<HandleCardDeathUseCase>(Lifetime.Transient);
            builder.Register<HandleCardAddedToFieldUseCase>(Lifetime.Transient);
            builder.Register<HandleCardRemovedFromFieldUseCase>(Lifetime.Transient);
            builder.Register<HandleHandCardsChangeUseCase>(Lifetime.Transient);
            builder.Register<HandleFieldCardChangedUseCase>(Lifetime.Transient);

            // ============================================
            // ABILITY SYSTEM
            // ============================================
            builder.Register<IAbilityProvider, AbilityProviderService>(Lifetime.Singleton);
        }
    }
}
