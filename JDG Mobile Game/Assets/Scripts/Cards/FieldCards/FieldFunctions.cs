using System;
using System.Linq;
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
            p1 = GameObject.Find("Player1");
            p2 = GameObject.Find("Player2");
            currentPlayerCard = p1.GetComponent<PlayerCards>();
        }

        /// <summary>
        /// PutFieldCard.
        /// Put a field card on field and apply its effect
        /// <param name="fieldCard">field card used</param>
        /// </summary>
        private void PutFieldCard(FieldCard fieldCard)
        {
            if (currentPlayerCard.field != null) return;
            miniCardMenu.SetActive(false);
            currentPlayerCard.field = fieldCard;
            currentPlayerCard.handCards.Remove(fieldCard);
            ApplyFieldCardEffect(fieldCard, currentPlayerCard);
        }

        /// <summary>
        /// Apply fieldCardEffect.
        /// <param name="fieldCard">field card used</param>
        /// <param name="playerCards">player cards of current user</param>
        /// </summary>
        public static void ApplyFieldCardEffect(FieldCard fieldCard, PlayerCards playerCards)
        {
            var fieldCardEffect = fieldCard.FieldCardEffect;

            var keys = fieldCardEffect.Keys;
            var values = fieldCardEffect.Values;

            var family = fieldCard.GetFamily();
            for (var i = 0; i < keys.Count; i++)
            {
                var value = values[i];
                switch (keys[i])
                {
                    case FieldEffect.ATK:
                        ApplyAtk(playerCards, value, family);
                        break;
                    case FieldEffect.DEF:
                        ApplyDef(playerCards, value, family);
                        break;
                    case FieldEffect.GetCard:
                        // Move to Draw phase
                        break;
                    case FieldEffect.DrawCard:
                        // Move to Draw phase
                        break;
                    case FieldEffect.Life:
                        // Move to Draw phase
                        break;
                    case FieldEffect.Change:
                        ApplyChange(playerCards, value, family);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        /// <summary>
        /// Apply ApplyAtk fieldCardEffect.
        /// Increment attack of invocation card of the same family as the field card
        /// <param name="playerCards">player cards of current user</param>
        /// <param name="value">value is a string that represent the bonus Atk</param>
        /// <param name="family">family of the field card</param>
        /// </summary>
        private static void ApplyAtk(PlayerCards playerCards, string value, CardFamily family)
        {
            // Must be called also when invocationCards change
            var atk = float.Parse(value);
            foreach (var invocationCard in playerCards.invocationCards)
            {
                if (!invocationCard.GetFamily().Contains(family)) continue;
                var newBonusAttack = invocationCard.GetBonusAttack() + atk;
                invocationCard.SetBonusAttack(newBonusAttack);
            }
        }
        
        /// <summary>
        /// Apply ApplyDef fieldCardEffect.
        /// Increment defense of invocation card of the same family as the field card
        /// <param name="playerCards">player cards of current user</param>
        /// <param name="value">value is a string that represent the bonus Def</param>
        /// <param name="family">family of the field card</param>
        /// </summary>
        private static void ApplyDef(PlayerCards playerCards, string value, CardFamily family)
        {
            // Must be called also when invocationCards change
            var def = float.Parse(value);
            foreach (var invocationCard in playerCards.invocationCards)
            {
                if (!invocationCard.GetFamily().Contains(family)) continue;
                var newBonusAttack = invocationCard.GetBonusDefense() + def;
                invocationCard.SetBonusDefense(newBonusAttack);
            }
        }

        /// <summary>
        /// Apply ApplyChange fieldCardEffect.
        /// Change family of specific card
        /// <param name="playerCards">player cards of current user</param>
        /// <param name="value">value is a string that represent cards names of card that can change their family</param>
        /// <param name="family">family of the field card</param>
        /// </summary>
        private static void ApplyChange(PlayerCards playerCards, string value, CardFamily family)
        {
            // Must be called also when invocationCards change
            var names = value.Split(';');
            foreach (var invocationCard in playerCards.invocationCards.Where(invocationCard =>
                         names.Contains(invocationCard.Nom)))
            {
                invocationCard.SetCurrentFamily(family);
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