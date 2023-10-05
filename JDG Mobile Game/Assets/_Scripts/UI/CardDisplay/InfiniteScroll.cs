using System.Collections.Generic;
using System.Linq;
using Cards;
using Menu;
using UnityEngine;

public class InfiniteScroll : MonoBehaviour
{
    [SerializeField] private GameObject prefabCard;
    [SerializeField] private Transform canvas;

    private int numberOfSelectedCards;
    private int numberOfRareCards;

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
        numberOfSelectedCards = 0;
        numberOfRareCards = 0;
        DisplayAvailableCards(displayP1Card ? deck1AllCards : deck2AllCards);
    }

    /// <summary>
    /// Card has been selected, count of card updated
    /// Display message if neccessary
    /// </summary>
    /// <param name="card"></param>
    private void OnSelectCard(InGameCard card)
    {
        numberOfSelectedCards++;
        if (card.Collector)
        {
            numberOfRareCards++;
        }

        CheckNumberOfSelectedCards(card);
        CheckNumberOfRareCards(card);
    }
    
    /// <summary>
    /// Checks if the number of rare cards selected exceeds the maximum allowed limit.
    /// If it does, the provided card is unselected, and a warning message is displayed.
    /// </summary>
    /// <param name="card">The card to check and possibly unselect.</param>
    private void CheckNumberOfRareCards(InGameCard card)
    {
        if (numberOfRareCards > GameState.MaxRare)
        {
            CardSelectionManager.Instance.UnselectCard(card);
            DisplayMessageBox(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_LIMIT_COLLECTOR_CARD)
            );
        }
    }
    
    /// <summary>
    /// Checks if the total number of selected cards exceeds the maximum allowed limit.
    /// If it does, the provided card is unselected, and a warning message is displayed.
    /// </summary>
    /// <param name="card">The card to check and possibly unselect.</param>
    private void CheckNumberOfSelectedCards(InGameCard card)
    {
        if (numberOfSelectedCards > GameState.MaxDeckCards)
        {
            CardSelectionManager.Instance.UnselectCard(card);
            DisplayMessageBox(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_LIMIT_NUMBER_CARDS)
            );
        }
    }

    /// <summary>
    /// Card has been removed from selection
    /// </summary>
    /// <param name="card"></param>
    private void OnUnSelectCard(InGameCard card)
    {
        numberOfSelectedCards--;
        if (card.Collector)
        {
            numberOfRareCards--;
        }
    }

    /// <summary>
    /// Display all available card to choose from for the user to choose
    /// </summary>
    /// <param name="allCards"></param>
    private void DisplayAvailableCards(List<Card> allCards)
    {
        foreach (var card in allCards.Where(card => card.Type != CardType.Contre && !removeCardTitles.Contains(card.Title)))
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