using UnityEngine;
using JDG.Infrastructure.DI;
using JDG.Infrastructure.Repositories;

namespace JDG.Infrastructure.Bridge
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

            // Initialize repository
            cardRepository.Initialize();

            // Load cards using the bridge initializer
            CardRepositoryInitializer.Initialize(cardRepository);

            Debug.Log("LegacyCardLoader: Legacy cards loaded successfully");
        }
    }
}
