using Cards.EffectCards;
using Cards.EquipmentCards;
using Cards.FieldCards;
using Cards.InvocationCards;
using UnityEngine;

namespace Cards
{
    public class InGameCard
    {
        public Card baseCard;
        protected string title = "";
        protected string description;
        protected string detailedDescription;
        protected CardType type;
        protected Material materialCard;
        protected bool collector;

        public CardOwner CardOwner { get; protected set; } = CardOwner.NotDefined;

        public string Title => title;

        public string Description => description;
        public string DetailedDescription => detailedDescription;

        public CardType Type => type;

        public Material MaterialCard => materialCard;

        public bool Collector => collector;

        public static InGameCard CreateInGameCard(Card card, CardOwner cardOwner)
        {
            return card switch
            {
                EquipmentCard equipmentCard => InGameEquipementCard.Init(equipmentCard, cardOwner),
                EffectCard effectCard => InGameEffectCard.Init(effectCard, cardOwner),
                InvocationCard invocationCard => InGameInvocationCard.Init(invocationCard, cardOwner),
                FieldCard fieldCard => InGameFieldCard.Init(fieldCard, cardOwner),
                _ => null
            };
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(title);
        }
    }
}