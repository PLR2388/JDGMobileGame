using UnityEngine;

namespace Cards
{
    /// <summary>
    /// Represents a basic card in the game. This class can be extended to create specialized card types.
    /// </summary>
    [CreateAssetMenu(fileName = "New Card", menuName = "Card")]
    public class Card : ScriptableObject
    {
        [SerializeField] protected string title;
        [SerializeField] protected string description;
        [SerializeField] protected string detailedDescription;
        [SerializeField] protected CardType type;
        [SerializeField] protected Material materialCard;
        [SerializeField] protected bool collector;
        
        /// <summary>
        /// Gets the title of the card.
        /// </summary>
        public string Title => title;

        /// <summary>
        /// Gets the short description of the card.
        /// </summary>
        public string Description => description;
        
        /// <summary>
        /// Gets the detailed description of the card.
        /// </summary>
        public string DetailedDescription => detailedDescription;
        
        /// <summary>
        /// Gets the type of the card.
        /// </summary>
        public CardType Type => type;

        /// <summary>
        /// Gets the material used for the card's appearance.
        /// </summary>
        public Material MaterialCard => materialCard;

        /// <summary>
        /// Gets a value indicating whether the card is a collector's edition.
        /// </summary>
        public bool Collector => collector;
    }
}