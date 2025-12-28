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
            Debug.Log("MainScreenScope: Awake called, looking for SharedServicesScope...");

            // SharedServicesScope is in DontDestroyOnLoad, so auto-find doesn't work.
            // We must explicitly enqueue it as parent before base.Awake() builds the container.
            var sharedScope = FindFirstObjectByType<SharedServicesScope>();
            if (sharedScope == null)
            {
                // SharedServicesScope not found - this happens when playing MainScreen directly
                // from Unity Editor without going through _preload scene.
                // Create SharedServicesScope dynamically to enable DI.
                Debug.LogWarning("MainScreenScope: SharedServicesScope NOT FOUND! Creating dynamically for Editor playback...");
                sharedScope = CreateSharedServicesScope();
            }

            if (sharedScope != null)
            {
                Debug.Log($"MainScreenScope: Found SharedServicesScope, enqueueing as parent");
                EnqueueParent(sharedScope);
            }
            else
            {
                Debug.LogError("MainScreenScope: Failed to create SharedServicesScope! DI will not work.");
                return;
            }

            base.Awake();
            Debug.Log($"MainScreenScope: After Awake, Parent = {(Parent != null ? Parent.GetType().Name : "NULL")}");
        }

        /// <summary>
        /// Creates SharedServicesScope dynamically when _preload scene wasn't loaded.
        /// This enables playing MainScreen scene directly from the Unity Editor.
        /// </summary>
        private SharedServicesScope CreateSharedServicesScope()
        {
            Debug.Log("MainScreenScope: Creating SharedServicesScope dynamically...");

            var sharedScopeGO = new GameObject("SharedServicesScope (Dynamic)");
            var sharedScope = sharedScopeGO.AddComponent<SharedServicesScope>();

            // SharedServicesScope.Awake() is called immediately by Unity when AddComponent runs,
            // which sets up DontDestroyOnLoad and triggers VContainer's initialization.

            Debug.Log("MainScreenScope: SharedServicesScope created and initialized");
            return sharedScope;
        }

        protected override void Configure(IContainerBuilder builder)
        {
            UnityEngine.Debug.Log("MainScreenScope: Configuring...");
            UnityEngine.Debug.Log($"MainScreenScope: Parent = {(Parent != null ? Parent.GetType().Name : "NULL")}");

            // ============================================
            // MAINSCREEN SCENE MONOBEHAVIOURS
            // ============================================

            // Note: InputManager and CardSelectionManager are NOT in MainScreen scene
            // ICardSelectionService is registered in SharedServicesScope (pure C#, no MonoBehaviour needed)

            // Card Choice UI - deck selection screen
            builder.RegisterComponentInHierarchy<Menu.CardChoiceUIManager>();
            builder.RegisterComponentInHierarchy<Menu.CardChoice>();

            // SceneLoader - needs DI for IDeckManagementService, IAudioService, ILocalizationService
            // Critical: Without this injection, BuildTutorialDecks() is never called when clicking Tutorial button
            builder.RegisterComponentInHierarchy<SceneLoader>();

            UnityEngine.Debug.Log("MainScreenScope: Configuration complete");
        }
    }
}
