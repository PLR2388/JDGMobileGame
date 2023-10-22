using System.Globalization;
using Cards;
using Cards.InvocationCards;
using UnityEngine;

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

    /// <summary>
    /// Initializes the component when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        cardDisplay = GetComponent<CardDisplay>();
    }

    /// <summary>
    /// Updates the details specific to Invocation cards.
    /// </summary>
    private void UpdateInvocationCardDetails()
    {
        allInvocationOptions.SetActive(true);
        cardTypeText.text =
            string.Format(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.TYPE_CARD),
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.TYPE_INVOCATION)
            );
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
    /// </summary>
    private void UpdateOtherCardDetails()
    {
        allInvocationOptions.SetActive(false);
        cardTypeText.text =
            string.Format(LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.TYPE_CARD),
                card.Type.ToName());
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