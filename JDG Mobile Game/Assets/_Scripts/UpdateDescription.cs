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
            cardTypeText.GetComponent<TMPro.TextMeshProUGUI>().text = "Carte INVOCATION";
            var invocationCard = (InvocationCard)card;
            var families = invocationCard.Family;
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
                invocationCard.GetAttack().ToString(CultureInfo.InvariantCulture);
            defenseText.GetComponent<TMPro.TextMeshProUGUI>().text =
                invocationCard.GetDefense().ToString(CultureInfo.InvariantCulture);
        }
        else
        {
            allInvocationOptions.SetActive(false);
            cardTypeText.GetComponent<TMPro.TextMeshProUGUI>().text = card.Type switch
            {
                CardType.Effect => "Carte EFFET",
                CardType.Contre => "Carte CONTRE",
                CardType.Equipment => "Carte ÉQUIPEMENT",
                CardType.Field => "Carte TERRAIN",
                _ => cardTypeText.GetComponent<TMPro.TextMeshProUGUI>().text
            };
        }

        collectorImage.SetActive(card.Collector);
    }
}