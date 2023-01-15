using UnityEngine;

namespace Cards.EquipmentCards
{
    [CreateAssetMenu(fileName = "New Card", menuName = "EquipmentCard")]
    public class EquipmentCard : Card
    {
        [SerializeField] private EquipmentInstantEffect equipmentInstantEffect;
        [SerializeField] private EquipmentPermEffect equipmentPermEffect;

        public EquipmentInstantEffect EquipmentInstantEffect => equipmentInstantEffect;
        public EquipmentPermEffect EquipmentPermEffect => equipmentPermEffect;

        private void Awake()
        {
            type = CardType.Equipment;
        }
    }
}