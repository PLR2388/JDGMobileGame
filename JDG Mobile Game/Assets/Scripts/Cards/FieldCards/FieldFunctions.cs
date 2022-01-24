using System;
using System.Collections.Generic;
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

        private void PutFieldCard(FieldCard fieldCard)
        {
            if (currentPlayerCard.field != null) return;
            miniCardMenu.SetActive(false);
            currentPlayerCard.field = fieldCard;
            currentPlayerCard.handCards.Remove(fieldCard);
            ApplyFieldCardEffect(fieldCard, currentPlayerCard);
        }

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

        private static void ApplyChange(PlayerCards playerCards, string value, CardFamily family)
        {
            // Must be called also when invocationCards change
            var names = value.Split(';');
            foreach (var invocationCard in playerCards.invocationCards.Where(invocationCard => names.Contains(invocationCard.Nom)))
            {
                invocationCard.SetCurrentFamily(family);
            }
        }

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

        private void ChangePlayer()
        {
            currentPlayerCard = GameLoop.IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
        }
    }
}