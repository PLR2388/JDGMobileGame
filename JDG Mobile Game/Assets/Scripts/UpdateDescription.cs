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
        titleCardText.GetComponent<TMPro.TextMeshProUGUI>().text = card.Nom;
        shortDescriptionText.GetComponent<TMPro.TextMeshProUGUI>().text = card.Description;
        descriptionText.GetComponent<TMPro.TextMeshProUGUI>().text = card.DetailedDescription;

        if (card.Type == CardType.Invocation)
        {
            allInvocationOptions.SetActive(true);
            cardTypeText.GetComponent<TMPro.TextMeshProUGUI>().text = "Carte INVOCATION";
            var invocationCard = (InvocationCard) card;
            var families = invocationCard.GetFamily();
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
            attackText.GetComponent<TMPro.TextMeshProUGUI>().text = invocationCard.GetAttack().ToString();
            defenseText.GetComponent<TMPro.TextMeshProUGUI>().text = invocationCard.GetDefense().ToString();
        }
        else
        {
            allInvocationOptions.SetActive(false);
            switch (card.Type)
            {
                case CardType.Effect:
                    cardTypeText.GetComponent<TMPro.TextMeshProUGUI>().text = "Carte EFFET";
                    break;
                case CardType.Contre:
                    cardTypeText.GetComponent<TMPro.TextMeshProUGUI>().text = "Carte CONTRE";
                    break;
                case CardType.Equipment:
                    cardTypeText.GetComponent<TMPro.TextMeshProUGUI>().text = "Carte ÉQUIPEMENT";
                    break;
                case CardType.Field:
                    cardTypeText.GetComponent<TMPro.TextMeshProUGUI>().text = "Carte TERRAIN";
                    break;
            }
        }

        collectorImage.SetActive(card.Collector);
    }
}