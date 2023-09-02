using System;
using _Scripts.Units.Invocation;
using UnityEngine;
using UnityEngine.Events;

namespace Cards.InvocationCards
{
    
    [Serializable]
    public class CancelInvocationEvent : UnityEvent<InGameInvocationCard>
    {
    }
    
    public class InvocationFunctions : MonoBehaviour
    {
        private PlayerCards currentPlayerCard;
        private PlayerCards opponentPlayerCards;
        private GameObject p1;
        private GameObject p2;
        [SerializeField] private Transform canvas;

        public static readonly CancelInvocationEvent CancelInvocationEvent = new CancelInvocationEvent();

        private void Start()
        {
            GameStateManager.ChangePlayer.AddListener(ChangePlayer);
            InGameMenuScript.InvocationCardEvent.AddListener(PutInvocationCard);
            TutoInGameMenuScript.InvocationCardEvent.AddListener(PutInvocationCard);
            CancelInvocationEvent.AddListener(OnCancelEffect);
            p1 = GameObject.Find("Player1");
            p2 = GameObject.Find("Player2");
            currentPlayerCard = p1.GetComponent<PlayerCards>();
            opponentPlayerCards = p2.GetComponent<PlayerCards>();
        }

        private void ChangePlayer()
        {
            currentPlayerCard = GameStateManager.Instance.IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
            opponentPlayerCards = GameStateManager.Instance.IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();
        }

        private void OnCancelEffect(InGameInvocationCard invocationCard)
        {
            if (invocationCard.CancelEffect)
            {
                foreach (var ability in invocationCard.Abilities)
                {
                    ability.CancelEffect(currentPlayerCard);
                }
            }
            else
            {
                foreach (var ability in invocationCard.Abilities)
                {
                    ability.ReactivateEffect(currentPlayerCard);
                }
            }
        }

        /// <summary>
        /// PutInvocationCard.
        /// Put an invocation card on field.
        /// Apply StartEffect and ConditionEffect of this card if there is enough place
        /// <param name="invocationCard">invocation card</param>
        /// </summary>
        private void PutInvocationCard(InGameInvocationCard invocationCard)
        {
            var size = currentPlayerCard.InvocationCards.Count;

            if (size >= 4) return;
            
            currentPlayerCard.InvocationCards.Add(invocationCard);
            currentPlayerCard.HandCards.Remove(invocationCard);

            foreach (var ability in invocationCard.Abilities)
            {
                ability.ApplyEffect(canvas, currentPlayerCard, opponentPlayerCards);
            }
        }
    }
}