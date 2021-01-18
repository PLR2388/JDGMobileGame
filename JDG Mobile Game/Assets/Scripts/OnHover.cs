using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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
    private Image image;
    public bool bIsSelected = false;
    public bool bIsInGame = false;

    public static CardSelectedEvent cardSelectedEvent = new CardSelectedEvent();


    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (!bIsInGame)
        {
            if (bIsSelected)
            {
                image.color = Color.green;
          
            }
            else
            {
                image.color = Color.white;
            }
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        //  OnHoverEnter ();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //   OnHoverExit ();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }

    void OnHoverEnter()
    {
        image.color = Color.gray;
    }

    /*void OnHoverExit()
     {
         image.color = Color.white;
     }*/

    void OnClick()
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
                Card card = gameObject.GetComponent<CardDisplay>().card;
                cardSelectedEvent.Invoke(card);
            }
        }
        else
        {
            Card currentCard = GetComponent<CardDisplay>().card;
            InGameMenuScript.EventClick.Invoke(currentCard);
        }
    }
}