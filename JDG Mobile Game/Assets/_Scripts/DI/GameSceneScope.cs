using VContainer;
using VContainer.Unity;
using JDG.Application.Services;
using JDG.Infrastructure.Services;
using JDG.Bridge;

namespace JDG.DI
{
    /// <summary>
    /// VContainer lifetime scope for Game scene.
    /// Registers MonoBehaviours that exist only in the Game scene.
    /// Phase 46: Split from LegacyServicesScope for proper scene-based DI.
    ///
    /// Place this on a GameObject in the Game scene.
    /// Set Parent to reference SharedServicesScope (or use Auto Run parent finding).
    /// </summary>
    public class GameSceneScope : LifetimeScope
    {
        protected override void Awake()
        {
            // VContainer's auto-parent-finding goes to the ROOT scope (GameLifetimeScope),
            // but we need SharedServicesScope which has the actual service registrations.
            // Explicitly find and set SharedServicesScope as our parent.
            if (Parent == null)
            {
                var sharedScope = FindObjectOfType<SharedServicesScope>();
                if (sharedScope != null)
                {
                    Parent = sharedScope;
                    UnityEngine.Debug.Log("GameSceneScope: Set parent to SharedServicesScope");
                }
                else
                {
                    UnityEngine.Debug.LogError(
                        "GameSceneScope: Could not find SharedServicesScope! Make sure:\n" +
                        "1. You started from the _preload scene (not Game directly)\n" +
                        "2. SharedServicesScope has 'Auto Run' checked in _preload scene");
                }
            }

            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            // ============================================
            // GAME SCENE MONOBEHAVIOURS
            // ============================================

            // Legacy Card Loader - loads card data
            builder.RegisterComponentInHierarchy<LegacyCardLoader>();

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
