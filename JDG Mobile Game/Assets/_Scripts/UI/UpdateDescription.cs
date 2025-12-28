using System.Globalization;
using Cards;
using Cards.InvocationCards;
using JDG.Application.Services;
using UnityEngine;
using VContainer;

/// <summary>
/// Phase 39: Uses ILocalizationService instead of LocalizationSystem.Instance.
/// </summary>
public class UpdateDescription : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI titleCardText;
    [SerializeField] private TMPro.TextMeshProUGUI shortDescriptionText;
    [SerializeField] private TMPro.TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject allInvocationOptions;
    [SerializeField] private TMPro.TextMeshProUGUI familyText;
    [SerializeField] private TMPro.TextMeshProUGUI attackText;
    [SerializeField] private TMPro.TextMeshProUGUI defenseText;
    [SerializeField] private GameObject collectorImage;
    [SerializeField] private TMPro.TextMeshProUGUI cardTypeText;
    private Card card;
    private CardDisplay cardDisplay;
    private Card previousCard;

    // Phase 39: ILocalizationService instead of LocalizationSystem.Instance
    private ILocalizationService _localizationService;

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 39: Inject ILocalizationService instead of LocalizationSystem.Instance.
    /// </summary>
    [Inject]
    public void Construct(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    /// <summary>
    /// Initializes the component when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        cardDisplay = GetComponent<CardDisplay>();
    }

    /// <summary>
    /// Updates the details specific to Invocation cards.
    /// Phase 39: Uses ILocalizationService instead of LocalizationSystem.Instance.
    /// </summary>
    private void UpdateInvocationCardDetails()
    {
        allInvocationOptions.SetActive(true);
        var typeCard = GetLocalizedValue(LocalizationKeys.TYPE_CARD);
        var typeInvocation = GetLocalizedValue(LocalizationKeys.TYPE_INVOCATION);
        cardTypeText.text = string.Format(typeCard, typeInvocation);
        var invocationCard = card as InvocationCard;
        var baseInvocationCardStats = invocationCard?.BaseInvocationCardStats;
        if (baseInvocationCardStats != null)
        {
            var families = baseInvocationCardStats.Value.Families;
            var familyFormatText = "";
            if (families?.Length == 2)
            {
                familyFormatText = $"{families[0]}, {families[1]}";
            }
            else if (families != null)
            {
                familyFormatText = families[0].ToString();
            }

            familyText.text = familyFormatText;
            attackText.text =
                baseInvocationCardStats.Value.Attack.ToString(CultureInfo.InvariantCulture);
            defenseText.text =
                baseInvocationCardStats.Value.Defense.ToString(CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Updates the details specific to non-Invocation cards.
    /// Phase 39: Uses ILocalizationService instead of LocalizationSystem.Instance.
    /// </summary>
    private void UpdateOtherCardDetails()
    {
        allInvocationOptions.SetActive(false);
        var typeCard = GetLocalizedValue(LocalizationKeys.TYPE_CARD);
        cardTypeText.text = string.Format(typeCard, card.Type.ToName());
    }

    /// <summary>
    /// Helper method to get localized value using injected service.
    /// Phase 39: Added for centralized localization access.
    /// Phase 63: Removed fallback - service is always injected via VContainer.
    /// </summary>
    private string GetLocalizedValue(LocalizationKeys key)
    {
        if (_localizationService == null)
        {
            throw new System.InvalidOperationException(
                "UpdateDescription._localizationService is not set. " +
                "Ensure VContainer injection is configured correctly.");
        }
        return _localizationService.GetLocalizedValue(key.ToString());
    }

    /// <summary>
    /// Updates the visibility status of the collector image based on card properties.
    /// </summary>
    private void UpdateCollectorImageStatus()
    {
        collectorImage.SetActive(card.Collector);
    }

    /// <summary>
    /// Called every frame, updates card details if there is a change in the card.
    /// </summary>
    private void Update()
    {
        card = cardDisplay.Card;

        if (previousCard != card)
        {
            titleCardText.text = card.Title;
            shortDescriptionText.text = card.Description;
            descriptionText.text = card.DetailedDescription;

            if (card.Type == CardType.Invocation)
            {
                UpdateInvocationCardDetails();
            }
            else
            {
                UpdateOtherCardDetails();
            }

            UpdateCollectorImageStatus();
            previousCard = card;
        }
    }
}