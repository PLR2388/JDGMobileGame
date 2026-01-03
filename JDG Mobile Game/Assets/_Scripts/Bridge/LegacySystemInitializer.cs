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
            // Phase 148: Add null check to prevent setting null on static fields
            if (localizationService == null)
            {
                Debug.LogError("LegacySystemInitializer: localizationService is null! " +
                    "Extension classes will not have localization support.");
                return;
            }

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
        /// Phase 144: Uses pattern matching for safer type checking.
        /// Note: Concrete CardRepository is required because Initialize() is an implementation
        /// detail not exposed via ICardRepository interface. If a different implementation
        /// is registered, this bridge code would need to be updated.
        /// </summary>
        public static void LoadCards(ICardRepository cardRepository)
        {
            if (cardRepository == null)
            {
                Debug.LogError("LegacySystemInitializer: CardRepository is null!");
                return;
            }

            // Phase 144: Use pattern matching for safer type checking
            if (cardRepository is not CardRepository concreteRepository)
            {
                Debug.LogError($"LegacySystemInitializer: Expected CardRepository but got {cardRepository.GetType().Name}. " +
                    "This bridge code requires the concrete CardRepository implementation.");
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
