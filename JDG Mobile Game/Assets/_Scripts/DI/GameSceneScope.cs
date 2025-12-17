using UnityEngine;
using VContainer;
using VContainer.Unity;
using JDG.Application.Services;
using JDG.Infrastructure.Services;

namespace JDG.DI
{
    /// <summary>
    /// VContainer lifetime scope for Game scene.
    /// Registers MonoBehaviours that exist only in the Game scene.
    /// Phase 46: Split from LegacyServicesScope for proper scene-based DI.
    ///
    /// Place this on a GameObject in the Game scene.
    /// Auto Run = true, Parent = None (explicitly finds SharedServicesScope via EnqueueParent).
    /// </summary>
    public class GameSceneScope : LifetimeScope
    {
        protected override void Awake()
        {
            Debug.Log("GameSceneScope: Awake called, looking for SharedServicesScope...");

            // SharedServicesScope is in DontDestroyOnLoad, so auto-find doesn't work.
            // We must explicitly enqueue it as parent before base.Awake() builds the container.
            var sharedScope = FindObjectOfType<SharedServicesScope>();
            if (sharedScope != null)
            {
                Debug.Log($"GameSceneScope: Found SharedServicesScope, enqueueing as parent");
                EnqueueParent(sharedScope);
            }
            else
            {
                Debug.LogError("GameSceneScope: SharedServicesScope NOT FOUND! DI will fail.");
            }

            base.Awake();
            Debug.Log($"GameSceneScope: After Awake, Parent = {(Parent != null ? Parent.GetType().Name : "NULL")}");
        }

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log("GameSceneScope: Configuring...");
            Debug.Log($"GameSceneScope: Parent = {(Parent != null ? Parent.GetType().Name : "NULL")}");

            // ============================================
            // GAME SCENE MONOBEHAVIOURS
            // ============================================
            // Note: LegacyCardLoader is in _preload scene, registered in SharedServicesScope

            // Input Manager - scene-specific
            builder.RegisterComponentInHierarchy<InputManager>();
            builder.Register<IInputService, InputService>(Lifetime.Singleton);

            // Card Pool Manager - manages card object pooling
            builder.RegisterComponentInHierarchy<CardPoolManager>();
            builder.Register<ICardPoolService, CardPoolService>(Lifetime.Singleton);

            // Card Selection Manager - for in-game card selection UI
            // Note: ICardSelectionService is registered in SharedServicesScope (pure C#)
            // CardSelectionManager is the legacy UI MonoBehaviour
            builder.RegisterComponentInHierarchy<CardSelectionManager>();

            // Card Collection Service - bridges to CardManager
            builder.Register<ICardCollectionService, CardCollectionServiceAdapter>(Lifetime.Singleton);

            // Invocation Menu Manager - card action menu
            builder.RegisterComponentInHierarchy<InvocationMenuManager>();
            builder.Register<IInvocationMenuService, InvocationMenuService>(Lifetime.Singleton);

            // Round Display Manager - turn/phase display
            builder.RegisterComponentInHierarchy<RoundDisplayManager>();
            builder.Register<IRoundDisplayService, RoundDisplayService>(Lifetime.Singleton);

            // UI Manager - delegates to presenters (being phased out)
            builder.RegisterComponentInHierarchy<UIManager>();

            // Player Manager - player status provider
            builder.RegisterComponentInHierarchy<PlayerManager>().As<IPlayerStatusProvider>();
        }
    }
}
