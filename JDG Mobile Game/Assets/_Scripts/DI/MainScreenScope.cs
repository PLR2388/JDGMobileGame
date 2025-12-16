using VContainer;
using VContainer.Unity;
using JDG.Infrastructure.Services;

namespace JDG.DI
{
    /// <summary>
    /// VContainer lifetime scope for MainScreen scene.
    /// Registers MonoBehaviours that exist only in the MainScreen scene.
    /// Phase 46: Split from LegacyServicesScope for proper scene-based DI.
    ///
    /// Place this on a GameObject in the MainScreen scene.
    /// Set Parent to reference SharedServicesScope (or use Auto Run parent finding).
    /// </summary>
    public class MainScreenScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // ============================================
            // MAINSCREEN SCENE MONOBEHAVIOURS
            // ============================================

            // Note: InputManager is NOT in MainScreen scene - only in Game scene
            // If needed later, add InputManager GameObject to MainScreen scene

            // Card Selection Manager - for deck building UI
            // Note: Only register if CardSelectionManager exists in this scene
            // If not present, comment out these lines
            builder.RegisterComponentInHierarchy<CardSelectionManager>();
            builder.Register<JDG.Application.Services.ICardSelectionService, JDG.Infrastructure.Services.CardSelectionService>(Lifetime.Singleton);

            // Card Choice UI - deck selection screen
            builder.RegisterComponentInHierarchy<Menu.CardChoiceUIManager>();
            builder.RegisterComponentInHierarchy<Menu.CardChoice>();
        }
    }
}
