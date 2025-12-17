using JDG.Application.Repositories;
using JDG.Application.Services;
using JDG.Infrastructure.Repositories;
using JDG.Infrastructure.Services;
using UnityEngine;

namespace JDG.Bridge
{
    /// <summary>
    /// Static initializer for legacy systems.
    /// Called from SharedServicesScope.RegisterBuildCallback to initialize
    /// legacy static fields without requiring a MonoBehaviour.
    /// Phase 46: Replaces LegacyCardLoader MonoBehaviour registration.
    /// </summary>
    public static class LegacySystemInitializer
    {
        /// <summary>
        /// Initializes all legacy static fields with DI services.
        /// Must be called after container is built.
        /// </summary>
        public static void Initialize(
            GameStateService gameStateService,
            ILocalizationService localizationService,
            IDialogService dialogService)
        {
            // Initialize legacy Ability base class
            Ability.GameStateService = gameStateService;
            Ability.LocalizationService = localizationService;
            Ability.DialogService = dialogService;

            // Initialize legacy EffectAbility base class
            EffectAbility.LocalizationService = localizationService;
            EffectAbility.DialogService = dialogService;

            // Initialize legacy FieldAbility base class
            FieldAbility.LocalizationService = localizationService;
            FieldAbility.DialogService = dialogService;

            // Initialize extension classes
            Cards.CardTypeExtensions.LocalizationService = localizationService;
            Cards.CardFamilyExtensions.LocalizationService = localizationService;
            MessageBoxBaseComponentExtensions.LocalizationService = localizationService;

            Debug.Log("LegacySystemInitializer: Initialized all legacy static fields");
        }

        /// <summary>
        /// Loads legacy card data into the repository.
        /// </summary>
        public static void LoadCards(ICardRepository cardRepository)
        {
            if (cardRepository == null)
            {
                Debug.LogError("LegacySystemInitializer: CardRepository is null!");
                return;
            }

            // Cast to concrete type to access Initialize method
            var concreteRepository = cardRepository as CardRepository;
            if (concreteRepository == null)
            {
                Debug.LogError("LegacySystemInitializer: CardRepository is not of type CardRepository!");
                return;
            }

            // Initialize repository
            concreteRepository.Initialize();

            // Load cards using the bridge initializer
            CardRepositoryInitializer.Initialize(concreteRepository);

            Debug.Log("LegacySystemInitializer: Legacy cards loaded successfully");
        }
    }
}
