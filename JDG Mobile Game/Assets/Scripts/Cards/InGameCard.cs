using UnityEngine;

namespace Cards
{
    public class InGameCard : MonoBehaviour
    {
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

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(title);
        }
    }
}
