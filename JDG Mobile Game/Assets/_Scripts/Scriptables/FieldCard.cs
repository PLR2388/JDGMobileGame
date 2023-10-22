using System.Collections.Generic;
using UnityEngine;

namespace Cards.FieldCards
{
    /// <summary>
    /// Represents a field card in the game, extending the base functionality of the Card class.
    /// </summary>
    [CreateAssetMenu(fileName = "New Card", menuName = "FieldCard")]
    public class FieldCard : Card
    {
        [SerializeField] private CardFamily family;
        
        /// <summary>
        /// Gets the family to which this field card belongs.
        /// </summary>
        public CardFamily Family => family;

        /// <summary>
        /// List of abilities that are associated with this field card.
        /// </summary>
        public List<FieldAbilityName> FieldAbilities = new List<FieldAbilityName>();

        private void Awake()
        {
            type = CardType.Field;
        }

       
    }
}