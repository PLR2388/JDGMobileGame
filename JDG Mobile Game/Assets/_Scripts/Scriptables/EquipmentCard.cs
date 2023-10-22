using System.Collections.Generic;
using UnityEngine;

namespace Cards.EquipmentCards
{
    /// <summary>
    /// Represents an equipment card in the game, extending the base functionality of the Card class.
    /// </summary>
    [CreateAssetMenu(fileName = "New Card", menuName = "EquipmentCard")]
    public class EquipmentCard : Card
    {
        /// <summary>
        /// List of abilities that are associated with this equipment card.
        /// </summary>
        public List<EquipmentAbilityName> EquipmentAbilities = new List<EquipmentAbilityName>();

        private void Awake()
        {
            type = CardType.Equipment;
        }
    }
}