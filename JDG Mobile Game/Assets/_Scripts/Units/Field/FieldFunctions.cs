using Sound;
using UnityEngine;

namespace Cards.FieldCards
{
    /// <summary>
    /// Provides functionalities related to field cards in the game, such as placing a card on the field.
    /// </summary>
    public class FieldFunctions : MonoBehaviour
    {
        [SerializeField] private GameObject miniCardMenu; // The UI component representing a mini card menu.

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
        /// </summary>
        /// <param name="fieldCard">The field card to be placed on the field.</param>
        private void PutFieldCard(InGameFieldCard fieldCard)
        {
            var currentPlayerCard = CardManager.Instance.GetCurrentPlayerCards();
            if (currentPlayerCard.FieldCard != null || fieldCard == null) return;
            miniCardMenu.SetActive(false);
            currentPlayerCard.FieldCard = fieldCard;
            currentPlayerCard.HandCards.Remove(fieldCard);
            foreach (var ability in fieldCard.FieldAbilities)
            {
                ability.ApplyEffect(currentPlayerCard);
            }
            AudioSystem.Instance.PlayFamilyMusic(fieldCard.Family);
        }
    }
}