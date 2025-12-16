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
    /// Set Parent to reference SharedServicesScope (or use Auto Run parent finding).
    /// </summary>
    public class MainScreenScope : LifetimeScope
    {
        protected override void Awake()
        {
            // Debug: Check if parent scope exists
            if (Parent == null)
            {
                UnityEngine.Debug.LogWarning(
                    "MainScreenScope: No parent scope found! Make sure:\n" +
                    "1. You started from the _preload scene (not MainScreen directly)\n" +
                    "2. SharedServicesScope has 'Auto Run' checked in _preload scene\n" +
                    "3. GameLifetimeScope has 'Auto Run' checked in _preload scene");
            }
            else
            {
                UnityEngine.Debug.Log($"MainScreenScope: Found parent scope: {Parent.GetType().Name}");
            }

            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            UnityEngine.Debug.Log("MainScreenScope: Configuring...");

            // ============================================
            // MAINSCREEN SCENE MONOBEHAVIOURS
            // ============================================

            // Note: InputManager and CardSelectionManager are NOT in MainScreen scene
            // ICardSelectionService is registered in SharedServicesScope (pure C#, no MonoBehaviour needed)

            // Card Choice UI - deck selection screen
            builder.RegisterComponentInHierarchy<Menu.CardChoiceUIManager>();
            builder.RegisterComponentInHierarchy<Menu.CardChoice>();

            UnityEngine.Debug.Log("MainScreenScope: Configuration complete");
        }
    }
}
