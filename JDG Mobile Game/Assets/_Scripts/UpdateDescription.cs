using System.Globalization;
using Cards;
using Cards.InvocationCards;
using UnityEngine;

public class UpdateDescription : MonoBehaviour
{
    [SerializeField] private GameObject titleCardText;
    [SerializeField] private GameObject shortDescriptionText;
    [SerializeField] private GameObject descriptionText;
    [SerializeField] private GameObject allInvocationOptions;
    [SerializeField] private GameObject familyText;
    [SerializeField] private GameObject attackText;
    [SerializeField] private GameObject defenseText;
    [SerializeField] private GameObject collectorImage;
    [SerializeField] private GameObject cardTypeText;
    private Card card;

    // Update is called once per frame
    private void Update()
    {
        card = GetComponent<CardDisplay>().card;
        titleCardText.GetComponent<TMPro.TextMeshProUGUI>().text = card.Title;
        shortDescriptionText.GetComponent<TMPro.TextMeshProUGUI>().text = card.Description;
        descriptionText.GetComponent<TMPro.TextMeshProUGUI>().text = card.DetailedDescription;

        if (card.Type == CardType.Invocation)
        {
            allInvocationOptions.SetActive(true);
            cardTypeText.GetComponent<TMPro.TextMeshProUGUI>().text =
                string.Format(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.TYPE_CARD),
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.TYPE_INVOCATION)
                );
            var invocationCard = (InvocationCard)card;
            var families = invocationCard.BaseInvocationCardStats.Families;
            var familyFormatText = "";
            if (families.Length == 2)
            {
                familyFormatText = families[0] + ", " + families[1];
            }
            else
            {
                familyFormatText = families[0].ToString();
            }

            familyText.GetComponent<TMPro.TextMeshProUGUI>().text = familyFormatText;
            attackText.GetComponent<TMPro.TextMeshProUGUI>().text =
                invocationCard.BaseInvocationCardStats.Attack.ToString(CultureInfo.InvariantCulture);
            defenseText.GetComponent<TMPro.TextMeshProUGUI>().text =
                invocationCard.BaseInvocationCardStats.Defense.ToString(CultureInfo.InvariantCulture);
        }
        else
        {
            allInvocationOptions.SetActive(false);
            cardTypeText.GetComponent<TMPro.TextMeshProUGUI>().text =
                string.Format(LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.TYPE_CARD), card.Type.ToName());
        }

        collectorImage.SetActive(card.Collector);
    }
}