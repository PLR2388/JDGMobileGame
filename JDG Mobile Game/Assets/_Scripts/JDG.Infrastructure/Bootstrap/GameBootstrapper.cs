using UnityEngine;
using JDG.Infrastructure.DI;

namespace JDG.Infrastructure.Bootstrap
{
    /// <summary>
    /// Game bootstrapper - initializes the DI container.
    /// Attach this to a GameObject in your startup scene.
    /// Note: Card loading and legacy initialization handled by LegacySystemInitializer
    /// (called from SharedServicesScope.RegisterBuildCallback).
    /// Phase 41: Removed ServiceLocator - all code now uses VContainer DI directly.
    /// Phase 67: Removed reference to obsolete LegacyCardLoader (deleted).
    /// </summary>
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private GameLifetimeScope _lifetimeScope;

        private void Awake()
        {
            // Ensure this GameObject persists across scenes
            DontDestroyOnLoad(gameObject);

            if (_lifetimeScope != null)
            {
                Debug.Log("GameBootstrapper: VContainer DI initialized");
            }
            else
            {
                Debug.LogError("GameBootstrapper: GameLifetimeScope not assigned!");
            }
        }
    }
}
