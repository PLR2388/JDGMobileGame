using UnityEngine;
using VContainer;
using VContainer.Unity;
using JDG.Infrastructure.DI;
using JDG.Infrastructure.Repositories;

namespace JDG.Infrastructure.Bootstrap
{
    /// <summary>
    /// Game bootstrapper - initializes the DI container and service locator.
    /// Attach this to a GameObject in your startup scene.
    /// </summary>
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private GameLifetimeScope _lifetimeScope;

        private void Awake()
        {
            // Ensure this GameObject persists across scenes
            DontDestroyOnLoad(gameObject);

            // Initialize the service locator with the VContainer resolver
            if (_lifetimeScope != null)
            {
                ServiceLocator.Initialize(_lifetimeScope.Container);
                Debug.Log("GameBootstrapper: Service locator initialized");

                // Initialize CardRepository to load all ScriptableObject cards
                var cardRepository = ServiceLocator.GetCardRepository();
                cardRepository.Initialize();
                Debug.Log("GameBootstrapper: CardRepository initialized");
            }
            else
            {
                Debug.LogError("GameBootstrapper: GameLifetimeScope not assigned!");
            }
        }

        private void OnDestroy()
        {
            // Clean up the service locator
            ServiceLocator.Clear();
        }
    }
}
