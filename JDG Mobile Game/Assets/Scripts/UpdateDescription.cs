using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    void Update()
    {
        card = GetComponent<CardDisplay>().card;
        titleCardText.GetComponent<TMPro.TextMeshProUGUI>().text = card.GetNom();
        shortDescriptionText.GetComponent<TMPro.TextMeshProUGUI>().text = card.GetDescription();
        descriptionText.GetComponent<TMPro.TextMeshProUGUI>().text = card.GetDescriptionDetaillee();

        if (card.GetType() == "invocation")
        {
            allInvocationOptions.SetActive(true);
            cardTypeText.GetComponent<TMPro.TextMeshProUGUI>().text = "Carte INVOCATION";
            InvocationCard invocationCard = (InvocationCard)card;
            string[] families = invocationCard.GetFamily();
            string familyFormatText = "";
            if (families.Length == 2)
            {
                familyFormatText = families[0] + ", " + families[1];
            }
            else
            {
                familyFormatText = families[0];
            }

            familyText.GetComponent<TMPro.TextMeshProUGUI>().text = familyFormatText;
            attackText.GetComponent<TMPro.TextMeshProUGUI>().text = invocationCard.GetAttack().ToString();
            defenseText.GetComponent<TMPro.TextMeshProUGUI>().text = invocationCard.GetDefense().ToString();
        }
        else
        {
            allInvocationOptions.SetActive(false);
            switch (card.GetType())
            {
                case "effect":
                    cardTypeText.GetComponent<TMPro.TextMeshProUGUI>().text = "Carte EFFET";
                    break;
                case "contre":
                    cardTypeText.GetComponent<TMPro.TextMeshProUGUI>().text = "Carte CONTRE";
                    break;
                case "equipment":
                    cardTypeText.GetComponent<TMPro.TextMeshProUGUI>().text = "Carte ÉQUIPEMENT";
                    break;
                case "field" :
                    cardTypeText.GetComponent<TMPro.TextMeshProUGUI>().text = "Carte TERRAIN";
                    break;
            }
        }

        if (card.IsCollector())
        {
            collectorImage.SetActive(true);
        }
        else
        {
            collectorImage.SetActive(false);
        }
    }
}
