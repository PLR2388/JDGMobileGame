using JDG.Application.Repositories;
using JDG.Application.Services;
using JDG.Infrastructure.Repositories;
using UnityEngine;

namespace JDG.Bridge
{
    /// <summary>
    /// Bridge: Initializes legacy static fields with modern DI services.
    ///
    /// <para><b>Purpose:</b> Extension methods use static properties for ILocalizationService.
    /// Since DI cannot inject into static fields, this initializer bridges the gap
    /// by setting these fields after the container is built.</para>
    ///
    /// <para><b>Called From:</b> SharedServicesScope.RegisterBuildCallback()</para>
    ///
    /// <para><b>History:</b></para>
    /// <list type="bullet">
    /// <item>Phase 46: Replaced LegacyCardLoader MonoBehaviour</item>
    /// <item>Phase 66: Removed unused GameStateService</item>
    /// <item>Phase 111: Documented as permanent bridge infrastructure</item>
    /// <item>Phase 115: Removed EffectAbility initialization (class deleted)</item>
    /// <item>Phase 116: Removed FieldAbility initialization (class deleted)</item>
    /// <item>Phase 118: Removed Ability initialization (class deleted)</item>
    /// </list>
    /// </summary>
    public static class LegacySystemInitializer
    {
        /// <summary>
        /// Initializes all legacy static fields with DI services.
        /// Must be called after container is built.
        /// Phase 118: Removed Ability class initialization (class deleted).
        /// Phase 126: Added InGameCard localization service initialization.
        /// </summary>
        public static void Initialize(ILocalizationService localizationService)
        {
            // Phase 118: Removed Ability class initialization (class deleted)
            // Initialize extension classes for localized names
            Cards.CardTypeExtensions.LocalizationService = localizationService;
            Cards.CardFamilyExtensions.LocalizationService = localizationService;
            MessageBoxBaseComponentExtensions.LocalizationService = localizationService;

            // Phase 126: Initialize card localization for multilanguage support
            Cards.InGameCard.SetLocalizationService(localizationService);

            Debug.Log("LegacySystemInitializer: Initialized extension class static fields and card localization");
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
