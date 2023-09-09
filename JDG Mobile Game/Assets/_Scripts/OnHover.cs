using Cards;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CardSelectedEvent : UnityEvent<InGameCard>
{
}

[RequireComponent(typeof(Image))]
public class OnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject numberText;
    private Image image;
    public int number = 0;
    private bool displayNumber = false;
    public bool bIsSelected = false;
    public bool bIsInGame = false;
    private InGameCard card;

    public static readonly CardSelectedEvent CardSelectedEvent = new CardSelectedEvent();
    public static readonly CardSelectedEvent CardUnselectedEvent = new CardSelectedEvent();

    public static readonly CardSelectedEvent ForceUnselectCardEvent = new CardSelectedEvent();

    private void Start()
    {
        CardSelector.NumberedCardEvent.AddListener(UpdateNumberOnCard);
        ForceUnselectCardEvent.AddListener(UnSelectCard);
        image = GetComponent<Image>();
        card = gameObject.GetComponent<CardDisplay>().InGameCard;
    }

    private void UnSelectCard(InGameCard cardToUnselect)
    {
        if (cardToUnselect == card)
        {
            bIsSelected = false;
        }
    }

    private void UpdateNumberOnCard(InGameCard cardToModify, int numberToApply)
    {
        if (card.Title == cardToModify.Title)
        {
            number = numberToApply;
            displayNumber = true;
        }
    }

    private void Update()
    {
        if (bIsInGame) return;
        if (bIsSelected)
        {
            image.color = Color.green;
        }
        else if (card.Collector)
        {
            image.color = Color.yellow;
        }
        else
        {
            image.color = Color.white;
        }

        if (displayNumber)
        {
            numberText.GetComponent<Text>().text = "" + number;
            numberText.SetActive(true);
        }
        else
        {
            numberText.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }

    private void OnClick()
    {
        if (!bIsInGame)
        {
            if (bIsSelected)
            {
                UnSelectCard();
            }
            else
            {
                SelectCard();
            }
        }
        else
        {
            var currentCard = GetComponent<CardDisplay>().InGameCard;
            if (SceneManager.GetActiveScene().name == "TutoPlayerGame")
            {
                TutoInGameMenuScript.EventClick.Invoke(currentCard);
            }
            else
            {
                InGameMenuScript.EventClick.Invoke(currentCard);
            }
          
        }
    }
    public void UnSelectCard()
    {

        image.color = Color.white;
        bIsSelected = false;
        displayNumber = false;
        CardUnselectedEvent.Invoke(card);
    }
    private void SelectCard()
    {

        image.color = Color.green;
        bIsSelected = true;
        var clickedCard = gameObject.GetComponent<CardDisplay>().InGameCard;
        CardSelectedEvent.Invoke(clickedCard);
    }
}