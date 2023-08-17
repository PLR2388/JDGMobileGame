using UnityEngine;

namespace Cards.FieldCards
{
    public class FieldFunctions : MonoBehaviour
    {
        private PlayerCards currentPlayerCard;
        private GameObject p1;
        private GameObject p2;

        [SerializeField] private GameObject miniCardMenu;

        // Start is called before the first frame update
        private void Start()
        {
            GameLoop.ChangePlayer.AddListener(ChangePlayer);
            InGameMenuScript.FieldCardEvent.AddListener(PutFieldCard);
            TutoInGameMenuScript.FieldCardEvent.AddListener(PutFieldCard);
            p1 = GameObject.Find("Player1");
            p2 = GameObject.Find("Player2");
            currentPlayerCard = p1.GetComponent<PlayerCards>();
        }

        /// <summary>
        /// PutFieldCard.
        /// Put a field card on field and apply its effect
        /// <param name="fieldCard">field card used</param>
        /// </summary>
        private void PutFieldCard(InGameFieldCard fieldCard)
        {
            if (currentPlayerCard.FieldCard != null) return;
            miniCardMenu.SetActive(false);
            currentPlayerCard.FieldCard = fieldCard;
            currentPlayerCard.handCards.Remove(fieldCard);
            foreach (var ability in fieldCard.FieldAbilities)
            {
                ability.ApplyEffect(currentPlayerCard);
            }
        }

        /// <summary>
        /// ChangePlayer.
        /// Change currentPlayerCard depending of player turn
        /// </summary>
        private void ChangePlayer()
        {
            currentPlayerCard = GameLoop.IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
        }
    }
}