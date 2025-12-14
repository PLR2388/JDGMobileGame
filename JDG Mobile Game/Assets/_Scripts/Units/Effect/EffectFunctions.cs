using JDG.Application;
using JDG.Application.Services;
using UnityEngine;
using VContainer;

namespace Cards.EffectCards
{
    /// <summary>
    /// Handles the functionality related to effect cards within the game.
    /// Phase 6: Refactored to delegate business logic to ICardPlacementService.
    /// Phase 24-25: Removed ServiceLocator, using VContainer DI.
    /// Phase 34: Uses ILocalizationService instead of LocalizationSystem.Instance.
    /// Phase 35: Uses IDialogService instead of MessageBox.Instance.
    /// </summary>
    public class EffectFunctions : MonoBehaviour
    {
        [SerializeField] private GameObject miniCardMenu;
        [SerializeField] private Transform canvas;

        // Phase 24-25: Injected via VContainer
        private ICardPlacementService _cardPlacementService;

        // Phase 34: Injected via VContainer
        private ILocalizationService _localizationService;

        // Phase 35: Injected via VContainer
        private IDialogService _dialogService;

        [Inject]
        public void Construct(
            ICardPlacementService cardPlacementService,
            ILocalizationService localizationService,
            IDialogService dialogService)
        {
            _cardPlacementService = cardPlacementService;
            _localizationService = localizationService;
            _dialogService = dialogService;
        }

        /// <summary>
        /// Initialization method. Subscribes to relevant events.
        /// </summary>
        private void Start()
        {
            InGameMenuScript.EffectCardEvent.AddListener(PutEffectCard);
            TutoInGameMenuScript.EffectCardEvent.AddListener(PutEffectCard);
        }
        
        /// <summary>
        /// Cleanup method. Unsubscribes from events when the object is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            InGameMenuScript.EffectCardEvent.RemoveListener(PutEffectCard);
            TutoInGameMenuScript.EffectCardEvent.RemoveListener(PutEffectCard);
        }

        /// <summary>
        /// Processes the placement of an effect card on the field. If there are less than 4 effect cards on the field, it applies the card's effects.
        /// Otherwise, a warning is shown to the player.
        /// Phase 6: Delegates to CardPlacementService.
        /// Phase 35: Uses IDialogService instead of MessageBox.Instance.
        /// </summary>
        /// <param name="effectCard">The effect card the user put on the field.</param>
        private void PutEffectCard(InGameEffectCard effectCard)
        {
            bool success = _cardPlacementService.PlaceEffectCard(effectCard, canvas);

            if (success)
            {
                // Hide mini card menu on successful placement
                miniCardMenu.SetActive(false);
            }
            else
            {
                // Show warning if field is full (4 effects max)
                // Phase 34: Use injected ILocalizationService
                // Phase 35: Use injected IDialogService instead of MessageBox.Instance
                _dialogService.ShowMessageBox(canvas, new MessageBoxOptions
                {
                    Title = _localizationService.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                    Message = _localizationService.GetLocalizedValue(LocalizationKeys.WARNING_LIMIT_EFFECT_CARDS),
                    ShowOkButton = true
                });
            }
        }
    }
}