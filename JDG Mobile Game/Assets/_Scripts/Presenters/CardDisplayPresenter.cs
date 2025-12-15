using JDG.Application.Cards;
using JDG.Application.Services;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Presenter for displaying cards in a large view (MVP pattern).
/// Replaces UIManager's large card display responsibility.
/// Part of Phase 5 - UIManager decomposition.
/// Phase 39: Updated to use IInGameCard + ICardVisualService for migration to JDG.Presentation.
///
/// Note: This presenter is in the default assembly during transition.
/// It will be moved to JDG.Presentation once all dependencies use interfaces.
/// </summary>
public class CardDisplayPresenter
{
    private readonly GameObject _bigImageCard;
    private readonly Image _bigImageCardImage;
    private readonly ICardVisualService _cardVisualService;

    /// <summary>
    /// Creates a new CardDisplayPresenter with dependency injection.
    /// Phase 39: Added ICardVisualService for Material resolution.
    /// </summary>
    /// <param name="bigImageCard">The GameObject for displaying the card</param>
    /// <param name="cardVisualService">Service for resolving card visuals</param>
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
    /// Legacy constructor for backward compatibility.
    /// Uses null ICardVisualService - ShowCard will require concrete InGameCard.
    /// </summary>
    /// <param name="bigImageCard">The GameObject for displaying the card</param>
    public CardDisplayPresenter(GameObject bigImageCard) : this(bigImageCard, null)
    {
    }

    /// <summary>
    /// Displays the given card in the large card viewer.
    /// Phase 39: Updated to accept IInGameCard interface.
    /// </summary>
    /// <param name="card">Card to be displayed.</param>
    public void ShowCard(IInGameCard card)
    {
        if (_bigImageCard == null || _bigImageCardImage == null || card == null)
            return;

        _bigImageCard.SetActive(true);

        // Get material via service or direct cast for legacy compatibility
        var material = _cardVisualService?.GetMaterial(card) as Material;
        if (material == null && card is Cards.InGameCard inGameCard)
        {
            // Fallback to direct access for legacy code paths
            material = inGameCard.MaterialCard;
        }

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
