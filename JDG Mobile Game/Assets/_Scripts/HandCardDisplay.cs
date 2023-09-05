using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Cards;
using UnityEngine;using UnityEngine.Events;

[Serializable]
public class HandCardChangeEvent : UnityEvent<ObservableCollection<InGameCard>>
{
}

public class HandCardDisplay : MonoBehaviour
{
    [SerializeField] private GameObject prefabCard;
    
    private List<GameObject> createdCards = new List<GameObject>();

    public static readonly HandCardChangeEvent HandCardChange = new HandCardChangeEvent();

    private void Awake()
    {
        HandCardChange.AddListener(DisplayHandCard);
    }

    private void DisplayHandCard(ObservableCollection<InGameCard> handCards)
    {
        if (createdCards.Count > 0)
        {
            DestroyCards();
            CreateCards(handCards);
        }
        else
        {
            CreateCards(handCards);
        }
    }
    private void CreateCards(ObservableCollection<InGameCard> handCards)
    {

        foreach (var handCard in handCards)
        {
            var newCard = Instantiate(prefabCard, Vector3.zero, Quaternion.identity);
            newCard.transform.SetParent(transform, true);
            newCard.GetComponent<CardDisplay>().Card = handCard.baseCard;
            newCard.GetComponent<CardDisplay>().InGameCard = handCard;
            newCard.GetComponent<OnHover>().bIsInGame = true;

            createdCards.Add(newCard);
        }

        var rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(420 * handCards.Count, rectTransform.sizeDelta.y);
    }
    private void DestroyCards()
    {

        foreach (var createdCard in createdCards)
        {
            Destroy(createdCard);
        }

        createdCards.Clear();
    }

    private void OnEnable()
    {
        HandCardChange.AddListener(DisplayHandCard);
    }

    private void OnDisable()
    {
        HandCardChange.RemoveListener(DisplayHandCard);
        if (createdCards.Count <= 0) return;
        foreach (var createdCard in createdCards)
        {
            Destroy(createdCard);
        }

        createdCards.Clear();
    }
}