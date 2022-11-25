using Cards;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CardSelectedEvent : UnityEvent<Card>
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
    private Card card;

    public static readonly CardSelectedEvent CardSelectedEvent = new CardSelectedEvent();
    public static readonly CardSelectedEvent CardUnselectedEvent = new CardSelectedEvent();

    private void Start()
    {
        MessageBox.NumberedCardEvent.AddListener(UpdateNumberOnCard);
        image = GetComponent<Image>();
        card = gameObject.GetComponent<CardDisplay>().card;
    }

    private void UpdateNumberOnCard(Card cardToModify, int numberToApply)
    {
        if (card.Nom == cardToModify.Nom)
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
                image.color = Color.white;
                bIsSelected = false;
                displayNumber = false;
                CardUnselectedEvent.Invoke(card);
            }
            else
            {
                image.color = Color.green;
                bIsSelected = true;
                var clickedCard = gameObject.GetComponent<CardDisplay>().card;
                CardSelectedEvent.Invoke(clickedCard);
            }
        }
        else
        {
            var currentCard = GetComponent<CardDisplay>().card;
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
}