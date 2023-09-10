using UnityEngine;
using UnityEngine.Events;

public class PlayerCardManager : MonoBehaviour
{
    /// <summary>
    /// Gets the PlayerCards component associated with this manager.
    /// </summary>
    public PlayerCards PlayerCards { get; private set; }

    /// <summary>
    /// Initialize references.
    /// </summary>
    private void Awake()
    {
        PlayerCards = GetComponent<PlayerCards>();
    }

    /// <summary>
    /// Draws a card from the deck. If the deck is empty, invokes the provided callback.
    /// </summary>
    /// <param name="onEmptyDeckCallback">Action to perform if the deck is empty.</param>
    public void DrawCard(UnityAction onEmptyDeckCallback)
    {
        if (PlayerCards.SkipCurrentDraw)
        {
            PlayerCards.SkipCurrentDraw = false;
        }
        else
        {
            DrawFromDeck(onEmptyDeckCallback);
        }
    }
    
    /// <summary>
    /// Helper method to draw a card from the deck and invoke a callback if the deck is empty.
    /// </summary>
    /// <param name="onEmptyDeckCallback">Action to perform if the deck is empty.</param>
    private void DrawFromDeck(UnityAction onEmptyDeckCallback)
    {
        int size = PlayerCards.Deck.Count;
        if (size > 0)
        {
            var c = PlayerCards.Deck[size - 1];
            PlayerCards.HandCards.Add(c);
            PlayerCards.Deck.RemoveAt(size - 1);
        }
        else
        {
            onEmptyDeckCallback();
        }
    }

    /// <summary>
    /// Processes end-of-turn logic for each invocation card.
    /// </summary>
    public void ProcessEndOfTurn()
    {
        var invocationCards = PlayerCards.InvocationCards;

        foreach (var invocationCard in invocationCards)
        {
            invocationCard.UnblockAttack();
            invocationCard.incrementNumberTurnOnField();
        }
    }
}