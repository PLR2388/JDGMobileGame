using _Scripts.Cards.InvocationCards;
using Cards;
using Cards.FieldCards;
using Cards.EffectCards;
using Cards.EquipmentCards;
using Cards.InvocationCards;
using JDG.Application;
using JDG.Application.Cards;
using JDG.Application.Services;
using JDG.Infrastructure.Services;
using OnePlayer;
using OnePlayer.DialogueBox;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace JDG.DI
{
    /// <summary>
    /// VContainer lifetime scope for Game scene.
    /// Registers scene-specific services and handles MonoBehaviour injection.
    /// </summary>
    public class GameSceneScope : LifetimeScope
    {
        protected override void Awake()
        {
            Debug.Log("GameSceneScope: Awake called");

            // Find SharedServicesScope in DontDestroyOnLoad
            var sharedScope = FindFirstObjectByType<SharedServicesScope>();
            if (sharedScope == null)
            {
                // SharedServicesScope not found - this happens when playing a scene directly
                // from Unity Editor without going through _preload scene.
                // Phase 148: Check static instance tracker before creating dynamically
                if (SharedServicesScope.InstanceExists)
                {
                    // Instance exists but not found - might be in a different Unity scene state
                    sharedScope = FindFirstObjectByType<SharedServicesScope>();
                }
                if (sharedScope == null)
                {
                    // Create SharedServicesScope dynamically to enable DI.
                    Debug.LogWarning("GameSceneScope: SharedServicesScope NOT FOUND! Creating dynamically for Editor playback...");
                    sharedScope = CreateSharedServicesScope();
                }
            }

            if (sharedScope != null)
            {
                Debug.Log("GameSceneScope: Found SharedServicesScope, setting as parent");
                EnqueueParent(sharedScope);
            }
            else
            {
                Debug.LogError("GameSceneScope: Failed to create SharedServicesScope! DI will not work.");
                return;
            }

            base.Awake();
        }

        /// <summary>
        /// Creates SharedServicesScope dynamically when _preload scene wasn't loaded.
        /// This enables playing Game/TutoPlayerGame scenes directly from the Unity Editor.
        /// </summary>
        private SharedServicesScope CreateSharedServicesScope()
        {
            Debug.Log("GameSceneScope: Creating SharedServicesScope dynamically...");

            var sharedScopeGO = new GameObject("SharedServicesScope (Dynamic)");
            var sharedScope = sharedScopeGO.AddComponent<SharedServicesScope>();

            // SharedServicesScope.Awake() is called immediately by Unity when AddComponent runs,
            // which sets up DontDestroyOnLoad and triggers VContainer's initialization.

            Debug.Log("GameSceneScope: SharedServicesScope created and initialized");
            return sharedScope;
        }

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log("GameSceneScope: Configure called");

            // ============================================
            // SCENE MONOBEHAVIOURS (must be registered first for services that depend on them)
            // ============================================

            // CardPoolManager - required by CardInstantiationService
            var cardPoolManager = FindFirstObjectByType<CardPoolManager>();
            if (cardPoolManager != null)
            {
                builder.RegisterInstance(cardPoolManager);
                Debug.Log("GameSceneScope: Registered CardPoolManager instance");
            }
            else
            {
                Debug.LogError("GameSceneScope: CardPoolManager NOT FOUND in scene!");
            }

            // InvocationMenuManager - required by InvocationMenuService
            var invocationMenuManager = FindFirstObjectByType<InvocationMenuManager>();
            if (invocationMenuManager != null)
            {
                builder.RegisterInstance(invocationMenuManager);
                Debug.Log("GameSceneScope: Registered InvocationMenuManager instance");
            }
            else
            {
                Debug.LogError("GameSceneScope: InvocationMenuManager NOT FOUND in scene! IInvocationMenuService will not work.");
            }

            // RoundDisplayManager - required by RoundDisplayService
            var roundDisplayManager = FindFirstObjectByType<RoundDisplayManager>();
            if (roundDisplayManager != null)
            {
                builder.RegisterInstance(roundDisplayManager);
                Debug.Log("GameSceneScope: Registered RoundDisplayManager instance");
            }
            else
            {
                Debug.LogError("GameSceneScope: RoundDisplayManager NOT FOUND in scene! IRoundDisplayService will not work.");
            }

            // InputManager - required by InputService
            var inputManager = FindFirstObjectByType<InputManager>();
            if (inputManager != null)
            {
                builder.RegisterInstance(inputManager);
                Debug.Log("GameSceneScope: Registered InputManager instance");
            }
            else
            {
                Debug.LogError("GameSceneScope: InputManager NOT FOUND in scene!");
            }

            // Phase 127: UIManager removed - GameLoop now uses presenters directly

            // Phase 94: MessageBox - required by DialogService
            // Phase 135: Changed to LogError since DialogService will fail without MessageBox
            var messageBox = FindFirstObjectByType<MessageBox>();
            if (messageBox != null)
            {
                builder.RegisterInstance(messageBox);
                Debug.Log("GameSceneScope: Registered MessageBox instance");
            }
            else
            {
                Debug.LogError("GameSceneScope: MessageBox NOT FOUND in scene! DialogService dialogs will not work.");
            }

            // Phase 94: CardSelector - required by DialogService
            // Phase 135: Changed to LogError since DialogService will fail without CardSelector
            var cardSelector = FindFirstObjectByType<CardSelector>();
            if (cardSelector != null)
            {
                builder.RegisterInstance(cardSelector);
                Debug.Log("GameSceneScope: Registered CardSelector instance");
            }
            else
            {
                Debug.LogError("GameSceneScope: CardSelector NOT FOUND in scene! Card selection dialogs will not work.");
            }

            // ============================================
            // SCENE-SPECIFIC SERVICES
            // These depend on MonoBehaviours that only exist in this scene
            // ============================================

            // Phase 86: ICardPoolService - wraps CardPoolManager for DI
            builder.Register<ICardPoolService, CardPoolService>(Lifetime.Scoped);

            // Phase 90: ITutorialStateService - replaces DialogueTutoHandler singleton
            builder.Register<ITutorialStateService, TutorialStateService>(Lifetime.Scoped);

            // ICardCollectionService - requires PlayerCardManager from scene
            builder.Register<ICardCollectionService, CardCollectionServiceAdapter>(Lifetime.Scoped);

            // Phase 144: Override ICardFactory from parent (SharedServicesScope) with scene-scoped version.
            // IMPORTANT: SharedServicesScope also registers ICardFactory but with null ICardCollectionService.
            // This scene-scoped registration OVERRIDES the parent scope registration, providing access to
            // ICardCollectionService (which requires PlayerCardManager from the scene).
            // VContainer scope resolution: child scope registrations take precedence over parent scope.
            // Phase 146: Added documentation explaining the intentional override pattern.
            builder.Register<ICardFactory>(container =>
            {
                return new CardFactory(
                    container.Resolve<IEventBus>(),
                    container.Resolve<ICardCollectionService>(),
                    container.Resolve<IAbilityProvider>(),
                    container.Resolve<IFieldAbilityProvider>(),
                    container.Resolve<IEquipmentAbilityProvider>(),
                    container.Resolve<IEffectAbilityProvider>(),
                    container.Resolve<IConditionProvider>()
                );
            }, Lifetime.Scoped);

            // IPlayerStatusProvider - requires PlayerManager from scene
            var playerManager = FindFirstObjectByType<PlayerManager>();
            if (playerManager != null)
            {
                builder.RegisterInstance<IPlayerStatusProvider>(playerManager);
                Debug.Log("GameSceneScope: Registered PlayerManager as IPlayerStatusProvider");
            }
            else
            {
                Debug.LogError("GameSceneScope: PlayerManager NOT FOUND in scene! " +
                    "ICardPlacementService, CombatService, and ITurnService will fail to resolve.");
            }

            // Canvas Transform for CombatService
            var canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                builder.RegisterInstance(canvas.transform).As<Transform>();
                Debug.Log("GameSceneScope: Registered Canvas Transform");
            }
            else
            {
                Debug.LogError("GameSceneScope: Canvas NOT FOUND in scene! " +
                    "CombatService and CanvasProviderService will not work correctly.");
            }

            // Phase 84 Fix: Set canvas on the singleton CanvasProviderService from parent scope
            // (ICanvasProvider is now registered in SharedServicesScope, we just set the canvas here)
            builder.RegisterBuildCallback(container =>
            {
                if (canvas != null)
                {
                    var canvasProvider = container.Resolve<CanvasProviderService>();
                    canvasProvider.SetCanvas(canvas.transform);
                    Debug.Log("GameSceneScope: Set canvas on CanvasProviderService from parent scope");
                }

                // Phase 94: Set MessageBox and CardSelector on DialogService from parent scope
                // Phase 147: Added null check for DialogService cast failure
                var dialogService = container.Resolve<IDialogService>() as DialogService;
                if (dialogService == null)
                {
                    Debug.LogError("GameSceneScope: Failed to resolve IDialogService as DialogService");
                }
                else if (messageBox != null && cardSelector != null)
                {
                    dialogService.SetDialogComponents(messageBox, cardSelector);
                    Debug.Log("GameSceneScope: Set dialog components on DialogService from parent scope");
                }
                else
                {
                    // Phase 145: Log which component is missing for easier debugging
                    if (messageBox == null)
                        Debug.LogError("GameSceneScope: Cannot set DialogService components - MessageBox is null");
                    if (cardSelector == null)
                        Debug.LogError("GameSceneScope: Cannot set DialogService components - CardSelector is null");
                }
            });

            // ICardPlacementService - depends on ICardCollectionService, IPlayerStatusProvider
            builder.Register<ICardPlacementService, CardPlacementService>(Lifetime.Scoped);

            // CombatService - depends on ICardCollectionService, IPlayerStatusProvider, Transform
            builder.Register<CombatService>(Lifetime.Scoped);
            builder.Register<ICombatService>(c => c.Resolve<CombatService>(), Lifetime.Scoped);
            builder.Register<ICombatQueryService>(c => c.Resolve<CombatService>(), Lifetime.Scoped);

            // ICardInstantiationService - depends on CardPoolManager (registered above)
            builder.Register<ICardInstantiationService, CardInstantiationService>(Lifetime.Scoped);

            // IDeckInitializationService - depends on IDeckManagementService (parent), ICardInstantiationService
            builder.Register<IDeckInitializationService, DeckInitializationService>(Lifetime.Scoped);

            // IInvocationMenuService - for UI menu
            builder.Register<IInvocationMenuService, InvocationMenuService>(Lifetime.Scoped);

            // IRoundDisplayService
            builder.Register<IRoundDisplayService, RoundDisplayService>(Lifetime.Scoped);

            // IInputService - depends on InputManager (registered above)
            builder.Register<IInputService, InputService>(Lifetime.Scoped);

            // IRaycastService - depends on IInputService
            builder.Register<IRaycastService, RaycastService>(Lifetime.Scoped);

            // Services that need PlayerCardManagers (player1 and player2)
            // Note: Using InstanceID sort to maintain backwards-compatible order
            var playerCardManagers = FindObjectsByType<PlayerCardManager>(FindObjectsSortMode.InstanceID);
            if (playerCardManagers.Length < 2)
            {
                var errorMessage = $"GameSceneScope: Expected 2 PlayerCardManagers, found {playerCardManagers.Length}. " +
                    "Ensure both Player1 and Player2 GameObjects have PlayerCardManager components in the Game scene.";
                Debug.LogError(errorMessage);
                throw new System.InvalidOperationException(errorMessage);
            }

            // PlayerCardManagers are ordered by scene hierarchy - player 1 first
            var player1CardManager = playerCardManagers[0];
            var player2CardManager = playerCardManagers[1];

            // ITurnService - depends on GameStateService, PlayerCardManagers, IPlayerStatusProvider, Transform
            builder.Register<ITurnService>(container =>
            {
                return new TurnService(
                    container.Resolve<GameStateService>(),
                    player1CardManager,
                    player2CardManager,
                    container.Resolve<IPlayerStatusProvider>(),
                    container.Resolve<Transform>()
                );
            }, Lifetime.Scoped);

            // ICardDrawService - depends on GameStateService, PlayerCardManagers
            builder.Register<ICardDrawService>(container =>
            {
                return new CardDrawService(
                    container.Resolve<GameStateService>(),
                    player1CardManager,
                    player2CardManager
                );
            }, Lifetime.Scoped);

            Debug.Log("GameSceneScope: Registered ITurnService and ICardDrawService with PlayerCardManagers");

            Debug.Log("GameSceneScope: Scene-specific services registered");

            // ============================================
            // MANUAL INJECTION for scene MonoBehaviours
            // ============================================
            builder.RegisterBuildCallback(container =>
            {
                Debug.Log("GameSceneScope: Injecting dependencies into scene MonoBehaviours...");

                // IMPORTANT: TutoSceneInitializer must be injected FIRST (before PlayerCards)
                // to ensure tutorial decks are built when loading TutoPlayerGame directly from Editor.
                // Without this, PlayerCards.Construct() gets empty decks and GameOver() triggers.
                InjectAllOfType<TutoSceneInitializer>(container);

                InjectAllOfType<PlayerCards>(container);
                InjectAllOfType<PlayerStatus>(container);
                InjectAllOfType<CardLocation>(container);
                InjectAllOfType<GameLoop>(container);
                InjectAllOfType<PlayerManager>(container);
                InjectAllOfType<HealthUI>(container);
                InjectAllOfType<RoundDisplayManager>(container);
                InjectAllOfType<CardPoolManager>(container);
                InjectAllOfType<InputManager>(container);
                // Phase 127: UIManager removed - no longer needed
                InjectAllOfType<InvocationMenuManager>(container);
                InjectAllOfType<InvocationFunctions>(container);
                InjectAllOfType<FieldFunctions>(container);
                InjectAllOfType<EffectFunctions>(container);
                InjectAllOfType<EquipmentFunctions>(container);
                InjectAllOfType<InGameMenuScript>(container);
                InjectAllOfType<HandCardDisplay>(container);
                InjectAllOfType<OnHover>(container);
                InjectAllOfType<MessageBox>(container);
                InjectAllOfType<CardSelector>(container);
                InjectAllOfType<DisplayCards>(container);
                InjectAllOfType<CardDisplay>(container);
                InjectAllOfType<UpdateDescription>(container);
                InjectAllOfType<TutoInvocationFunctions>(container);

                // Phase 133: Tutorial UI components with [Inject]
                InjectAllOfType<TutoHandCardDisplay>(container);
                InjectAllOfType<DialogueUI>(container);
                InjectAllOfType<VideoPlayerObserver>(container);
                InjectAllOfType<HighLightCard>(container);
                InjectAllOfType<HighLightButton>(container);
                InjectAllOfType<HighLightPlane>(container);
                InjectAllOfType<HighLightPhysicalCard>(container);
                InjectAllOfType<HightLightText>(container);
                InjectAllOfType<TutoInGameMenuScript>(container);

                Debug.Log("GameSceneScope: Injection complete");
            });
        }

        private void InjectAllOfType<T>(IObjectResolver container) where T : Component
        {
            // Include inactive GameObjects to ensure components on disabled parents get injected
            // This is critical for HandCardDisplay which is on an inactive handScreen parent
            var components = FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var c in components)
            {
                try
                {
                    container.Inject(c);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"GameSceneScope: Failed to inject {typeof(T).Name}: {ex.Message}");
                }
            }
            if (components.Length > 0)
                Debug.Log($"GameSceneScope: Injected {components.Length} {typeof(T).Name}");
        }

        /// <summary>
        /// Phase 144: Clear canvas reference on scene unload to prevent stale references.
        /// </summary>
        protected override void OnDestroy()
        {
            Debug.Log("GameSceneScope: OnDestroy called");

            // Clear canvas reference from singleton service
            try
            {
                var canvasProvider = Container?.Resolve<CanvasProviderService>();
                canvasProvider?.ClearCanvas();
            }
            catch (System.Exception)
            {
                // Container might already be disposed, which is fine
            }

            base.OnDestroy();
        }
    }
}
