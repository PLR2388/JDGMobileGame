using UnityEngine;
using JDG.Application.Repositories;
using JDG.Infrastructure.Repositories;
using JDG.Infrastructure.Services;
using VContainer;

namespace JDG.Bridge
{
    /// <summary>
    /// MonoBehaviour that loads legacy ScriptableObject cards into CardRepository.
    /// Must be in default assembly to access old Card classes.
    /// Attach this to the same GameObject as GameBootstrapper.
    /// Phase 27: Now uses VContainer dependency injection instead of ServiceLocator.
    /// Also initializes legacy Ability.GameStateService for old ability system.
    /// </summary>
    public class LegacyCardLoader : MonoBehaviour
    {
        [Tooltip("Load cards automatically on Start")]
        [SerializeField] private bool _loadOnStart = true;

        private ICardRepository _cardRepository;
        private GameStateService _gameStateService;

        /// <summary>
        /// VContainer method injection for dependencies.
        /// Phase 27: Inject ICardRepository and GameStateService instead of using ServiceLocator.
        /// </summary>
        [Inject]
        public void Construct(ICardRepository cardRepository, GameStateService gameStateService)
        {
            _cardRepository = cardRepository;
            _gameStateService = gameStateService;

            // Phase 27: Initialize legacy Ability base class with GameStateService
            // This must be done in default assembly since JDG.Infrastructure cannot reference default assembly
            Ability.GameStateService = gameStateService;
            Debug.Log("LegacyCardLoader: Initialized Ability.GameStateService for legacy ability system");
        }

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
            var cardRepository = _cardRepository;

            if (cardRepository == null)
            {
                Debug.LogError("LegacyCardLoader: CardRepository not injected!");
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
