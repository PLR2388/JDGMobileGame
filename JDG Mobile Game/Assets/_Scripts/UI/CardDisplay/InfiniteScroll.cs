using System.Collections.Generic;
using System.Linq;
using Cards;
using Menu;
using UnityEngine;

public class InfiniteScroll : MonoBehaviour
{
    [SerializeField] private GameObject prefabCard;
    [SerializeField] private Transform canvas;

    private int numberSelected;
    private int numberRare;

    private bool displayP1Card = true;
    private List<Card> deck1AllCards;
    private List<Card> deck2AllCards;

    private readonly string[] removeCardTitles =
    {
        CardNameMappings.CardNameMap[CardNames.AttaqueDeLaTourEiffel],
        CardNameMappings.CardNameMap[CardNames.BlagueInterdite],
        CardNameMappings.CardNameMap[CardNames.UnBonTuyau]
    };

    // Start is called before the first frame update
    private void Start()
    {
        deck1AllCards = GameState.Instance.deck1AllCards;
        deck2AllCards = GameState.Instance.deck2AllCards;
        DisplayAvailableCards(deck1AllCards);
        CardSelectionManager.Instance.MultipleCardSelection = true;
        CardSelectionManager.Instance.MultipleSelectionLimit = GameState.MaxDeckCards;
        CardSelectionManager.Instance.CardSelected.AddListener(OnSelectCard);
        CardSelectionManager.Instance.CardDeselected.AddListener(OnUnSelectCard);
        CardChoice.ChangeChoicePlayer.AddListener(OnChangePlayer);
    }


    /// <summary>
    /// Reset value when another player must choose his cards
    /// </summary>
    /// <param name="numberPlayer">Number representing the current player choosing his cards</param>
    private void OnChangePlayer(int numberPlayer)
    {
        displayP1Card = numberPlayer == 1;
        numberSelected = 0;
        numberRare = 0;
        DisplayAvailableCards(displayP1Card ? deck1AllCards : deck2AllCards);
    }

    /// <summary>
    /// Card has been selected, count of card updated
    /// Display message if neccessary
    /// </summary>
    /// <param name="card"></param>
    private void OnSelectCard(InGameCard card)
    {
        numberSelected++;
        if (card.Collector)
        {
            numberRare++;
        }

        if (numberSelected > GameState.MaxDeckCards)
        {
            CardSelectionManager.Instance.UnselectCard(card);
            DisplayMessageBox(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_LIMIT_NUMBER_CARDS)
            );
        }

        if (numberRare > GameState.MaxRare)
        {
            CardSelectionManager.Instance.UnselectCard(card);
            DisplayMessageBox(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_LIMIT_COLLECTOR_CARD)
            );
        }
    }

    /// <summary>
    /// Card has been removed from selection
    /// </summary>
    /// <param name="card"></param>
    private void OnUnSelectCard(InGameCard card)
    {
        numberSelected--;
        if (card.Collector)
        {
            numberRare--;
        }
    }

    /// <summary>
    /// Display all available card to choose from for the user to choose
    /// </summary>
    /// <param name="allCards"></param>
    private void DisplayAvailableCards(List<Card> allCards)
    {
        foreach (var card in allCards.Where(card => card.Type != CardType.Contre)
                     .Where(card => !removeCardTitles.Contains(card.Title)))
        {
            CreateCardDisplay(card);
        }
    }

    /// <summary>
    /// Create a display for a card
    /// </summary>
    /// <param name="card"></param>
    private void CreateCardDisplay(Card card)
    {
        var newCard = Instantiate(prefabCard, Vector3.zero, Quaternion.identity);
        newCard.GetComponent<OnHover>().bIsInGame = false;
        newCard.transform.SetParent(transform, true);
        newCard.GetComponent<CardDisplay>().Card = card;
    }

    /// <summary>
    /// Display a warning messageBox with a ok button and a custom message
    /// </summary>
    /// <param name="msg"></param>
    private void DisplayMessageBox(string msg)
    {
        var config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
            msg,
            showOkButton: true
        );
        MessageBox.Instance.CreateMessageBox(canvas, config);
    }

    /// <summary>
    /// Remove Event listener attached
    /// </summary>
    private void OnDestroy()
    {
        CardChoice.ChangeChoicePlayer.RemoveListener(OnChangePlayer);
    }
}