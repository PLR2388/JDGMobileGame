using JDG.Application.Cards;
using JDG.Application.Services;
using UnityEngine;
using UnityEngine.UI;

namespace JDG.Presentation.Presenters
{
    /// <summary>
    /// Presenter for displaying cards in a large view (MVP pattern).
    /// Replaces UIManager's large card display responsibility.
    /// Part of Phase 5 - UIManager decomposition.
    /// Phase 39: Updated to use IInGameCard + ICardVisualService for migration to JDG.Presentation.
    /// Phase 43: Migrated to JDG.Presentation - removed legacy fallback.
    /// </summary>
    public class CardDisplayPresenter
    {
        private readonly GameObject _bigImageCard;
        private readonly Image _bigImageCardImage;
        private readonly ICardVisualService _cardVisualService;

        /// <summary>
        /// Creates a new CardDisplayPresenter with dependency injection.
        /// Phase 43: ICardVisualService is now required (legacy fallback removed).
        /// </summary>
        /// <param name="bigImageCard">The GameObject for displaying the card</param>
        /// <param name="cardVisualService">Service for resolving card visuals (required)</param>
        public CardDisplayPresenter(GameObject bigImageCard, ICardVisualService cardVisualService)
        {
            _bigImageCard = bigImageCard;
            _cardVisualService = cardVisualService;
            if (_bigImageCard != null)
            {
                _bigImageCardImage = _bigImageCard.GetComponent<Image>();
            }
        }

        /// <summary>
        /// Displays the given card in the large card viewer.
        /// Phase 43: Uses ICardVisualService exclusively for material resolution.
        /// </summary>
        /// <param name="card">Card to be displayed.</param>
        public void ShowCard(IInGameCard card)
        {
            if (_bigImageCard == null || _bigImageCardImage == null || card == null)
                return;

            _bigImageCard.SetActive(true);

            // Get material via service
            var material = _cardVisualService?.GetMaterial(card) as Material;
            if (material != null)
            {
                _bigImageCardImage.material = material;
            }
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
