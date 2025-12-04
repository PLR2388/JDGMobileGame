using Sound;
using UnityEngine;

namespace Cards.FieldCards
{
    /// <summary>
    /// Provides functionalities related to field cards in the game, such as placing a card on the field.
    /// Phase 6: Refactored to delegate business logic to ICardPlacementService.
    /// </summary>
    public class FieldFunctions : MonoBehaviour
    {
        [SerializeField] private GameObject miniCardMenu; // The UI component representing a mini card menu.

        // Phase 6: Use service for business logic
        private ICardPlacementService CardPlacementService => ServiceLocator.Get<ICardPlacementService>();

        /// <summary>
        /// Initialization method that sets up listeners for relevant events.
        /// </summary>
        private void Start()
        {
            InGameMenuScript.FieldCardEvent.AddListener(PutFieldCard);
            TutoInGameMenuScript.FieldCardEvent.AddListener(PutFieldCard);
        }
        
        /// <summary>
        /// Called when the object is being destroyed and unsubscribes from events.
        /// </summary>
        private void OnDestroy()
        {
            InGameMenuScript.FieldCardEvent.RemoveListener(PutFieldCard);
            TutoInGameMenuScript.FieldCardEvent.RemoveListener(PutFieldCard);
        }

        /// <summary>
        /// Places a field card onto the game field and applies its associated effects.
        /// Phase 6: Delegates to CardPlacementService.
        /// </summary>
        /// <param name="fieldCard">The field card to be placed on the field.</param>
        private void PutFieldCard(InGameFieldCard fieldCard)
        {
            bool success = CardPlacementService.PlaceFieldCard(fieldCard);

            if (success)
            {
                // Hide mini card menu on successful placement
                miniCardMenu.SetActive(false);
            }
            // Note: No warning shown for field cards - original logic just returns silently
        }
    }
}