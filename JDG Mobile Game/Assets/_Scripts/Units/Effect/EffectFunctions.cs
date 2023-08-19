using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards.FieldCards;
using Cards.InvocationCards;
using OnePlayer;
using UnityEngine;
using UnityEngine.Events;

namespace Cards.EffectCards
{
    public class EffectFunctions : MonoBehaviour
    {
        [SerializeField] private GameObject miniCardMenu;
        [SerializeField] private Transform canvas;
        private PlayerCards currentPlayerCard;
        private PlayerStatus currentPlayerStatus;
        private PlayerCards opponentPlayerCard;
        private PlayerStatus opponentPlayerStatus;
        private GameObject p1;
        private GameObject p2;

        // Start is called before the first frame update
        private void Start()
        {
            TutoPlayerGameLoop.ChangePlayer.AddListener(ChangePlayer);
            GameLoop.ChangePlayer.AddListener(ChangePlayer);
            InGameMenuScript.EffectCardEvent.AddListener(PutEffectCard);
            TutoInGameMenuScript.EffectCardEvent.AddListener(PutEffectCard);
            p1 = GameObject.Find("Player1");
            p2 = GameObject.Find("Player2");
            currentPlayerCard = p1.GetComponent<PlayerCards>();
            currentPlayerStatus = p1.GetComponent<PlayerStatus>();
            opponentPlayerCard = p2.GetComponent<PlayerCards>();
            opponentPlayerStatus = p2.GetComponent<PlayerStatus>();
        }

        /// <summary>
        /// Called when user clicks on put for an effect card
        /// Apply effects of the card or refuse to put it if 4 effects cards are on the field
        /// </summary>
        /// <param name="effectCard">The effect card the user put on field</param>
        private void PutEffectCard(InGameEffectCard effectCard)
        {
            var size = currentPlayerCard.effectCards.Count;

            if (size < 4)
            {
                foreach (var effectCardEffectAbility in effectCard.EffectAbilities)
                {
                    effectCardEffectAbility.ApplyEffect(canvas, currentPlayerCard, opponentPlayerCard, currentPlayerStatus, opponentPlayerStatus);
                }

                miniCardMenu.SetActive(false);
                currentPlayerCard.handCards.Remove(effectCard);
                currentPlayerCard.effectCards.Add(effectCard);
            }
            else
            {
                MessageBox.CreateOkMessageBox(canvas, "Attention",
                    "Tu ne peux pas poser plus de 4 cartes effet durant un tour");
            }
        }

        private void ChangePlayer()
        {
            if (GameStateManager.Instance.IsP1Turn)
            {
                currentPlayerCard = p1.GetComponent<PlayerCards>();
                currentPlayerStatus = p1.GetComponent<PlayerStatus>();
                opponentPlayerCard = p2.GetComponent<PlayerCards>();
                opponentPlayerStatus = p2.GetComponent<PlayerStatus>();
            }
            else
            {
                currentPlayerCard = p2.GetComponent<PlayerCards>();
                currentPlayerStatus = p2.GetComponent<PlayerStatus>();
                opponentPlayerCard = p1.GetComponent<PlayerCards>();
                opponentPlayerStatus = p1.GetComponent<PlayerStatus>();
            }
        }
    }
}