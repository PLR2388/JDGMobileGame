using _Scripts.Cards.InvocationCards;
using Cards;
using Cards.FieldCards;
using Cards.EffectCards;
using Cards.EquipmentCards;
using Cards.InvocationCards;
using JDG.Application;
using JDG.Application.Cards;
using JDG.Application.Services;
using JDG.Infrastructure.Cards;
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
#if UNITY_EDITOR
            Debug.Log("GameSceneScope: Awake called");
#endif

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
#if UNITY_EDITOR
                Debug.Log("GameSceneScope: Found SharedServicesScope, setting as parent");
#endif
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
#if UNITY_EDITOR
            Debug.Log("GameSceneScope: Creating SharedServicesScope dynamically...");
#endif

            var sharedScopeGO = new GameObject("SharedServicesScope (Dynamic)");
            var sharedScope = sharedScopeGO.AddComponent<SharedServicesScope>();

            // SharedServicesScope.Awake() is called immediately by Unity when AddComponent runs,
            // which sets up DontDestroyOnLoad and triggers VContainer's initialization.

#if UNITY_EDITOR
            Debug.Log("GameSceneScope: SharedServicesScope created and initialized");
#endif
            return sharedScope;
        }

        protected override void Configure(IContainerBuilder builder)
        {
#if UNITY_EDITOR
            Debug.Log("GameSceneScope: Configure called");
#endif

            // Phase 155: Validate all required scene components before registration
            ValidateRequiredSceneComponents();

            // ============================================
            // SCENE MONOBEHAVIOURS (must be registered first for services that depend on them)
            // ============================================

            // CardPoolManager - required by CardInstantiationService
            var cardPoolManager = FindFirstObjectByType<CardPoolManager>();
            if (cardPoolManager != null)
            {
                builder.RegisterInstance(cardPoolManager);
#if UNITY_EDITOR
                Debug.Log("GameSceneScope: Registered CardPoolManager instance");
#endif
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
#if UNITY_EDITOR
                Debug.Log("GameSceneScope: Registered InvocationMenuManager instance");
#endif
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
#if UNITY_EDITOR
                Debug.Log("GameSceneScope: Registered RoundDisplayManager instance");
#endif
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
#if UNITY_EDITOR
                Debug.Log("GameSceneScope: Registered InputManager instance");
#endif
            }
            else
            {
                Debug.LogError("GameSceneScope: InputManager NOT FOUND in scene!");
            }

            // Phase 127: UIManager removed - GameLoop now uses presenters directly

            // Phase 94: MessageBox - required by DialogService
            // Phase 156: Throw exception if MessageBox is missing (required for dialogs)
            var messageBox = FindFirstObjectByType<MessageBox>();
            if (messageBox == null)
            {
                throw new System.InvalidOperationException(
                    "GameSceneScope: MessageBox NOT FOUND in scene! " +
                    "This component is required for DialogService to show dialogs. " +
                    "Ensure MessageBox exists in the Game scene.");
            }
            builder.RegisterInstance(messageBox);
#if UNITY_EDITOR
            Debug.Log("GameSceneScope: Registered MessageBox instance");
#endif

            // Phase 94: CardSelector - required by DialogService
            // Phase 156: Throw exception if CardSelector is missing (required for card selection)
            var cardSelector = FindFirstObjectByType<CardSelector>();
            if (cardSelector == null)
            {
                throw new System.InvalidOperationException(
                    "GameSceneScope: CardSelector NOT FOUND in scene! " +
                    "This component is required for DialogService to show card selections. " +
                    "Ensure CardSelector exists in the Game scene.");
            }
            builder.RegisterInstance(cardSelector);
#if UNITY_EDITOR
            Debug.Log("GameSceneScope: Registered CardSelector instance");
#endif

            // ============================================
            // SCENE-SPECIFIC SERVICES
            // These depend on MonoBehaviours that only exist in this scene
            // ============================================

            // Phase 86: ICardPoolService - wraps CardPoolManager for DI
            builder.Register<ICardPoolService, CardPoolService>(Lifetime.Scoped);

            // Phase 90: ITutorialStateService - replaces DialogueTutoHandler singleton
            builder.Register<ITutorialStateService, TutorialStateService>(Lifetime.Scoped);

            // ICardCollectionService + ICardCollectionProvider - requires PlayerCardManager from scene
            // Phase 166: CardCollectionServiceAdapter now implements both interfaces
            builder.Register<CardCollectionServiceAdapter>(Lifetime.Scoped);
            builder.Register<ICardCollectionService>(container => container.Resolve<CardCollectionServiceAdapter>(), Lifetime.Scoped);
            builder.Register<ICardCollectionProvider>(container => container.Resolve<CardCollectionServiceAdapter>(), Lifetime.Scoped);

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
                    container.Resolve<ICardCollectionProvider>(),
                    container.Resolve<IAbilityProvider>(),
                    container.Resolve<IFieldAbilityProvider>(),
                    container.Resolve<IEquipmentAbilityProvider>(),
                    container.Resolve<IEffectAbilityProvider>(),
                    container.Resolve<IConditionProvider>()
                );
            }, Lifetime.Scoped);

            // IPlayerStatusProvider - requires PlayerManager from scene
            // Phase 156: Throw exception if PlayerManager is missing (required for gameplay)
            var playerManager = FindFirstObjectByType<PlayerManager>();
            if (playerManager == null)
            {
                throw new System.InvalidOperationException(
                    "GameSceneScope: PlayerManager NOT FOUND in scene! " +
                    "This component is required for ICardPlacementService, CombatService, and ITurnService. " +
                    "Ensure PlayerManager exists in the Game scene.");
            }
            builder.RegisterInstance<IPlayerStatusProvider>(playerManager);
