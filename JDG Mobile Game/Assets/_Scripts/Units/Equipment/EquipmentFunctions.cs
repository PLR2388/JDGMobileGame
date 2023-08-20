using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

namespace Cards.EquipmentCards
{
    public class EquipmentFunctions : MonoBehaviour
    {
        [SerializeField] private GameObject miniCardMenu;
        [SerializeField] private Transform canvas;

        private void Start()
        {
            InGameMenuScript.EquipmentCardEvent.AddListener(DisplayEquipmentPopUp);
            TutoInGameMenuScript.EquipmentCardEvent.AddListener(DisplayEquipmentPopUp);
        }

        private static PlayerCards CurrentPlayerCard
        {
            get
            {
                PlayerCards currentPlayerCard;
                if (GameStateManager.Instance.IsP1Turn)
                {
                    var player = GameObject.Find("Player1");
                    currentPlayerCard = player.GetComponent<PlayerCards>();
                }
                else
                {
                    var player = GameObject.Find("Player2");
                    currentPlayerCard = player.GetComponent<PlayerCards>();
                }

                return currentPlayerCard;
            }
        }

        private static PlayerCards OpponentPlayerCard
        {
            get
            {
                PlayerCards opponentPlayerCard;
                if (GameStateManager.Instance.IsP1Turn)
                {
                    var player = GameObject.Find("Player2");
                    opponentPlayerCard = player.GetComponent<PlayerCards>();
                }
                else
                {
                    var player = GameObject.Find("Player1");
                    opponentPlayerCard = player.GetComponent<PlayerCards>();
                }

                return opponentPlayerCard;
            }
        }

        /// <summary>
        /// DisplayEquipmentPopUp.
        /// Show the player invocations cards he can put equipment on.
        /// <param name="equipmentCard">equipmentCard player want to put</param>
        /// </summary>
        private void DisplayEquipmentPopUp(InGameEquipementCard equipmentCard)
        {
            var playerCards = CurrentPlayerCard;
            var opponentInvocationCards = OpponentPlayerCard.invocationCards;
            var currentInvocationCards = playerCards.invocationCards;
            var invocationCards = currentInvocationCards.Concat(opponentInvocationCards);

            var message = DisplayEquipmentMessageBox(
                invocationCards,
                equipmentCard.EquipmentAbilities.Any(ability => ability.CanAlwaysBePut)
            );

            message.GetComponent<MessageBox>().PositiveAction = () =>
            {
                var currentSelectedInvocationCard =
                    (InGameInvocationCard)message.GetComponent<MessageBox>().GetSelectedCard();
                if (currentSelectedInvocationCard != null)
                {
                    miniCardMenu.SetActive(false);

                    foreach (var equipmentCardEquipmentAbility in equipmentCard.EquipmentAbilities)
                    {
                        equipmentCardEquipmentAbility.ApplyEffect(
                            currentSelectedInvocationCard,
                            playerCards,
                            OpponentPlayerCard
                        );
                    }

                    currentSelectedInvocationCard.SetEquipmentCard(equipmentCard);
                    playerCards.handCards.Remove(equipmentCard);
                }

                Destroy(message);
            };
            message.GetComponent<MessageBox>().NegativeAction = () =>
            {
                miniCardMenu.SetActive(false);
                Destroy(message);
            };
        }

        /// <summary>
        /// DisplayEquipmentMessageBox.
        /// Show messageBox with invocationsCards that can receive the equipment card.
        /// <param name="invocationCards">invocation card allow to receive equipment card</param>
        /// <param name="equipmentInstantEffect">equipmentCard instant effect to test if it authorizes invocations with equipment</param>
        /// </summary>
        private GameObject DisplayEquipmentMessageBox(IEnumerable<InGameInvocationCard> invocationCards, bool addAll)
        {
            var cards = new List<InGameCard>();
            foreach (var invocationCard in invocationCards)
            {
                if (addAll || invocationCard.EquipmentCard == null)
                {
                    cards.Add(invocationCard);
                }
            }
            return MessageBox.CreateMessageBoxWithCardSelector(
                canvas,
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_INVOCATION_FOR_EQUIPMENT),
                cards
            );
        }
    }
}