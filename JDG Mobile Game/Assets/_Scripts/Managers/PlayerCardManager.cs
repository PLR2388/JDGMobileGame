using UnityEngine;
using UnityEngine.Events;

public class PlayerCardManager : MonoBehaviour
{
    public PlayerCards playerCards;

    private void Awake()
    {
        playerCards = GetComponent<PlayerCards>();
    }

    public void Draw(UnityAction onNoCard)
    {
        if (playerCards.SkipCurrentDraw)
        {
            playerCards.SkipCurrentDraw = false;
        }
        else
        {
            var size = playerCards.Deck.Count;
            if (size > 0)
            {
                var c = playerCards.Deck[size - 1];
                playerCards.HandCards.Add(c);
                playerCards.Deck.RemoveAt(size - 1);
            }
            else
            {
                onNoCard();
            }
        }
    }

    public void HandleEndTurn()
    {
        var invocationCards = playerCards.InvocationCards;

        foreach (var invocationCard in invocationCards)
        {
            invocationCard.UnblockAttack();
            invocationCard.incrementNumberTurnOnField();
        }
    }
}