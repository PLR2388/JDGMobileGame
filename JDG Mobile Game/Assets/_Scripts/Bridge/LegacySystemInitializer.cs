using JDG.Application.Repositories;
using JDG.Application.Services;
using JDG.Infrastructure.Repositories;
using UnityEngine;

namespace JDG.Bridge
{
    /// <summary>
    /// Bridge: Initializes legacy static fields with modern DI services.
    ///
    /// <para><b>Purpose:</b> Legacy ability class (Ability) uses static
    /// properties for services. Since DI cannot inject into static fields, this initializer
    /// bridges the gap by setting these fields after the container is built.</para>
    ///
    /// <para><b>Called From:</b> SharedServicesScope.RegisterBuildCallback()</para>
    ///
    /// <para><b>Removal Condition:</b> When all legacy ability implementations are migrated
    /// to IAbility interface. Until then, this initializer is essential infrastructure.</para>
    ///
    /// <para><b>History:</b></para>
    /// <list type="bullet">
    /// <item>Phase 46: Replaced LegacyCardLoader MonoBehaviour</item>
    /// <item>Phase 66: Removed unused GameStateService</item>
    /// <item>Phase 111: Documented as permanent bridge infrastructure</item>
    /// <item>Phase 115: Removed EffectAbility initialization (class deleted)</item>
    /// <item>Phase 116: Removed FieldAbility initialization (class deleted)</item>
    /// </list>
    /// </summary>
    public static class LegacySystemInitializer
    {
        /// <summary>
        /// Initializes all legacy static fields with DI services.
        /// Must be called after container is built.
        /// Phase 66: Removed GameStateService parameter - was never used by abilities.
        /// </summary>
        public static void Initialize(
            ILocalizationService localizationService,
            IDialogService dialogService)
        {
            // Phase 66: Static properties marked obsolete, wrapped with pragma
            // Phase 115: Removed EffectAbility initialization (class deleted)
            // Phase 116: Removed FieldAbility initialization (class deleted)
            #pragma warning disable CS0618 // Suppress obsolete warning - intentional backward compatibility
            // Initialize legacy Ability base class
            Ability.LocalizationService = localizationService;
            Ability.DialogService = dialogService;
            #pragma warning restore CS0618

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
