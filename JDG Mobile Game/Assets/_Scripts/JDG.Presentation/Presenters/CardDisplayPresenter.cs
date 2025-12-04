using Cards;
using UnityEngine;
using UnityEngine.UI;

namespace JDG.Presentation.Presenters
{
    /// <summary>
    /// Presenter for displaying cards in a large view (MVP pattern).
    /// Replaces UIManager's large card display responsibility.
    /// Part of Phase 5 - UIManager decomposition.
    /// </summary>
    public class CardDisplayPresenter
    {
        private readonly GameObject _bigImageCard;
        private readonly Image _bigImageCardImage;

        public CardDisplayPresenter(GameObject bigImageCard)
        {
            _bigImageCard = bigImageCard;
            if (_bigImageCard != null)
            {
                _bigImageCardImage = _bigImageCard.GetComponent<Image>();
            }
        }

        /// <summary>
        /// Displays the given card in the large card viewer.
        /// </summary>
        /// <param name="card">Card to be displayed.</param>
        public void ShowCard(InGameCard card)
        {
            if (_bigImageCard == null || _bigImageCardImage == null || card == null)
                return;

            _bigImageCard.SetActive(true);
            _bigImageCardImage.material = card.MaterialCard;
        }

        /// <summary>
        /// Hides the large card viewer.
        /// </summary>
        public void HideCard()
        {
            if (_bigImageCard != null)
            {
                _bigImageCard.SetActive(false);
            }
        }
    }
}
