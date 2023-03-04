using System;
using Cards;
using Cards.InvocationCards;
using UnityEngine;
using UnityEngine.UI;

public class CardImageBuilder : MonoBehaviour
{
    public InGameCard card; 

    private Image background;
    [SerializeField] private Text title;
    [SerializeField] private GameObject family1;
    [SerializeField] private GameObject family2;
    [SerializeField] private Text quote;
    [SerializeField] private Text description;

    [SerializeField] private GameObject atkStar1;
    [SerializeField] private GameObject atkStar2;
    [SerializeField] private GameObject atkStar3;
    [SerializeField] private GameObject atkStar4;
    [SerializeField] private GameObject atkStar5;
    [SerializeField] private GameObject atkStar6;

    [SerializeField] private GameObject defStar1;
    [SerializeField] private GameObject defStar2;
    [SerializeField] private GameObject defStar3;
    [SerializeField] private GameObject defStar4;
    [SerializeField] private GameObject defStar5;
    [SerializeField] private GameObject defStar6;

    [SerializeField] private Sprite fullAtkStar;
    [SerializeField] private Sprite halfAtkStar;
    [SerializeField] private Sprite quarterAtkStar;

    [SerializeField] private Sprite fullDefStar;
    [SerializeField] private Sprite halfDefStar;
    [SerializeField] private Sprite quarterDefStar;

    [SerializeField] private Sprite humanFamily;
    [SerializeField] private Sprite japanFamily;
    [SerializeField] private Sprite rpgFamily;
    [SerializeField] private Sprite spatialFamily;
    [SerializeField] private Sprite comicsFamily;
    [SerializeField] private Sprite developerFamily;
    [SerializeField] private Sprite fistilandFamily;
    [SerializeField] private Sprite hardCornerFamily;
    [SerializeField] private Sprite incarnationFamily;
    [SerializeField] private Sprite monsterFamily;
    [SerializeField] private Sprite policeFamily;
    [SerializeField] private Sprite wizardFamily;

    [SerializeField] private Sprite invocationBackground;
    [SerializeField] private Sprite contreBackground;
    [SerializeField] private Sprite effectBackground;
    [SerializeField] private Sprite equipmentBackground;
    [SerializeField] private Sprite fieldBackground;


    private void Awake()
    {
        background = GetComponent<Image>();
    }

    void Start()
    {
        if (card != null)
        {
            BuildCard(card);
        }
    }

