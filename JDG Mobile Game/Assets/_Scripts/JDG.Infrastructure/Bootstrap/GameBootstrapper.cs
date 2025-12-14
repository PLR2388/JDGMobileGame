using UnityEngine;
using JDG.Infrastructure.DI;

namespace JDG.Infrastructure.Bootstrap
{
    /// <summary>
    /// Game bootstrapper - initializes the DI container.
    /// Attach this to a GameObject in your startup scene.
    /// Note: Card loading is handled by LegacyCardLoader (in default assembly).
    /// Phase 27: Ability.GameStateService initialization moved to LegacyCardLoader.
    /// Phase 41: Removed ServiceLocator - all code now uses VContainer DI directly.
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
