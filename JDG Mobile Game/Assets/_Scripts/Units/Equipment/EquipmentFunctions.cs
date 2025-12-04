using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

namespace Cards.EquipmentCards
{
    /// <summary>
    /// Handles the functionalities associated with equipment cards within the game.
    /// Phase 6: Refactored to delegate business logic to ICardPlacementService.
    /// </summary>
    public class EquipmentFunctions : MonoBehaviour
    {
        [SerializeField] private GameObject miniCardMenu;
        [SerializeField] private Transform canvas;

        // Phase 6: Use service for business logic
        private ICardPlacementService CardPlacementService => ServiceLocator.Get<ICardPlacementService>();

        /// <summary>
        /// Initializes listeners for equipment card events.
        /// </summary>
        private void Start()
        {
            InGameMenuScript.EquipmentCardEvent.AddListener(DisplayEquipmentPopUp);
            TutoInGameMenuScript.EquipmentCardEvent.AddListener(DisplayEquipmentPopUp);
        }

        /// <summary>
        /// Cleans up listeners upon object destruction.
        /// </summary>
        private void OnDestroy()
        {
            InGameMenuScript.EquipmentCardEvent.RemoveListener(DisplayEquipmentPopUp);
            TutoInGameMenuScript.EquipmentCardEvent.RemoveListener(DisplayEquipmentPopUp);
        }

        /// <summary>
        /// Displays a pop-up for equipping a card, showing invocations on which equipment can be added.
        /// Phase 6: Delegates business logic to CardPlacementService.
        /// </summary>
        /// <param name="equipmentCard">The equipment card the player wishes to apply.</param>
        private void DisplayEquipmentPopUp(InGameEquipmentCard equipmentCard)
        {
            // Get valid targets from service
            var validTargets = CardPlacementService.GetEquipmentTargets(equipmentCard);

            // Display card selector UI
            var config = new CardSelectorConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_INVOCATION_FOR_EQUIPMENT),
                validTargets,
                showNegativeButton: true,
                showPositiveButton: true,
                positiveAction: (card) =>
                {
                    if (card is InGameInvocationCard selectedInvocationCard)
                    {
                        // Delegate to service for business logic
                        CardPlacementService.PlaceEquipmentCard(equipmentCard, selectedInvocationCard, canvas);

                        // Hide UI after placement
                        miniCardMenu.SetActive(false);
                    }
                },
                negativeAction: () =>
                {
                    // Hide UI on cancel
                    miniCardMenu.SetActive(false);
                }
            );

            CardSelector.Instance.CreateCardSelection(canvas, config);
        }
    }
}