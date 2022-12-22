using Cards.EffectCards;
using Cards.FieldCards;
using Cards.InvocationCards;
using UnityEngine;

namespace Cards
{
    public class InGameCard
    {
        public Card baseCard;
        [SerializeField] protected string title = "";
        [SerializeField] protected string description;
        [SerializeField] protected string detailedDescription;
        [SerializeField] protected CardType type;
        [SerializeField] protected Material materialCard;
        [SerializeField] protected bool collector;
        [SerializeField] private CardOwner cardOwner = CardOwner.NotDefined;
        
        public CardOwner CardOwner
        {
            get => cardOwner;
            set => cardOwner = value;
        }

        public string Title => title;

        public string Description => description;
        public string DetailedDescription => detailedDescription;

        public CardType Type => type;

        public Material MaterialCard => materialCard;

        public bool Collector => collector;

        public static InGameCard CreateInGameCard(Card card)
        {
            return card switch
            {
                EquipmentCard equipmentCard => InGameEquipementCard.Init(equipmentCard),
                EffectCard effectCard => InGameEffectCard.Init(effectCard),
                InvocationCard invocationCard => InGameInvocationCard.Init(invocationCard),
                FieldCard fieldCard => InGameFieldCard.Init(fieldCard),
                _ => null
            };
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(title);
        }
    }
}
