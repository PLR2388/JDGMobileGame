using VContainer;
using VContainer.Unity;
using JDG.Application.Services;
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

            // Input Manager - scene-specific
            builder.RegisterComponentInHierarchy<InputManager>();
            builder.Register<IInputService, InputService>(Lifetime.Singleton);

            // Card Selection Manager - for deck building UI
            builder.RegisterComponentInHierarchy<CardSelectionManager>();
            builder.Register<JDG.Application.Services.ICardSelectionService, JDG.Infrastructure.Services.CardSelectionService>(Lifetime.Singleton);

            // Card Choice UI - deck selection screen
            builder.RegisterComponentInHierarchy<Menu.CardChoiceUIManager>();
            builder.RegisterComponentInHierarchy<Menu.CardChoice>();
        }
    }
}
