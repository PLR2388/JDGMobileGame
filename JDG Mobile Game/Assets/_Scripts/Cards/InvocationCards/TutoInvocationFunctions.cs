using _Scripts.Units.Invocation;
using OnePlayer;
using UnityEngine;

namespace Cards.InvocationCards
{
    public class TutoInvocationFunctions : MonoBehaviour
    {
        private PlayerCards currentPlayerCard;
        private PlayerCards opponentPlayerCards;
        private GameObject p1;
        private GameObject p2;
        [SerializeField] private GameObject inHandButton;
        [SerializeField] private Transform canvas;
        [SerializeField] private GameObject invocationMenu;

        private void Start()
        {
            TutoPlayerGameLoop.ChangePlayer.AddListener(ChangePlayer);
            TutoInGameMenuScript.InvocationCardEvent.AddListener(PutInvocationCard);
            p1 = GameObject.Find("Player1");
            p2 = GameObject.Find("Player2");
            currentPlayerCard = p1.GetComponent<PlayerCards>();
            opponentPlayerCards = p2.GetComponent<PlayerCards>();
        }

        private void ChangePlayer()
        {
            currentPlayerCard = GameLoop.IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
            opponentPlayerCards = GameLoop.IsP1Turn ? p2.GetComponent<PlayerCards>() : p1.GetComponent<PlayerCards>();
        }
        
        /// <summary>
        /// PutInvocationCard.
        /// Put an invocation card on field.
        /// Apply StartEffect and ConditionEffect of this card if there is enough place
        /// <param name="invocationCard">invocation card</param>
        /// </summary>
        private void PutInvocationCard(InGameInvocationCard invocationCard)
        {
            var size = currentPlayerCard.invocationCards.Count;

            if (size >= 4) return;
            currentPlayerCard.invocationCards.Add(invocationCard);
            currentPlayerCard.handCards.Remove(invocationCard);
        }
    }
}