using System.Collections.Generic;
using Cards;
using Menu;
using UnityEngine;

public class InfiniteScroll : MonoBehaviour
{
    [SerializeField] private GameObject prefabCard;
    [SerializeField] private Transform canvas;

    private int numberSelected;
    private int numberRare;
    private CardChoice cardChoice;
    private bool displayP1Card = true;
    private List<Card> deck1AllCards;
    private List<Card> deck2AllCards;

    // Start is called before the first frame update
    private void Start()
    {
        cardChoice = FindObjectOfType<CardChoice>();
        deck1AllCards = FindObjectOfType<GameState>().deck1AllCards;
        deck2AllCards = FindObjectOfType<GameState>().deck2AllCards;
        DisplayAvailableCards(deck1AllCards);
    }

    private void DisplayAvailableCards(List<Card> allCards)
    {
        foreach (var card in allCards)
        {
            if (card.Type == CardType.Contre) continue;
            if (card.Title == "Attaque de la tour Eiffel" || card.Title == "Blague interdite" ||
                card.Title == "Un bon tuyau") continue;
            var newCard = Instantiate(prefabCard, Vector3.zero, Quaternion.identity);

            newCard.GetComponent<OnHover>().bIsInGame = false;
            newCard.transform.SetParent(transform, true);
            newCard.GetComponent<CardDisplay>().card = card;
        }
    }

    private void DisplayMessageBox(string msg)
    {
        var config = new MessageBoxConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                msg,
                showOkButton: true
            );
        MessageBox.Instance.CreateMessageBox(canvas, config);
    }

    private void Update()
    {
        // If there is a change
        if (cardChoice.isPlayerOneCardChosen == displayP1Card)
        {
            displayP1Card = !cardChoice.isPlayerOneCardChosen;
            DisplayAvailableCards(displayP1Card ? deck1AllCards : deck2AllCards);
        }

        numberSelected = 0;
        numberRare = 0;
        var children = GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            var childGameObject = child.gameObject;
            if (childGameObject.GetComponent<OnHover>() == null) continue;
            var isSelected = childGameObject.GetComponent<OnHover>().bIsSelected;
            if (!isSelected) continue;
            if (numberSelected < GameState.MaxDeckCards)
            {
                numberSelected++;
                if (!childGameObject.GetComponent<CardDisplay>().card.Collector) continue;
                if (numberRare < GameState.MaxRare)
                {
                    numberRare++;
                }
                else
                {
                    childGameObject.GetComponent<OnHover>().bIsSelected = false;
                    DisplayMessageBox(
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_LIMIT_COLLECTOR_CARD)
                    );
                }
            }
            else
            {
                childGameObject.GetComponent<OnHover>().bIsSelected = false;
                DisplayMessageBox(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_LIMIT_NUMBER_CARDS)
                );
            }
        }
    }
}