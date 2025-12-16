using System;
using VContainer.Unity;

namespace JDG.DI
{
    /// <summary>
    /// OBSOLETE: This scope has been split into scene-specific scopes.
    /// Use the following instead:
    /// - SharedServicesScope (preload scene) - Pure services without scene dependencies
    /// - MainScreenScope (MainScreen scene) - MainScreen MonoBehaviours
    /// - GameSceneScope (Game scene) - Game scene MonoBehaviours
    ///
    /// Phase 46: Split for proper scene-based DI architecture.
    /// </summary>
    [Obsolete("Use SharedServicesScope, MainScreenScope, or GameSceneScope instead. See class documentation.")]
    public class LegacyServicesScope : LifetimeScope
    {
        protected override void Configure(VContainer.IContainerBuilder builder)
        {
            UnityEngine.Debug.LogError(
                "LegacyServicesScope is obsolete! Replace with:\n" +
                "- SharedServicesScope (in preload scene)\n" +
                "- MainScreenScope (in MainScreen scene)\n" +
                "- GameSceneScope (in Game scene)");
        }
    }
}
