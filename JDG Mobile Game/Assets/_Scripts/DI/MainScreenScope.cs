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
    /// Auto Run = true, Parent = None (auto-finds SharedServicesScope).
    /// </summary>
    public class MainScreenScope : LifetimeScope
    {
        protected override void Awake()
        {
            UnityEngine.Debug.Log("MainScreenScope: Awake called, looking for parent scope...");
            base.Awake();
            UnityEngine.Debug.Log($"MainScreenScope: After Awake, Parent = {(Parent != null ? Parent.GetType().Name : "NULL")}");
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

            UnityEngine.Debug.Log("MainScreenScope: Configuration complete");
        }
    }
}
