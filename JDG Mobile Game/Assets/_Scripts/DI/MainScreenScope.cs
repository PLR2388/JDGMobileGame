using JDG.Infrastructure.Services;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace JDG.DI
{
    /// <summary>
    /// VContainer lifetime scope for MainScreen scene.
    /// Registers MonoBehaviours that exist only in the MainScreen scene.
    /// Phase 46: Split from LegacyServicesScope for proper scene-based DI.
    ///
    /// Place this on a GameObject in the MainScreen scene.
    /// Auto Run = true, Parent = None (explicitly finds SharedServicesScope via EnqueueParent).
    /// </summary>
    public class MainScreenScope : LifetimeScope
    {
        protected override void Awake()
        {
#if UNITY_EDITOR
            Debug.Log("MainScreenScope: Awake called, looking for SharedServicesScope...");
#endif

            // SharedServicesScope is in DontDestroyOnLoad, so auto-find doesn't work.
            // We must explicitly enqueue it as parent before base.Awake() builds the container.
            var sharedScope = FindFirstObjectByType<SharedServicesScope>();
            if (sharedScope == null)
            {
                // SharedServicesScope not found - this happens when playing MainScreen directly
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
                    Debug.LogWarning("MainScreenScope: SharedServicesScope NOT FOUND! Creating dynamically for Editor playback...");
                    sharedScope = CreateSharedServicesScope();
                }
            }

            if (sharedScope != null)
            {
#if UNITY_EDITOR
                Debug.Log($"MainScreenScope: Found SharedServicesScope, enqueueing as parent");
#endif
                EnqueueParent(sharedScope);
            }
            else
            {
                Debug.LogError("MainScreenScope: Failed to create SharedServicesScope! DI will not work.");
                return;
            }

            base.Awake();
#if UNITY_EDITOR
            Debug.Log($"MainScreenScope: After Awake, Parent = {(Parent != null ? Parent.GetType().Name : "NULL")}");
#endif
        }

        /// <summary>
        /// Creates SharedServicesScope dynamically when _preload scene wasn't loaded.
        /// This enables playing MainScreen scene directly from the Unity Editor.
        /// </summary>
        private SharedServicesScope CreateSharedServicesScope()
        {
#if UNITY_EDITOR
            Debug.Log("MainScreenScope: Creating SharedServicesScope dynamically...");
#endif

            var sharedScopeGO = new GameObject("SharedServicesScope (Dynamic)");
            var sharedScope = sharedScopeGO.AddComponent<SharedServicesScope>();

            // SharedServicesScope.Awake() is called immediately by Unity when AddComponent runs,
            // which sets up DontDestroyOnLoad and triggers VContainer's initialization.

#if UNITY_EDITOR
            Debug.Log("MainScreenScope: SharedServicesScope created and initialized");
#endif
            return sharedScope;
        }

        protected override void Configure(IContainerBuilder builder)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log("MainScreenScope: Configuring...");
            UnityEngine.Debug.Log($"MainScreenScope: Parent = {(Parent != null ? Parent.GetType().Name : "NULL")}");
#endif

            // ============================================
            // MAINSCREEN SCENE MONOBEHAVIOURS
            // ============================================

            // Note: InputManager is NOT in MainScreen scene (only in Game scene)
            // ICardSelectionService is registered in SharedServicesScope (pure C#, no MonoBehaviour needed)
            // CardSelectionManager IS in MainScreen for deck builder card selection functionality

            // Card Choice UI - deck selection screen
            builder.RegisterComponentInHierarchy<Menu.CardChoiceUIManager>();
            builder.RegisterComponentInHierarchy<Menu.CardChoice>();

            // SceneLoader - needs DI for IDeckManagementService, IAudioService, ILocalizationService
            // Critical: Without this injection, BuildTutorialDecks() is never called when clicking Tutorial button
            builder.RegisterComponentInHierarchy<SceneLoader>();

            // MainMenuAction - needs IAudioService injection for music playback
            // Note: Using RegisterComponentInHierarchy alone doesn't guarantee injection
            // We need explicit injection in RegisterBuildCallback (like GameSceneScope pattern)
            builder.RegisterComponentInHierarchy<MainMenuAction>();

            // Phase 84 Fix: Manual injection for scene MonoBehaviours (same pattern as GameSceneScope)
            builder.RegisterBuildCallback(container =>
            {
#if UNITY_EDITOR
                UnityEngine.Debug.Log("MainScreenScope: Injecting dependencies into scene MonoBehaviours...");
#endif
                // Phase 166: Wire DialogService with MessageBox/CardSelector from _preload scene
                // These components live on Systems/UI (DontDestroyOnLoad) and must be set
                // for any scene that uses IDialogService (e.g., CardChoice validation messages).
                var messageBox = FindFirstObjectByType<MessageBox>();
                var cardSelector = FindFirstObjectByType<CardSelector>();
                if (messageBox != null)
                {
                    var dialogService = container.Resolve<JDG.Application.Services.IDialogService>() as DialogService;
                    if (dialogService != null)
                    {
                        dialogService.SetDialogComponents(messageBox, cardSelector);
#if UNITY_EDITOR
                        UnityEngine.Debug.Log("MainScreenScope: Set dialog components on DialogService");
#endif
                    }
                }

                InjectAllOfType<MainMenuAction>(container);
                InjectAllOfType<SceneLoader>(container);

                // Phase 133: Deck builder and UI components with [Inject]
                InjectAllOfType<InfiniteScroll>(container);
                InjectAllOfType<UpdateDescription>(container);
                InjectAllOfType<CardSelectionManager>(container);
                InjectAllOfType<Menu.OptionMenu>(container);

                // Phase 144: Card choice components - explicit injection for consistency with GameSceneScope pattern
                InjectAllOfType<Menu.CardChoice>(container);
                InjectAllOfType<Menu.CardChoiceUIManager>(container);

#if UNITY_EDITOR
                UnityEngine.Debug.Log("MainScreenScope: Injection complete");
#endif
            });

#if UNITY_EDITOR
            UnityEngine.Debug.Log("MainScreenScope: Configuration complete");
#endif
        }

        /// <summary>
        /// Helper method to inject dependencies into all instances of a component type.
        /// Same pattern as GameSceneScope.InjectAllOfType.
        /// </summary>
        private void InjectAllOfType<T>(VContainer.IObjectResolver container) where T : Component
        {
            // Include inactive GameObjects to ensure components on disabled parents get injected
            var components = FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var c in components)
            {
                try
                {
                    container.Inject(c);
                }
                catch (System.Exception ex)
                {
                    UnityEngine.Debug.LogError($"MainScreenScope: Failed to inject {typeof(T).Name}: {ex.Message}");
                }
            }
            if (components.Length > 0)
            {
#if UNITY_EDITOR
                UnityEngine.Debug.Log($"MainScreenScope: Injected {components.Length} {typeof(T).Name}");
#endif
            }
            else
                UnityEngine.Debug.LogWarning($"MainScreenScope: No {typeof(T).Name} found in scene!");
        }
    }
}