#if UNITY_EDITOR
            Debug.Log("GameSceneScope: Registered PlayerManager as IPlayerStatusProvider");
#endif

            // Canvas Transform for CombatService
            // Phase 156: Throw exception if Canvas is missing (required for UI)
            var canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                throw new System.InvalidOperationException(
                    "GameSceneScope: Canvas NOT FOUND in scene! " +
                    "This component is required for CombatService and CanvasProviderService. " +
                    "Ensure a Canvas exists in the Game scene.");
            }
            builder.RegisterInstance(canvas.transform).As<Transform>();
#if UNITY_EDITOR
            Debug.Log("GameSceneScope: Registered Canvas Transform");
#endif

            // Phase 84 Fix: Set canvas on the singleton CanvasProviderService from parent scope
            // (ICanvasProvider is now registered in SharedServicesScope, we just set the canvas here)
            // Phase 156: Removed null checks - canvas, messageBox, cardSelector now guaranteed non-null
            builder.RegisterBuildCallback(container =>
            {
                var canvasProvider = container.Resolve<CanvasProviderService>();
                canvasProvider.SetCanvas(canvas.transform);
#if UNITY_EDITOR
                Debug.Log("GameSceneScope: Set canvas on CanvasProviderService from parent scope");
#endif

                // Phase 94: Set MessageBox and CardSelector on DialogService from parent scope
                // Phase 156: Simplified since messageBox/cardSelector guaranteed non-null
                var dialogService = container.Resolve<IDialogService>() as DialogService;
                if (dialogService == null)
                {
                    throw new System.InvalidOperationException(
                        "GameSceneScope: Failed to resolve IDialogService as DialogService. " +
                        "Ensure DialogService is properly registered in SharedServicesScope.");
                }
                dialogService.SetDialogComponents(messageBox, cardSelector);
#if UNITY_EDITOR
                Debug.Log("GameSceneScope: Set dialog components on DialogService from parent scope");
#endif
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

            // ITurnService - depends on GameStateService, PlayerCardManagers, IPlayerStatusProvider, ICardSyncService, Transform
            // Phase 151: Added ICardSyncService for SourceCard conversion in ability contexts
            // Phase 156: Note - lambdas capture MonoBehaviour references. This is safe because:
            // 1. GameSceneScope (LifetimeScope) is destroyed on scene unload
            // 2. VContainer disposes the container, releasing these lambdas
            // 3. Services are Scoped lifetime, destroyed with the container
            builder.Register<ITurnService>(container =>
            {
                return new TurnService(
                    container.Resolve<GameStateService>(),
                    player1CardManager,
                    player2CardManager,
                    container.Resolve<IPlayerStatusProvider>(),
                    container.Resolve<ICardSyncService>(),
                    container.Resolve<Transform>()
                );
            }, Lifetime.Scoped);

            // ICardDrawService - depends on GameStateService, PlayerCardManagers
            // Phase 156: Same safety note as ITurnService above
            builder.Register<ICardDrawService>(container =>
            {
                return new CardDrawService(
                    container.Resolve<GameStateService>(),
                    player1CardManager,
                    player2CardManager
                );
            }, Lifetime.Scoped);

#if UNITY_EDITOR
            Debug.Log("GameSceneScope: Registered ITurnService and ICardDrawService with PlayerCardManagers");
            Debug.Log("GameSceneScope: Scene-specific services registered");
#endif

            // ============================================
            // MANUAL INJECTION for scene MonoBehaviours
            // ============================================
            builder.RegisterBuildCallback(container =>
            {
#if UNITY_EDITOR
                Debug.Log("GameSceneScope: Injecting dependencies into scene MonoBehaviours...");
#endif

                // ============================================
                // CRITICAL ORDER DEPENDENCY - DO NOT REORDER
                // ============================================
                // TutoSceneInitializer MUST be injected FIRST (before PlayerCards)
                // to ensure tutorial decks are built when loading TutoPlayerGame directly from Editor.
                // Without this, PlayerCards.Construct() gets empty decks and GameOver() triggers.
                //
                // Phase 156: Added explicit check for tutorial scene
                var tutoInitializers = FindObjectsByType<TutoSceneInitializer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                if (tutoInitializers.Length > 0)
                {
#if UNITY_EDITOR
                    Debug.Log($"GameSceneScope: Tutorial scene detected ({tutoInitializers.Length} TutoSceneInitializer) - injecting first");
#endif
                    InjectAllOfType<TutoSceneInitializer>(container);
                }

                // Inject PlayerCards AFTER TutoSceneInitializer (order is critical for tutorials)
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

                // Phase 155: Log summary of any injection failures
                LogInjectionSummary();

#if UNITY_EDITOR
                Debug.Log("GameSceneScope: Injection complete");
#endif
            });
        }

        // Phase 155: Track injection failures for summary logging
        private int _injectionFailureCount = 0;
        private readonly System.Collections.Generic.List<string> _failedInjections = new System.Collections.Generic.List<string>();

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
                    _injectionFailureCount++;
                    _failedInjections.Add($"{typeof(T).Name}: {ex.Message}");
                    Debug.LogError($"GameSceneScope: Failed to inject {typeof(T).Name}: {ex.Message}");
                }
            }
            if (components.Length > 0)
            {
#if UNITY_EDITOR
                Debug.Log($"GameSceneScope: Injected {components.Length} {typeof(T).Name}");
#endif
            }
        }

        /// <summary>
        /// Phase 155: Logs a summary of any injection failures.
        /// Call at the end of RegisterBuildCallback.
        /// </summary>
        private void LogInjectionSummary()
        {
            if (_injectionFailureCount > 0)
            {
                Debug.LogError($"GameSceneScope: INJECTION FAILURES - {_injectionFailureCount} components failed to inject!\n" +
                    "Failed components:\n  - " + string.Join("\n  - ", _failedInjections) +
                    "\n\nSome game features may not work correctly.");
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("GameSceneScope: All component injections successful");
#endif
            }
        }

        /// <summary>
        /// Phase 155: Validates that all required scene components exist before DI registration.
        /// Fails fast with a clear error message if any critical components are missing.
        /// </summary>
        private void ValidateRequiredSceneComponents()
        {
            var missingComponents = new System.Collections.Generic.List<string>();

            // Critical MonoBehaviours that services depend on
            if (FindFirstObjectByType<CardPoolManager>() == null)
                missingComponents.Add("CardPoolManager (required by ICardPoolService)");

            if (FindFirstObjectByType<InvocationMenuManager>() == null)
                missingComponents.Add("InvocationMenuManager (required by IInvocationMenuService)");

            if (FindFirstObjectByType<RoundDisplayManager>() == null)
                missingComponents.Add("RoundDisplayManager (required by IRoundDisplayService)");

            if (FindFirstObjectByType<InputManager>() == null)
                missingComponents.Add("InputManager (required by IInputService)");

            if (FindFirstObjectByType<MessageBox>() == null)
                missingComponents.Add("MessageBox (required by IDialogService)");

            if (FindFirstObjectByType<CardSelector>() == null)
                missingComponents.Add("CardSelector (required by IDialogService)");

            if (FindFirstObjectByType<PlayerManager>() == null)
                missingComponents.Add("PlayerManager (required by IPlayerStatusProvider)");

            // Check for PlayerCardManagers (need exactly 2)
            // Phase 156: Fixed - was checking PlayerCards instead of PlayerCardManager
            // PlayerCardManager is what ITurnService and ICardDrawService depend on (see line 288)
            var playerCardManagers = FindObjectsByType<PlayerCardManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (playerCardManagers.Length < 2)
                missingComponents.Add($"PlayerCardManager (found {playerCardManagers.Length}, need 2 for ITurnService/ICardDrawService)");

            if (missingComponents.Count > 0)
            {
                var errorMessage = "GameSceneScope: CRITICAL - Missing required scene components!\n" +
                                   "The following components must exist in the game scene:\n  - " +
                                   string.Join("\n  - ", missingComponents) +
                                   "\n\nPlease verify that the scene is properly configured with all required prefabs.";

                Debug.LogError(errorMessage);

                // Phase 157: Always throw exception for missing critical components.
                // Previously only threw in Editor, which allowed builds to start with missing
                // components and crash later with confusing null reference errors.
                throw new System.InvalidOperationException(errorMessage);
            }
        }

        /// <summary>
        /// Phase 144: Clear canvas reference on scene unload to prevent stale references.
        /// </summary>
        protected override void OnDestroy()
        {
#if UNITY_EDITOR
            Debug.Log("GameSceneScope: OnDestroy called");
#endif

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
