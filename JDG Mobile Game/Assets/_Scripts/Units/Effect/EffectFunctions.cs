using UnityEngine;

namespace Cards.EffectCards
{
    /// <summary>
    /// Handles the functionality related to effect cards within the game.
    /// </summary>
    public class EffectFunctions : MonoBehaviour
    {
        [SerializeField] private GameObject miniCardMenu;
        [SerializeField] private Transform canvas;

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
        /// </summary>
        /// <param name="effectCard">The effect card the user put on the field.</param>
        private void PutEffectCard(InGameEffectCard effectCard)
        {
            var currentPlayerCard = CardManager.Instance.GetCurrentPlayerCards();
            var opponentPlayerCard = CardManager.Instance.GetOpponentPlayerCards();
            var currentPlayerStatus = PlayerManager.Instance.GetCurrentPlayerStatus();
            var opponentPlayerStatus = PlayerManager.Instance.GetOpponentPlayerStatus();
            var size = currentPlayerCard.EffectCards.Count;

            if (size < 4)
            {
                foreach (var effectCardEffectAbility in effectCard.EffectAbilities)
                {
                    effectCardEffectAbility.ApplyEffect(canvas, currentPlayerCard, opponentPlayerCard, currentPlayerStatus, opponentPlayerStatus);
                }

                miniCardMenu.SetActive(false);
                currentPlayerCard.HandCards.Remove(effectCard);
                currentPlayerCard.EffectCards.Add(effectCard);
            }
            else
            {
                var config = new MessageBoxConfig(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_LIMIT_EFFECT_CARDS),
                    showOkButton: true
                );
                MessageBox.Instance.CreateMessageBox(
                    canvas,
                    config
                );
            }
        }
    }
}