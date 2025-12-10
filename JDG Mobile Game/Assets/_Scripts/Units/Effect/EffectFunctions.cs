using JDG.Application;
using UnityEngine;
using VContainer;

namespace Cards.EffectCards
{
    /// <summary>
    /// Handles the functionality related to effect cards within the game.
    /// Phase 6: Refactored to delegate business logic to ICardPlacementService.
    /// Phase 24-25: Removed ServiceLocator, using VContainer DI.
    /// </summary>
    public class EffectFunctions : MonoBehaviour
    {
        [SerializeField] private GameObject miniCardMenu;
        [SerializeField] private Transform canvas;

        // Phase 24-25: Injected via VContainer
        private ICardPlacementService _cardPlacementService;

        [Inject]
        public void Construct(ICardPlacementService cardPlacementService)
        {
            _cardPlacementService = cardPlacementService;
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
                var config = new MessageBoxConfig(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_LIMIT_EFFECT_CARDS),
                    showOkButton: true
                );
                MessageBox.Instance.CreateMessageBox(canvas, config);
            }
        }
    }
}