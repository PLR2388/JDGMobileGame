using System.Collections.Generic;
using UnityEngine;

namespace Cards.FieldCards
{
    [CreateAssetMenu(fileName = "New Card", menuName = "FieldCard")]
    public class FieldCard : Card
    {
        [SerializeField] private CardFamily family;
        [SerializeField] private FieldCardEffect fieldCardEffect;

        public FieldCardEffect FieldCardEffect => fieldCardEffect;
        public CardFamily Family => family;

        public List<FieldAbilityName> FieldAbilities = new List<FieldAbilityName>();

        private void Awake()
        {
            type = CardType.Field;
        }

       
    }
}