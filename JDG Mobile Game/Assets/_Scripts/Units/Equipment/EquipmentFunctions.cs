using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

namespace Cards.EquipmentCards
{
    /// <summary>
    /// Handles the functionalities associated with equipment cards within the game.
    /// </summary>
    public class EquipmentFunctions : MonoBehaviour
    {
        [SerializeField] private GameObject miniCardMenu;
        [SerializeField] private Transform canvas;

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
        /// </summary>
        /// <param name="equipmentCard">The equipment card the player wishes to apply.</param>
        private void DisplayEquipmentPopUp(InGameEquipmentCard equipmentCard)
        {
            var playerCards = CardManager.Instance.GetCurrentPlayerCards();
            var opponentInvocationCards = CardManager.Instance.GetOpponentPlayerCards().InvocationCards;
            var currentInvocationCards = playerCards.InvocationCards;
            var invocationCards = currentInvocationCards.Concat(opponentInvocationCards);

            var addAll = equipmentCard.EquipmentAbilities.Any(ability => ability.CanAlwaysBePut);
            var cards = invocationCards.Where(invocationCard => addAll || invocationCard.EquipmentCard == null).Cast<InGameCard>().ToList();
            var config = new CardSelectorConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_INVOCATION_FOR_EQUIPMENT),
                cards,
                showNegativeButton: true,
                showPositiveButton: true,
                positiveAction: (card) =>
                {
                    if (card is InGameInvocationCard currentSelectedInvocationCard)
                    {
                        miniCardMenu.SetActive(false);

                        foreach (var equipmentCardEquipmentAbility in equipmentCard.EquipmentAbilities)
                        {
                            equipmentCardEquipmentAbility.ApplyEffect(
                                currentSelectedInvocationCard,
                                playerCards,
                                CardManager.Instance.GetOpponentPlayerCards()
                            );
                        }

                        currentSelectedInvocationCard.SetEquipmentCard(equipmentCard);
                        playerCards.HandCards.Remove(equipmentCard);
                    }
                },
                negativeAction: () =>
                {
                    miniCardMenu.SetActive(false);
                }
            );
            CardSelector.Instance.CreateCardSelection(canvas, config);
        }
    }
}