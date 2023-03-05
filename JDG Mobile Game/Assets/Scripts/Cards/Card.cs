using UnityEngine;

namespace Cards
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Card")]
    public class Card : ScriptableObject
    {
        [SerializeField] protected string nom = "";
        [SerializeField] protected string description;
        [SerializeField] protected string descriptionDetaillee;
        [SerializeField] protected CardType type;
        [SerializeField] protected Material materialCard;
        [SerializeField] protected bool collector;
        [SerializeField] protected Sprite image;
        
        public string Nom => nom;

        public string Description => description;
        public string DetailedDescription => descriptionDetaillee;

        public CardType Type => type;

        public Material MaterialCard => materialCard;

        public bool Collector => collector;

        public Sprite Image => image;
    }
}