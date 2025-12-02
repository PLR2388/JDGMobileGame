using UnityEngine;
using JDG.Infrastructure.DI;
using JDG.Infrastructure.Repositories;

namespace JDG.Bridge
{
    /// <summary>
    /// MonoBehaviour that loads legacy ScriptableObject cards into CardRepository.
    /// Must be in default assembly to access old Card classes.
    /// Attach this to the same GameObject as GameBootstrapper.
    /// </summary>
    public class LegacyCardLoader : MonoBehaviour
    {
        [Tooltip("Load cards automatically on Start")]
        [SerializeField] private bool _loadOnStart = true;

        private void Start()
        {
            if (_loadOnStart)
            {
                LoadCards();
            }
        }

        /// <summary>
        /// Loads all ScriptableObject cards from Resources/Cards into CardRepository.
        /// </summary>
        public void LoadCards()
        {
            var cardRepository = ServiceLocator.GetCardRepository();

            if (cardRepository == null)
            {
                Debug.LogError("LegacyCardLoader: CardRepository not found in ServiceLocator!");
                return;
            }

            // Cast to concrete type to access Initialize and RegisterCardDefinition methods
            var concreteRepository = cardRepository as CardRepository;
            if (concreteRepository == null)
            {
                Debug.LogError("LegacyCardLoader: CardRepository is not of type CardRepository!");
                return;
            }

            // Initialize repository
            concreteRepository.Initialize();

            // Load cards using the bridge initializer
            CardRepositoryInitializer.Initialize(concreteRepository);

            Debug.Log("LegacyCardLoader: Legacy cards loaded successfully");
        }
    }
}
