using Cards;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class CardSelectedEvent : UnityEvent<Card>
{
}

[RequireComponent(typeof(Image))]
public class OnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Image image;
    public bool bIsSelected = false;
    public bool bIsInGame = false;
    private Card card;

    public static readonly CardSelectedEvent CardSelectedEvent = new CardSelectedEvent();


    private void Start()
    {
        image = GetComponent<Image>();
        card = gameObject.GetComponent<CardDisplay>().card;
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

    // for test purposes
    public void OnClick()
    {
        if (!bIsInGame)
        {
            if (bIsSelected)
            {
                image.color = Color.white;
                bIsSelected = false;
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
            InGameMenuScript.EventClick.Invoke(currentCard);
        }
    }
}