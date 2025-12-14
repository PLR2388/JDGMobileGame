using UnityEngine;
using JDG.Application.Repositories;
using JDG.Application.Services;
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
    /// Phase 38: Added ILocalizationService and IDialogService initialization.
    /// </summary>
    public class LegacyCardLoader : MonoBehaviour
    {
        [Tooltip("Load cards automatically on Start")]
        [SerializeField] private bool _loadOnStart = true;

        private ICardRepository _cardRepository;
        private GameStateService _gameStateService;
        private ILocalizationService _localizationService;
        private IDialogService _dialogService;

        /// <summary>
        /// VContainer method injection for dependencies.
        /// Phase 27: Inject ICardRepository and GameStateService instead of using ServiceLocator.
        /// Phase 38: Added ILocalizationService and IDialogService for legacy ability system.
        /// </summary>
        [Inject]
        public void Construct(
            ICardRepository cardRepository,
            GameStateService gameStateService,
            ILocalizationService localizationService,
            IDialogService dialogService)
        {
            _cardRepository = cardRepository;
            _gameStateService = gameStateService;
            _localizationService = localizationService;
            _dialogService = dialogService;

            // Phase 27: Initialize legacy Ability base class with GameStateService
            // This must be done in default assembly since JDG.Infrastructure cannot reference default assembly
            Ability.GameStateService = gameStateService;

            // Phase 38: Initialize legacy Ability base class with ILocalizationService and IDialogService
            // Eliminates LocalizationSystem.Instance, MessageBox.Instance, and CardSelector.Instance calls
            Ability.LocalizationService = localizationService;
            Ability.DialogService = dialogService;

            // Phase 38: Initialize legacy EffectAbility base class with ILocalizationService and IDialogService
            // EffectAbility is separate from Ability, so needs its own initialization
            EffectAbility.LocalizationService = localizationService;
            EffectAbility.DialogService = dialogService;

            // Phase 38: Initialize legacy FieldAbility base class with ILocalizationService and IDialogService
            // FieldAbility is separate from Ability and EffectAbility, so needs its own initialization
            FieldAbility.LocalizationService = localizationService;
            FieldAbility.DialogService = dialogService;

            // Phase 39: Initialize extension classes with ILocalizationService
            // These static extension methods need the service for localization
            Cards.CardTypeExtensions.LocalizationService = localizationService;
            Cards.CardFamilyExtensions.LocalizationService = localizationService;
            MessageBoxBaseComponentExtensions.LocalizationService = localizationService;

            Debug.Log("LegacyCardLoader: Initialized Ability, EffectAbility, FieldAbility, CardType, CardFamily, and MessageBox services for legacy systems");
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
