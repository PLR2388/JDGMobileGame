using System.Collections.Generic;
using UnityEngine;

namespace Cards.EquipmentCards
{
    [CreateAssetMenu(fileName = "New Card", menuName = "EquipmentCard")]
    public class EquipmentCard : Card
    {
        public List<EquipmentAbilityName> EquipmentAbilities = new List<EquipmentAbilityName>();

        private void Awake()
        {
            type = CardType.Equipment;
        }
    }
}