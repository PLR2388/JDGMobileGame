using UnityEngine;

namespace Cards
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Card")]
    public class Card : ScriptableObject
    {
        [SerializeField] protected string title;
        [SerializeField] protected string description;
        [SerializeField] protected string detailedDescription;
        [SerializeField] protected CardType type;
        [SerializeField] protected Material materialCard;
        [SerializeField] protected bool collector;
        
        public string Title => title;

        public string Description => description;
        public string DetailedDescription => detailedDescription;

        public CardType Type => type;

        public Material MaterialCard => materialCard;

        public bool Collector => collector;
    }
}