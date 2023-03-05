using System;
using Cards;
using Cards.InvocationCards;
using UnityEngine;
using UnityEngine.UI;

public class CardImageBuilder : MonoBehaviour
{
    public InGameCard inGameCard;

    [SerializeField] public Card card;

    public bool bIsFaceHidden = false;

    private Image background;
    [SerializeField] private Image image;
    [SerializeField] private Text title;
    [SerializeField] private GameObject family1;
    [SerializeField] private GameObject family2;
    [SerializeField] private Text quote;
    [SerializeField] private Text description;
    [SerializeField] private RectTransform quotePosition;
    [SerializeField] private RectTransform descriptionPosition;
    

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

    private float yQuotePositionInvocation = -120f;
    private float yDescriptionPositionInvocation = -300f;
    
    private float yQuotePosition = -185f;
    private float yDescriptionPosition = -400f;


    private void Awake()
    {
        background = GetComponent<Image>();
        
        if (card != null && inGameCard == null)
        {
            inGameCard = InGameCard.CreateInGameCard(card, CardOwner.NotDefined);
        }

        if (card == null && inGameCard != null)
        {
            card = inGameCard.baseCard;
        }
    }

    void Start()
    {

        if (inGameCard != null)
        {
            BuildInGameCard(inGameCard);
        } else if (card != null)
        {
            BuildCard(card);
        }
    }

    private void Update()
    {

        if (card != null && inGameCard == null)
        {
            inGameCard = InGameCard.CreateInGameCard(card, CardOwner.NotDefined);
        }

        if (card == null && inGameCard != null)
        {
            card = inGameCard.baseCard;
        }
    }

    private void SetQuoteDescriptionPosition(bool isInvocation)
    {
        if (isInvocation)
        {
            var position = quotePosition.localPosition;
            position = new Vector3(position.x, yQuotePositionInvocation, position.z);
            quotePosition.localPosition = position;

            var position1 = descriptionPosition.localPosition;
            position1 = new Vector3(position1.x, yDescriptionPositionInvocation, position1.z);
            descriptionPosition.localPosition = position1;
        }
        else
        {
            var position = quotePosition.localPosition;
            position = new Vector3(position.x, yQuotePosition, position.z);
            quotePosition.localPosition = position;

            var position1 = descriptionPosition.localPosition;
            position1 = new Vector3(position1.x, yDescriptionPosition, position1.z);
            descriptionPosition.localPosition = position1;
        }
    }

    private void BuildCard(Card card)
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

        image.sprite = card.Image;

        SetQuoteDescriptionPosition(false);

        switch (card.Type)
        {
            case CardType.Contre:
                BuildContreCard(card);
                break;
            case CardType.Effect:
                BuildEffectCard(card);
                break;
            case CardType.Equipment:
                BuildEquipmentCard(card);
                break;
            case CardType.Field:
                BuildFieldCard(card);
                break;
            case CardType.Invocation:
                BuildInvocationCard(card as InvocationCard);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void BuildInGameCard(InGameCard inGameCard)
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

        image.sprite = inGameCard.Image;
        
        SetQuoteDescriptionPosition(false);
        
        switch (inGameCard.Type)
        {
            case CardType.Contre:
                BuildInGameContreCard(inGameCard);
                break;
            case CardType.Effect:
                BuildInGameEffectCard(inGameCard);
                break;
            case CardType.Equipment:
                BuildInGameEquipmentCard(inGameCard);
                break;
            case CardType.Field:
                BuildInGameFieldCard(inGameCard);
                break;
            case CardType.Invocation:
                BuildInGameInvocationCard(inGameCard as InGameInvocationCard);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void BuildContreCard(Card card)
    {
        background.sprite = contreBackground;
        title.text = card.Nom;
        quote.text = "« " + card.Description + " »";
        description.text = card.DetailedDescription;
    }

    private void BuildInGameContreCard(InGameCard inGameCard)
    {
        background.sprite = contreBackground;
        title.text = inGameCard.Title;
        quote.text = "« " + inGameCard.Description + " »";
        description.text = inGameCard.DetailedDescription;
    }
    
    private void BuildEffectCard(Card card)
    {
        background.sprite = effectBackground;
        title.text = card.Nom;
        quote.text = "« " + card.Description  + " »";
        description.text = card.DetailedDescription;
    }

    private void BuildInGameEffectCard(InGameCard inGameCard)
    {
        background.sprite = effectBackground;
        title.text = inGameCard.Title;
        quote.text = "« " + inGameCard.Description + " »";
        description.text = inGameCard.DetailedDescription;
    }
    
    private void BuildEquipmentCard(Card card)
    {
        background.sprite = equipmentBackground;
        title.text = card.Nom;
        quote.text = "« " + card.Description + " »";
        description.text = card.DetailedDescription;
    }

    private void BuildInGameEquipmentCard(InGameCard inGameCard)
    {
        background.sprite = equipmentBackground;
        title.text = inGameCard.Title;
        quote.text = inGameCard.Description + " »";
        description.text = inGameCard.DetailedDescription;
    }
    
    private void BuildFieldCard(Card card)
    {
        background.sprite = fieldBackground;
        title.text = card.Nom;
        quote.text = "« " + card.Description + " »";
        description.text = card.DetailedDescription;
    }

    private void BuildInGameFieldCard(InGameCard inGameCard)
    {
        background.sprite = fieldBackground;
        title.text = inGameCard.Title;
        quote.text = "« " + inGameCard.Description + " »";
        description.text = inGameCard.DetailedDescription;
    }

    private void BuildInvocationCard(InvocationCard invocationCard)
    {
        SetQuoteDescriptionPosition(true);
        background.sprite = invocationBackground;
        title.text = card.Nom;
        quote.text = "« " + card.Description + " »";
        description.text = card.DetailedDescription;
        
        switch (invocationCard.Family.Length)
        {
            case 1:
                family1.SetActive(false);
                family2.SetActive(true);
                family2.GetComponent<Image>().sprite = ConvertFamilyToSprite(invocationCard.Family[0]);
                break;
            case 2:
                family1.SetActive(true);
                family2.SetActive(true);
                family1.GetComponent<Image>().sprite = ConvertFamilyToSprite(invocationCard.Family[0]);
                family2.GetComponent<Image>().sprite = ConvertFamilyToSprite(invocationCard.Family[1]);
                break;
        }

        DisplayStars(invocationCard.GetAttack(), true);
        DisplayStars(invocationCard.GetDefense(), false);
    }

    private void BuildInGameInvocationCard(InGameInvocationCard invocationCard)
    {
        SetQuoteDescriptionPosition(true);
        background.sprite = invocationBackground;
        title.text = invocationCard.Title;
        quote.text = "« " + invocationCard.Description + " »";
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
}