    private void BuildCard(InGameCard inGameCard)
    {
        family1.SetActive(false);
        family2.SetActive(false);
        atkStar1.SetActive(false);
        atkStar2.SetActive(false);
        atkStar3.SetActive(false);
        atkStar4.SetActive(false);
        atkStar5.SetActive(false);
        atkStar6.SetActive(false);

        defStar1.SetActive(false);
        defStar2.SetActive(false);
        defStar3.SetActive(false);
        defStar4.SetActive(false);
        defStar5.SetActive(false);
        defStar6.SetActive(false);


        switch (inGameCard.Type)
        {
            case CardType.Contre:
                BuildContreCard(inGameCard);
                break;
            case CardType.Effect:
                BuildEffectCard(inGameCard);
                break;
            case CardType.Equipment:
                BuildEquipmentCard(inGameCard);
                break;
            case CardType.Field:
                BuildFieldCard(inGameCard);
                break;
            case CardType.Invocation:
                BuildInvocationCard(inGameCard as InGameInvocationCard);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void BuildContreCard(InGameCard inGameCard)
    {
        background.sprite = contreBackground;
        title.text = inGameCard.Title;
        quote.text = inGameCard.Description;
        description.text = inGameCard.DetailedDescription;
    }

    private void BuildEffectCard(InGameCard inGameCard)
    {
        background.sprite = effectBackground;
        title.text = inGameCard.Title;
        quote.text = inGameCard.Description;
        description.text = inGameCard.DetailedDescription;
    }

    private void BuildEquipmentCard(InGameCard inGameCard)
    {
        background.sprite = equipmentBackground;
        title.text = inGameCard.Title;
        quote.text = inGameCard.Description;
        description.text = inGameCard.DetailedDescription;
    }

    private void BuildFieldCard(InGameCard inGameCard)
    {
        background.sprite = fieldBackground;
        title.text = inGameCard.Title;
        quote.text = inGameCard.Description;
        description.text = inGameCard.DetailedDescription;
    }

    private void BuildInvocationCard(InGameInvocationCard invocationCard)
    {
        background.sprite = invocationBackground;
        title.text = invocationCard.Title;
        quote.text = invocationCard.Description;
        description.text = invocationCard.DetailedDescription;

        switch (invocationCard.Families.Length)
        {
            case 1:
                family1.SetActive(false);
                family2.SetActive(true);
                family2.GetComponent<Image>().sprite = ConvertFamilyToSprite(invocationCard.Families[0]);
                break;
            case 2:
                family1.SetActive(true);
                family2.SetActive(true);
                family1.GetComponent<Image>().sprite = ConvertFamilyToSprite(invocationCard.Families[0]);
                family2.GetComponent<Image>().sprite = ConvertFamilyToSprite(invocationCard.Families[1]);
                break;
        }

        DisplayStars(invocationCard.Attack, true);
        DisplayStars(invocationCard.Defense, false);
    }

    private void DisplayStars(float value, bool isAtk)
    {
        int numberHalfStar = 0;
        int numberQuarterStar = 0;
        int numberOneStar = (int)value;

        float newValue = value - numberOneStar;

        if (newValue - 0.5f == 0)
        {
            numberHalfStar = 1;
        }
        else if (newValue - 0.25f == 0)
        {
            numberQuarterStar = 1;
        }

        switch (numberOneStar)
        {
            case 0:
                if (isAtk)
                {
                    if (numberHalfStar == 1)
                    {
                        atkStar3.SetActive(true);
                        atkStar3.GetComponent<Image>().sprite = halfAtkStar;
                    }
                    else if (numberQuarterStar == 1)
                    {
                        atkStar3.SetActive(true);
                        atkStar3.GetComponent<Image>().sprite = quarterAtkStar;
                    }
                }
                else
                {
                    if (numberHalfStar == 1)
                    {
                        defStar3.SetActive(true);
                        defStar3.GetComponent<Image>().sprite = halfDefStar;
                    }
                    else if (numberQuarterStar == 1)
                    {
                        defStar3.SetActive(true);
                        defStar3.GetComponent<Image>().sprite = quarterDefStar;
                    }
                }

                break;
            case 1:
                if (isAtk)
                {
                    if (numberHalfStar == 0 && numberQuarterStar == 0)
                    {
                        atkStar3.SetActive(true);
                        atkStar3.GetComponent<Image>().sprite = fullAtkStar;
                    }
                    else
                    {
                        atkStar2.SetActive(true);
                        atkStar2.GetComponent<Image>().sprite = fullAtkStar;
                        if (numberHalfStar == 1)
                        {
                            atkStar3.SetActive(true);
                            atkStar3.GetComponent<Image>().sprite = halfAtkStar;
                        }
                        else if (numberQuarterStar == 1)
                        {
                            atkStar3.SetActive(true);
                            atkStar3.GetComponent<Image>().sprite = quarterAtkStar;
                        }
                    }
                }
                else
                {
                    if (numberHalfStar == 0 && numberQuarterStar == 0)
                    {
                        defStar3.SetActive(true);
                        defStar3.GetComponent<Image>().sprite = fullDefStar;
                    }
                    else
                    {
                        defStar3.SetActive(true);
                        defStar3.GetComponent<Image>().sprite = fullDefStar;
                        if (numberHalfStar == 1)
                        {
                            defStar2.SetActive(true);
                            defStar2.GetComponent<Image>().sprite = halfDefStar;
                        }
                        else if (numberQuarterStar == 1)
                        {
                            defStar2.SetActive(true);
                            defStar2.GetComponent<Image>().sprite = quarterDefStar;
                        }
                    }
                }

                break;
            case 2:
                if (isAtk)
                {
                    if (numberHalfStar == 0 && numberQuarterStar == 0)
                    {
                        atkStar2.SetActive(true);
                        atkStar3.SetActive(true);
                        atkStar2.GetComponent<Image>().sprite = fullAtkStar;
                        atkStar3.GetComponent<Image>().sprite = fullAtkStar;
                    }
                    else
                    {
                        atkStar1.SetActive(true);
                        atkStar2.SetActive(true);
                        atkStar1.GetComponent<Image>().sprite = fullAtkStar;
                        atkStar2.GetComponent<Image>().sprite = fullAtkStar;
                        if (numberHalfStar == 1)
                        {
                            atkStar3.SetActive(true);
                            atkStar3.GetComponent<Image>().sprite = halfAtkStar;
                        }
                        else if (numberQuarterStar == 1)
                        {
                            atkStar3.SetActive(true);
                            atkStar3.GetComponent<Image>().sprite = quarterAtkStar;
                        }
                    }
                }
                else
                {
                    if (numberHalfStar == 0 && numberQuarterStar == 0)
                    {
                        defStar2.SetActive(true);
                        defStar3.SetActive(true);
                        defStar2.GetComponent<Image>().sprite = fullDefStar;
                        defStar3.GetComponent<Image>().sprite = fullDefStar;
                    }
                    else
                    {
                        defStar2.SetActive(true);
                        defStar3.SetActive(true);
                        defStar2.GetComponent<Image>().sprite = fullDefStar;
                        defStar3.GetComponent<Image>().sprite = fullDefStar;
                        if (numberHalfStar == 1)
                        {
                            defStar1.SetActive(true);
                            defStar1.GetComponent<Image>().sprite = halfDefStar;
                        }
                        else if (numberQuarterStar == 1)
                        {
                            defStar1.SetActive(true);
                            defStar1.GetComponent<Image>().sprite = quarterDefStar;
                        }
                    }
                }

                break;
            case 3:
                if (isAtk)
                {
                    if (numberHalfStar == 0 && numberQuarterStar == 0)
                    {
                        atkStar2.SetActive(true);
                        atkStar3.SetActive(true);
                        atkStar4.SetActive(true);
                        atkStar2.GetComponent<Image>().sprite = fullAtkStar;
                        atkStar3.GetComponent<Image>().sprite = fullAtkStar;
                        atkStar4.GetComponent<Image>().sprite = fullAtkStar;
                    }
                    else
                    {
                        atkStar1.SetActive(true);
                        atkStar2.SetActive(true);
                        atkStar3.SetActive(true);
                        atkStar1.GetComponent<Image>().sprite = fullAtkStar;
                        atkStar2.GetComponent<Image>().sprite = fullAtkStar;
                        atkStar3.GetComponent<Image>().sprite = fullAtkStar;
                        if (numberHalfStar == 1)
                        {
                            atkStar4.SetActive(true);
                            atkStar4.GetComponent<Image>().sprite = halfAtkStar;
                        }
                        else if (numberQuarterStar == 1)
                        {
                            atkStar4.SetActive(true);
                            atkStar4.GetComponent<Image>().sprite = quarterAtkStar;
                        }
                    }
                }
                else
                {
                    if (numberHalfStar == 0 && numberQuarterStar == 0)
                    {
                        defStar2.SetActive(true);
                        defStar3.SetActive(true);
                        defStar4.SetActive(true);
                        defStar2.GetComponent<Image>().sprite = fullDefStar;
                        defStar3.GetComponent<Image>().sprite = fullDefStar;
                        defStar4.GetComponent<Image>().sprite = fullDefStar;
                    }
                    else
                    {
                        defStar2.SetActive(true);
                        defStar3.SetActive(true);
                        defStar4.SetActive(true);
                        defStar2.GetComponent<Image>().sprite = fullDefStar;
                        defStar3.GetComponent<Image>().sprite = fullDefStar;
                        defStar4.GetComponent<Image>().sprite = fullDefStar;
                        if (numberHalfStar == 1)
                        {
                            defStar1.SetActive(true);
                            defStar1.GetComponent<Image>().sprite = halfDefStar;
                        }
                        else if (numberQuarterStar == 1)
                        {
                            defStar1.SetActive(true);
                            defStar1.GetComponent<Image>().sprite = quarterDefStar;
                        }
                    }
                }

                break;
            case 4:
                if (isAtk)
                {
                    atkStar1.SetActive(true);
                    atkStar2.SetActive(true);
                    atkStar3.SetActive(true);
                    atkStar4.SetActive(true);
                    atkStar1.GetComponent<Image>().sprite = fullAtkStar;
                    atkStar2.GetComponent<Image>().sprite = fullAtkStar;
                    atkStar3.GetComponent<Image>().sprite = fullAtkStar;
                    atkStar4.GetComponent<Image>().sprite = fullAtkStar;
                    if (numberHalfStar == 1)
                    {
                        atkStar5.SetActive(true);
                        atkStar5.GetComponent<Image>().sprite = halfAtkStar;
                    }
                    else if (numberQuarterStar == 1)
                    {
                        atkStar5.SetActive(true);
                        atkStar5.GetComponent<Image>().sprite = quarterAtkStar;
                    }
                }
                else
                {
                    defStar1.SetActive(true);
                    defStar2.SetActive(true);
                    defStar3.SetActive(true);
                    defStar4.SetActive(true);
                    defStar1.GetComponent<Image>().sprite = fullDefStar;
                    defStar2.GetComponent<Image>().sprite = fullDefStar;
                    defStar3.GetComponent<Image>().sprite = fullDefStar;
                    defStar4.GetComponent<Image>().sprite = fullDefStar;
                    if (numberHalfStar == 1)
                    {
                        defStar5.SetActive(true);
                        defStar5.GetComponent<Image>().sprite = halfDefStar;
                    }
                    else if (numberQuarterStar == 1)
                    {
                        defStar5.SetActive(true);
                        defStar5.GetComponent<Image>().sprite = quarterDefStar;
                    }
                }

                break;
            case 5:
                if (isAtk)
                {
                    atkStar1.SetActive(true);
                    atkStar2.SetActive(true);
                    atkStar3.SetActive(true);
                    atkStar4.SetActive(true);
                    atkStar5.SetActive(true);
                    atkStar1.GetComponent<Image>().sprite = fullAtkStar;
                    atkStar2.GetComponent<Image>().sprite = fullAtkStar;
                    atkStar3.GetComponent<Image>().sprite = fullAtkStar;
                    atkStar4.GetComponent<Image>().sprite = fullAtkStar;
                    atkStar5.GetComponent<Image>().sprite = fullAtkStar;
                    if (numberHalfStar == 1)
                    {
                        atkStar6.SetActive(true);
                        atkStar6.GetComponent<Image>().sprite = halfAtkStar;
                    }
                    else if (numberQuarterStar == 1)
                    {
                        atkStar6.SetActive(true);
                        atkStar6.GetComponent<Image>().sprite = quarterAtkStar;
                    }
                }
                else
                {
                    defStar1.SetActive(true);
                    defStar2.SetActive(true);
                    defStar3.SetActive(true);
                    defStar4.SetActive(true);
                    defStar5.SetActive(true);
                    defStar1.GetComponent<Image>().sprite = fullDefStar;
                    defStar2.GetComponent<Image>().sprite = fullDefStar;
                    defStar3.GetComponent<Image>().sprite = fullDefStar;
                    defStar4.GetComponent<Image>().sprite = fullDefStar;
                    defStar5.GetComponent<Image>().sprite = fullDefStar;
                    if (numberHalfStar == 1)
                    {
                        defStar6.SetActive(true);
                        defStar6.GetComponent<Image>().sprite = halfDefStar;
                    }
                    else if (numberQuarterStar == 1)
                    {
                        defStar6.SetActive(true);
                        defStar6.GetComponent<Image>().sprite = quarterDefStar;
                    }
                }

                break;
            case 6:
                if (isAtk)
                {
                    atkStar1.SetActive(true);
                    atkStar2.SetActive(true);
                    atkStar3.SetActive(true);
                    atkStar4.SetActive(true);
                    atkStar5.SetActive(true);
                    atkStar6.SetActive(true);
                    atkStar1.GetComponent<Image>().sprite = fullAtkStar;
                    atkStar2.GetComponent<Image>().sprite = fullAtkStar;
                    atkStar3.GetComponent<Image>().sprite = fullAtkStar;
                    atkStar4.GetComponent<Image>().sprite = fullAtkStar;
                    atkStar5.GetComponent<Image>().sprite = fullAtkStar;
                    atkStar6.GetComponent<Image>().sprite = fullAtkStar;
                }
                else
                {
                    defStar1.SetActive(true);
                    defStar2.SetActive(true);
                    defStar3.SetActive(true);
                    defStar4.SetActive(true);
                    defStar5.SetActive(true);
                    defStar6.SetActive(true);
                    defStar1.GetComponent<Image>().sprite = fullDefStar;
                    defStar2.GetComponent<Image>().sprite = fullDefStar;
                    defStar3.GetComponent<Image>().sprite = fullDefStar;
                    defStar4.GetComponent<Image>().sprite = fullDefStar;
                    defStar5.GetComponent<Image>().sprite = fullDefStar;
                    defStar6.GetComponent<Image>().sprite = fullDefStar;
                }

                break;
        }
    }

    private Sprite ConvertFamilyToSprite(CardFamily cardFamily)
    {
        switch (cardFamily)
        {
            case CardFamily.Comics:
                return comicsFamily;
            case CardFamily.Developer:
                return developerFamily;
            case CardFamily.Fistiland:
                return fistilandFamily;
            case CardFamily.HardCorner:
                return hardCornerFamily;
            case CardFamily.Human:
                return humanFamily;
            case CardFamily.Incarnation:
                return incarnationFamily;
            case CardFamily.Japan:
                return japanFamily;
            case CardFamily.Monster:
                return monsterFamily;
            case CardFamily.Police:
                return policeFamily;
            case CardFamily.Rpg:
                return rpgFamily;
            case CardFamily.Spatial:
                return spatialFamily;
            case CardFamily.Wizard:
                return wizardFamily;
            default:
                throw new ArgumentOutOfRangeException(nameof(cardFamily), cardFamily, null);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}