using UnityEngine;
using VContainer;
using JDG.Application;
using JDG.Application.Repositories;
using JDG.Application.Services;
using JDG.Application.UseCases;

namespace JDG.Infrastructure.DI
{
    /// <summary>
    /// Service Locator adapter for gradual migration from static access to DI.
    /// Provides static access to services during the transition period.
    ///
    /// USAGE PATTERN (Strangler Fig):
    /// 1. Old code calls ServiceLocator.Get<T>()
    /// 2. ServiceLocator forwards to VContainer
    /// 3. Gradually refactor old code to use constructor injection
    /// 4. Once all code uses DI, remove ServiceLocator
    /// </summary>
    public static class ServiceLocator
    {
        private static IObjectResolver _container;

        /// <summary>
        /// Initializes the service locator with a VContainer resolver.
        /// Should be called once during game startup.
        /// </summary>
        public static void Initialize(IObjectResolver container)
        {
            if (_container != null)
            {
                Debug.LogWarning("ServiceLocator: Already initialized!");
                return;
            }

            _container = container;
            Debug.Log("ServiceLocator: Initialized with VContainer");
        }

        /// <summary>
        /// Gets a service instance from the container.
        /// </summary>
        public static T Get<T>()
        {
            if (_container == null)
            {
                Debug.LogError("ServiceLocator: Not initialized! Call ServiceLocator.Initialize() first.");
                return default;
            }

            try
            {
                return _container.Resolve<T>();
            }
            catch (VContainerException ex)
            {
                Debug.LogError($"ServiceLocator: Failed to resolve {typeof(T).Name}: {ex.Message}");
                return default;
            }
        }

        /// <summary>
        /// Checks if the service locator has been initialized.
        /// </summary>
        public static bool IsInitialized => _container != null;

        /// <summary>
        /// Clears the service locator (useful for testing).
        /// </summary>
        public static void Clear()
        {
            _container = null;
            Debug.Log("ServiceLocator: Cleared");
        }

        // ============================================
        // CONVENIENCE METHODS (for common services)
        // These will be removed as code migrates to DI
        // ============================================

        public static IEventBus GetEventBus() => Get<IEventBus>();
        public static IPlayerRepository GetPlayerRepository() => Get<IPlayerRepository>();
        public static ICardRepository GetCardRepository() => Get<ICardRepository>();
        public static IGameStateRepository GetGameStateRepository() => Get<IGameStateRepository>();

        public static DrawCardUseCase GetDrawCardUseCase() => Get<DrawCardUseCase>();
        public static PlayCardUseCase GetPlayCardUseCase() => Get<PlayCardUseCase>();
        public static AttackUseCase GetAttackUseCase() => Get<AttackUseCase>();
        public static EndTurnUseCase GetEndTurnUseCase() => Get<EndTurnUseCase>();
        public static StartGameUseCase GetStartGameUseCase() => Get<StartGameUseCase>();

        // Phase 13: Service Layer Foundation - convenience methods
        public static IAudioService GetAudioService() => Get<IAudioService>();
        public static ILocalizationService GetLocalizationService() => Get<ILocalizationService>();
        public static IDialogService GetDialogService() => Get<IDialogService>();
        public static IInputService GetInputService() => Get<IInputService>();
    }
}
