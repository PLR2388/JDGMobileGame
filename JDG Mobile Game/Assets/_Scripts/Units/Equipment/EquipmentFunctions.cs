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
        private void DisplayEquipmentPopUp(InGameEquipmentCard equipmentCard)
        {
            var playerCards = CurrentPlayerCard;
            var opponentInvocationCards = OpponentPlayerCard.InvocationCards;
            var currentInvocationCards = playerCards.InvocationCards;
            var invocationCards = currentInvocationCards.Concat(opponentInvocationCards);

            var cards = new List<InGameCard>();
            var addAll = equipmentCard.EquipmentAbilities.Any(ability => ability.CanAlwaysBePut);
            foreach (var invocationCard in invocationCards)
            {
                if (addAll || invocationCard.EquipmentCard == null)
                {
                    cards.Add(invocationCard);
                }
            }
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
                                OpponentPlayerCard
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